using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// Come gestire l'aggancio al NavMesh in fase di spawn.
/// </summary>
public enum ENavMeshSpawnMode
{
    /// <summary>Il NavMesh viene ignorato: il prefab è posizionato al punto random grezzo.</summary>
    Ignore,

    /// <summary>
    /// Il punto random viene agganciato al NavMesh più vicino entro una distanza.
    /// Se non trova NavMesh, ritenta con un nuovo punto; dopo troppi tentativi salta l'istanza.
    /// </summary>
    SnapToNavMesh
}

/// <summary>
/// Tool editor che istanzia N volte un prefab in posizioni casuali dentro l'area
/// definita da due punti nel mondo. Permette di scegliere su quali assi
/// distribuire le posizioni e se tenere conto del NavMesh.
/// <para>
/// Deve risiedere in una cartella "Editor". Aprire da: Tools ▸ NPC ▸ Random Prefab Spawner.
/// </para>
/// </summary>
public class PrefabAreaSpawnerWindow : EditorWindow
{
    private const string UndoGroupName = "Spawn Prefabs In Area";

    [Header("Prefab")]
    private GameObject _prefab;
    private int _count = 10;
    private Transform _parent;

    [Header("Area")]
    private Vector3 _pointA = new Vector3(-5f, 0f, -5f);
    private Vector3 _pointB = new Vector3(5f, 0f, 5f);

    [Header("Assi di distribuzione")]
    private bool _spreadX = true;
    private bool _spreadY = false;
    private bool _spreadZ = true;

    [Header("NavMesh")]
    private ENavMeshSpawnMode _navMeshMode = ENavMeshSpawnMode.SnapToNavMesh;
    private float _navMeshSampleDistance = 2f;
    private int _maxAttemptsPerSpawn = 20;

    [Header("Extra")]
    private bool _randomYRotation = true;
    private bool _useSeed = false;
    private int _seed = 0;

    private bool _drawSceneGizmos = true;

