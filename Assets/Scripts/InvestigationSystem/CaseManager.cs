using System;
using UnityEngine;

public class CaseManager : MonoBehaviour
{
    public static CaseManager Instance { get; private set; }
    [SerializeField] private RandomWalkPlaceEvidences _randomWalkPlaceEvidences;
    [SerializeField] private int _steps;
    [SerializeField] private PointOfInterest[] _biases;


    [SerializeField] private CaseGenerator caseGenerator;
    public CaseData CurrentCase { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public CaseData StartNewCase(int seed)
    {
        if (caseGenerator == null)
        {
            Debug.LogError("CaseManager has no CaseGenerator assigned!");
            return null;
        }

        CurrentCase = caseGenerator.GenerateCase(seed);
        _randomWalkPlaceEvidences.Initialize(seed,_steps, 1, Vector2Int.zero, CurrentCase.Evidences.ToArray(), _biases);

        if (CurrentCase == null)
        {
            Debug.LogError("Case generation failed!");
        }
        return CurrentCase;
    }

    public CaseData StartNewCase()
    {
        return StartNewCase(Environment.TickCount);
    }
}