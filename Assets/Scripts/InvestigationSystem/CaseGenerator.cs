using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CaseGenerator : MonoBehaviour
{
    [SerializeField]
    private List<Suspect> suspects = new();

    [SerializeField]
    private List<string> locations = new();

    [SerializeField, Tooltip("Minimum/maximum number of clues generated per case (acceptance criteria: at least 3).")]
    private int minClueCount = 3;
    [SerializeField]
    private int maxClueCount = 5;

    private int _incrementalId = 0;

    public CaseData CurrentCase { get; private set; }

    public event Action<EvidenceNode> OnEvidenceGenerated;

    public CaseData GenerateCase(int seed)
    {
        if (suspects.Count == 0)
        {
            Debug.LogError("No suspects assigned!");
            return null;
        }

        if (locations.Count == 0)
        {
            Debug.LogError("No locations assigned!");
            return null;
        }

        System.Random random = new System.Random(seed);

        CurrentCase = new CaseData
        {
            Seed = seed,
            Victim = "Spider-Man"
        };

        CurrentCase.Culprit = suspects[random.Next(suspects.Count)];
        CurrentCase.CrimeLocation = locations[random.Next(locations.Count)];

        GenerateEvidenceSet(CurrentCase, random);
        GeneratePath(CurrentCase, random);

        return CurrentCase;
    }

    private void GenerateEvidenceSet(CaseData data, System.Random random)
    {
        var culpritPool = EvidenceSystem.Instance.Evidences
            .Where(e => e.LinkedSuspect == data.Culprit)
            .ToList();

        if (culpritPool.Count == 0)
        {
            Debug.LogWarning(
                $"No EvidenceNode assets have LinkedSuspect set to '{data.Culprit.Name}'. " +
                "Clues for this case won't point to the culprit until some are linked in the data.");
        }

        int targetCount = random.Next(minClueCount, maxClueCount + 1);

        for (int i = culpritPool.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (culpritPool[i], culpritPool[j]) = (culpritPool[j], culpritPool[i]);
        }

        EvidenceNode lastAdded = null;
        for (int i = 0; i < targetCount && i < culpritPool.Count; i++)
        {
            lastAdded = culpritPool[i];
            AddEvidence(data, lastAdded);
        }

        if (lastAdded != null && data.Evidences.Count < minClueCount)
        {
            var related = EvidenceSystem.Instance.GetPossibleEvidences(lastAdded, random);
            if (related != null)
            {
                foreach (var node in related)
                {
                    if (data.Evidences.Count >= maxClueCount) break;
                    AddEvidence(data, node);
                }
            }
        }
    }

    private void AddEvidence(CaseData data, EvidenceNode sourceNode)
    {
        EvidenceNode runtimeNode = Instantiate(sourceNode);
        runtimeNode.Id = _incrementalId++;

        data.Evidences.Add(runtimeNode);
        EvidenceSystem.Instance.RegisterEvidence(runtimeNode);
        OnEvidenceGenerated?.Invoke(runtimeNode);
    }

    private void GeneratePath(CaseData data, System.Random random)
    {
        int desiredPathLength = random.Next(3, 6);

        data.CulpritPath.Add(data.CrimeLocation);

        var pool = data.Culprit.RelatedLocations
            .Where(loc => !data.CulpritPath.Contains(loc))
            .ToList();

        ShuffleInPlace(pool, random);

        int stepsToTake = Math.Min(desiredPathLength, pool.Count);

        if (stepsToTake < desiredPathLength)
        {
            Debug.LogWarning(
                $"'{data.Culprit.Name}' only has {pool.Count} unique related location(s) available " +
                $"(excluding the crime location); path length reduced from {desiredPathLength} to {stepsToTake} " +
                "to avoid repeating a location.");
        }

        for (int i = 0; i < stepsToTake; i++)
        {
            data.CulpritPath.Add(pool[i]);
        }
    }

    private void ShuffleInPlace(List<string> list, System.Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}