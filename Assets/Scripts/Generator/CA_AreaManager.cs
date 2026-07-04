using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CA_AreaManager : MonoBehaviour
{
    [SerializeField] Vector2Int[] caPArea;
    [SerializeField] float repeatRate = 5f;
    Vector2Int size;
    [Space]
    [SerializeField] bool showQuadGizmos = true;

    GenerationResult generationResult;
    HashSet<Vector2Int> infoPoint;

    [Header("Refs")]
    [SerializeField] CADirector caDirector;
    [SerializeField] Transform NpcsContainer;
    [SerializeField] NpcHandler[] npcs;

    private void Start()
    {
        Initialize();
        StartCoroutine(StartCheckingArea());
    }
    void Initialize()
    {
        size = GetSize();
        PopulateNpcs();
    }
    [ContextMenu("Debug_BSP")]
    public void GenerateCAArea()
    {
        //Vector3 newPos = transform.position + new Vector3(caPArea[caPArea.Length - 1].x, 0, caPArea[caPArea.Length - 1].y);
        //transform.position = newPos;

        size = GetSize();
        caDirector.GenerateBSPDebug(size);
        generationResult = caDirector.GetGenerationResult();
    }
    Vector2Int GetSize()
    {
        if (caPArea == null || caPArea.Length == 0)
            return Vector2Int.zero;

        int minX = caPArea[0].x;
        int maxX = caPArea[0].x;
        int minY = caPArea[0].y;
        int maxY = caPArea[0].y;

        foreach (Vector2Int point in caPArea)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);

            minY = Mathf.Min(minY, point.y);
            maxY = Mathf.Max(maxY, point.y);
        }

        return new Vector2Int(
            maxX - minX,
            maxY - minY
        );
    }
    IEnumerator StartCheckingArea()
    {
        while (true)
        {
            caDirector.GenerateBSPDebug(size);
            generationResult = caDirector.GetGenerationResult();

            CheckArea();
            CheckNPCs();

            yield return new WaitForSeconds(repeatRate);
        }
    }
    void PopulateNpcs()
    {
        npcs = NpcsContainer.GetComponentsInChildren<NpcHandler>();
    }
    /// <summary>
    /// controllo l'area e mi salvo i punti di interesse in un hashset
    /// </summary>
    void CheckArea()
    {
        infoPoint = new HashSet<Vector2Int>();
        generationResult.ForEachCell((x, y, value) =>
        {
            Vector3 newPos = transform.position + new Vector3(caPArea[caPArea.Length - 1].x, 0, caPArea[caPArea.Length - 1].y);
            Vector2Int newPosXZ = new Vector2Int(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.z));
            switch (value)
            {
                // room
                case 0:
                    infoPoint.Add(newPosXZ + new Vector2Int(x, y));
                    break;
                // wall
                case 1:
                    break;
            }
        });
    }
    /// <summary>
    /// fa un foreach di ogni NPC presente dalla lista e controllo se il punto più vicino è all'interno dell'hashset o no, 
    /// se si allora lo faccio entrare in modalità di ricerca del punto di interesse, altrimenti lo faccio tornare alla sua routine normale
    /// </summary>
    void CheckNPCs()
    {
        foreach (NpcHandler npc in npcs)
        {
            Vector2Int npcPos = new Vector2Int(Mathf.RoundToInt(npc.transform.position.x), Mathf.RoundToInt(npc.transform.position.z));
            if (infoPoint.Contains(npcPos))
            {
                //il npc diventa stato informato
                npc.SetInfoState(ECharacterInfoState.Informed);
            }
            else
            {
                //il npc diventa stato ignaro
                npc.SetInfoState(ECharacterInfoState.Unaware);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (caPArea == null || caPArea.Length < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < caPArea.Length; i++)
        {
            Vector3 currentPoint = transform.position + new Vector3(caPArea[i].x, 0, caPArea[i].y);
            Vector3 nextPoint = transform.position + new Vector3(caPArea[(i + 1) % caPArea.Length].x, 0, caPArea[(i + 1) % caPArea.Length].y);

            Gizmos.DrawLine(currentPoint, nextPoint);
        }

        if (showQuadGizmos && generationResult != null)
        {
            Vector3 newPos = transform.position + new Vector3(caPArea[caPArea.Length - 1].x, 0, caPArea[caPArea.Length - 1].y);
            generationResult.ForEachCell((x, y, value) =>
            {
                switch (value)
                {
                    // room
                    case 0:
                        Gizmos.color = Color.green;
                        break;
                    // wall
                    case 1:
                        Gizmos.color = Color.blue;
                        break;
                }
                Gizmos.DrawWireCube(new Vector3(x + 0.5f, 0, y + 0.5f) + newPos, new Vector3(1, 0, 1));
            });
        }
    }
}
