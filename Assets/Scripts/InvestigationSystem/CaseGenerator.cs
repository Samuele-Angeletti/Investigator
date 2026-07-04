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

    [SerializeField, Tooltip("Add at least 3 Clues and a Max of 5")]
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
            Victim = "L'uomo Ragno"
        };

        CurrentCase.Culprit = suspects[random.Next(suspects.Count)];
        CurrentCase.CrimeLocation = locations[random.Next(locations.Count)];

        // Clear the previous case's evidence so "in system" checks reflect only this case.
        EvidenceSystem.Instance.ResetGeneratedEvidence();

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
        // Keep a link to the source asset so dialogues (which reference the asset) can be matched.
        runtimeNode.SourceTemplate = sourceNode;

        data.Evidences.Add(runtimeNode);
        EvidenceSystem.Instance.RegisterEvidence(runtimeNode);
        OnEvidenceGenerated?.Invoke(runtimeNode);
    }

    private void GeneratePath(CaseData data, System.Random random)
    {
        int pathLength = random.Next(3, 6);

        data.CulpritPath.Add(data.CrimeLocation);

        for (int i = 0; i < pathLength; i++)
        {
            if (data.Culprit.RelatedLocations.Count == 0) break;

            string location =
                data.Culprit.RelatedLocations[random.Next(data.Culprit.RelatedLocations.Count)];

            data.CulpritPath.Add(location);
        }
    }
}