    /// <summary>Apre la finestra del tool.</summary>
    [MenuItem("Tools/NPC/Random Prefab Spawner")]
    private static void Open()
    {
        PrefabAreaSpawnerWindow window = GetWindow<PrefabAreaSpawnerWindow>();
        window.titleContent = new GUIContent("Prefab Spawner");
        window.minSize = new Vector2(320f, 440f);
        window.Show();
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel);
        _prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), false);
        _count = Mathf.Max(0, EditorGUILayout.IntField("Quantità", _count));
        _parent = (Transform)EditorGUILayout.ObjectField("Parent (opzionale)", _parent, typeof(Transform), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Area (due punti nel mondo)", EditorStyles.boldLabel);
        _pointA = EditorGUILayout.Vector3Field("Punto A", _pointA);
        _pointB = EditorGUILayout.Vector3Field("Punto B", _pointB);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("A = Selezione"))
                TrySetFromSelection(ref _pointA);
            if (GUILayout.Button("B = Selezione"))
                TrySetFromSelection(ref _pointB);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Assi di distribuzione", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Sugli assi non selezionati la posizione è fissa al valore del Punto A.",
            MessageType.None);
        using (new EditorGUILayout.HorizontalScope())
        {
            _spreadX = GUILayout.Toggle(_spreadX, "X", "Button");
            _spreadY = GUILayout.Toggle(_spreadY, "Y", "Button");
            _spreadZ = GUILayout.Toggle(_spreadZ, "Z", "Button");
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("NavMesh", EditorStyles.boldLabel);
        _navMeshMode = (ENavMeshSpawnMode)EditorGUILayout.EnumPopup("Modalità", _navMeshMode);
        if (_navMeshMode == ENavMeshSpawnMode.SnapToNavMesh)
        {
            _navMeshSampleDistance = Mathf.Max(0f, EditorGUILayout.FloatField("Distanza campionamento", _navMeshSampleDistance));
            _maxAttemptsPerSpawn = Mathf.Max(1, EditorGUILayout.IntField("Tentativi per istanza", _maxAttemptsPerSpawn));
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Extra", EditorStyles.boldLabel);
        _randomYRotation = EditorGUILayout.Toggle("Rotazione Y casuale", _randomYRotation);
        _useSeed = EditorGUILayout.Toggle("Usa seed", _useSeed);
        if (_useSeed)
            _seed = EditorGUILayout.IntField("Seed", _seed);
        _drawSceneGizmos = EditorGUILayout.Toggle("Mostra area in Scene", _drawSceneGizmos);

        EditorGUILayout.Space();
        using (new EditorGUI.DisabledScope(_prefab == null || _count <= 0))
        {
            if (GUILayout.Button("Spawn", GUILayout.Height(32f)))
                Spawn();
        }

        if (_prefab == null)
            EditorGUILayout.HelpBox("Assegna un prefab per abilitare lo spawn.", MessageType.Info);
    }

    /// <summary>
    /// Esegue lo spawn delle istanze e le raggruppa in un unico step di Undo.
    /// </summary>
    private void Spawn()
    {
        if (_prefab == null || _count <= 0)
            return;

        if (_useSeed)
            Random.InitState(_seed);

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName(UndoGroupName);
        int undoGroup = Undo.GetCurrentGroup();

        var spawned = new System.Collections.Generic.List<GameObject>(_count);
        int skipped = 0;

        for (int i = 0; i < _count; i++)
        {
            if (!TryResolvePosition(out Vector3 position))
            {
                skipped++;
                continue;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
            if (instance == null)
                continue;

            instance.transform.position = position;
            instance.transform.rotation = _randomYRotation
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : Quaternion.identity;

            if (_parent != null)
                instance.transform.SetParent(_parent, true);

            Undo.RegisterCreatedObjectUndo(instance, UndoGroupName);
            spawned.Add(instance);
        }

        Undo.CollapseUndoOperations(undoGroup);

        if (spawned.Count > 0)
            Selection.objects = spawned.ToArray();

        Debug.Log($"[PrefabAreaSpawner] Spawnati {spawned.Count}/{_count} '{_prefab.name}'" +
                  (skipped > 0 ? $" (saltati {skipped}: nessun NavMesh entro la distanza)." : "."));
    }

    /// <summary>
    /// Calcola la posizione finale di un'istanza, applicando la modalità NavMesh.
    /// </summary>
    /// <returns><c>false</c> se in modalità Snap non è stato trovato NavMesh valido.</returns>
    private bool TryResolvePosition(out Vector3 position)
    {
        if (_navMeshMode == ENavMeshSpawnMode.Ignore)
        {
            position = GetRandomPointInArea();
            return true;
        }

        for (int attempt = 0; attempt < _maxAttemptsPerSpawn; attempt++)
        {
            Vector3 candidate = GetRandomPointInArea();
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, _navMeshSampleDistance, NavMesh.AllAreas))
            {
                position = hit.position;
                return true;
            }
        }

        position = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Ritorna un punto casuale nell'area. Sugli assi non selezionati usa il Punto A.
    /// </summary>
    private Vector3 GetRandomPointInArea()
    {
        return new Vector3(
            _spreadX ? Random.Range(Mathf.Min(_pointA.x, _pointB.x), Mathf.Max(_pointA.x, _pointB.x)) : _pointA.x,
            _spreadY ? Random.Range(Mathf.Min(_pointA.y, _pointB.y), Mathf.Max(_pointA.y, _pointB.y)) : _pointA.y,
            _spreadZ ? Random.Range(Mathf.Min(_pointA.z, _pointB.z), Mathf.Max(_pointA.z, _pointB.z)) : _pointA.z);
    }

    private void TrySetFromSelection(ref Vector3 target)
    {
        if (Selection.activeTransform != null)
        {
            target = Selection.activeTransform.position;
            Repaint();
        }
        else
        {
            ShowNotification(new GUIContent("Seleziona un oggetto in scena."));
        }
    }

    /// <summary>
    /// Disegna l'area e gli handle trascinabili dei due punti nella Scene view.
    /// </summary>
    private void OnSceneGUI(SceneView sceneView)
    {
        if (!_drawSceneGizmos)
            return;

        Vector3 min = Vector3.Min(_pointA, _pointB);
        Vector3 max = Vector3.Max(_pointA, _pointB);
        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;

        Handles.color = new Color(0.2f, 0.8f, 1f, 0.9f);
        Handles.DrawWireCube(center, size);

        EditorGUI.BeginChangeCheck();
        Vector3 newA = Handles.PositionHandle(_pointA, Quaternion.identity);
        Vector3 newB = Handles.PositionHandle(_pointB, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            _pointA = newA;
            _pointB = newB;
            Repaint();
        }

        Handles.Label(_pointA, "A");
        Handles.Label(_pointB, "B");
    }
}
