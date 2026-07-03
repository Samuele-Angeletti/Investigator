using System;
using UnityEngine;

public class CaseManager : MonoBehaviour
{
    public static CaseManager Instance { get; private set; }

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
        return CurrentCase;
    }

    public CaseData StartNewCase()
    {
        return StartNewCase(Environment.TickCount);
    }
}
