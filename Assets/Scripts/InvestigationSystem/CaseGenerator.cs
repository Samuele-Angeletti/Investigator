using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CaseGenerator : MonoBehaviour
{
    [SerializeField]
    public List<Suspect> suspects = new();

    [SerializeField]
    private List<string> locations = new();

    [SerializeField, Tooltip("Name of the victim for this case.")]
    private string victimName = "Spider-Man";

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

        if (EvidenceSystem.Instance == null)
        {
            Debug.LogError("EvidenceSystem.Instance is null! Ensure an EvidenceSystem exists in the scene.");
            return null;
        }

        if (EvidenceSystem.Instance.Evidences == null)
        {
            Debug.LogError("EvidenceSystem.Instance.Evidences is null!");
            return null;
        }

        System.Random random = new System.Random(seed);

        CurrentCase = new CaseData
        {
            Seed = seed,
            Victim = victimName
        };

        CurrentCase.Culprit = suspects[random.Next(suspects.Count)];
        CurrentCase.CrimeLocation = locations[random.Next(locations.Count)];

        // Reset ID counter for a fresh case (optional)
        _incrementalId = 0;

        EvidenceSystem.Instance.ResetGeneratedEvidence();

        GenerateEvidenceSet(CurrentCase, random);
        GeneratePath(CurrentCase, random);

        return CurrentCase;
    }

    private void GenerateEvidenceSet(CaseData data, System.Random random)
    {
        var allEvidence = EvidenceSystem.Instance.Evidences;
        if (allEvidence == null || allEvidence.Count == 0)
        {
            Debug.LogError("No evidence assets available in EvidenceSystem!");
            return;
        }

        // Pool of evidence linked to the culprit
        var culpritPool = allEvidence
            .Where(e => e.LinkedSuspect == data.Culprit)
            .ToList();

        // If no culprit-linked evidence exists, fallback to all evidence
        var poolToUse = culpritPool.Count > 0 ? culpritPool : allEvidence;

        int targetCount = Mathf.Clamp(random.Next(minClueCount, maxClueCount + 1), 0, poolToUse.Count);

        // Shuffle the pool
        ShuffleList(poolToUse, random);

        EvidenceNode lastAdded = null;
        for (int i = 0; i < targetCount; i++)
        {
            lastAdded = poolToUse[i];
            AddEvidence(data, lastAdded);
        }

        // If we still haven't reached minClueCount, add related evidence (avoid duplicates)
        if (data.Evidences.Count < minClueCount)
        {
            // Use the last added node as a starting point, or pick a random one if none
            EvidenceNode startNode = lastAdded ?? allEvidence[random.Next(allEvidence.Count)];
            var related = EvidenceSystem.Instance.GetPossibleEvidences(startNode, random);
            if (related != null)
            {
                // Shuffle related to add randomness
                ShuffleList(related, random);
                foreach (var node in related)
                {
                    if (data.Evidences.Count >= maxClueCount) break;
                    // Avoid duplicates
                    if (!data.Evidences.Contains(node))
                        AddEvidence(data, node);
                }
            }
        }

        // Final safeguard: if still not enough, just pick random from all evidence
        if (data.Evidences.Count < minClueCount)
        {
            var fallbackPool = allEvidence.Except(data.Evidences).ToList();
            ShuffleList(fallbackPool, random);
            foreach (var node in fallbackPool)
            {
                if (data.Evidences.Count >= minClueCount) break;
                AddEvidence(data, node);
            }
        }
    }

    private void AddEvidence(CaseData data, EvidenceNode sourceNode)
    {
        if (sourceNode == null) return;

        // Avoid adding duplicates (should be already checked, but double-check)
        if (data.Evidences.Any(e => e.SourceTemplate == sourceNode)) return;

        EvidenceNode runtimeNode = Instantiate(sourceNode);
        runtimeNode.Id = _incrementalId++;
        runtimeNode.SourceTemplate = sourceNode;

        data.Evidences.Add(runtimeNode);
        EvidenceSystem.Instance.RegisterEvidence(runtimeNode);
        OnEvidenceGenerated?.Invoke(runtimeNode);
    }

    private void GeneratePath(CaseData data, System.Random random)
    {
        int desiredPathLength = random.Next(3, 6);

        data.CulpritPath.Add(data.CrimeLocation);

        // Use Distinct to avoid duplicate entries in the path
        var pool = data.Culprit.RelatedLocations
            .Distinct()
            .Where(loc => !data.CulpritPath.Contains(loc))
            .ToList();

        ShuffleList(pool, random);

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

    private void ShuffleList<T>(List<T> list, System.Random random)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}