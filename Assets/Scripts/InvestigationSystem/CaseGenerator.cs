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
        int pathLength = random.Next(3, 6);
        var relatedLocations = data.Culprit.RelatedLocations;

        data.CulpritPath.Add(data.CrimeLocation);

        for (int i = 0; i < pathLength; i++)
        {
            if (relatedLocations.Count == 0) break;

            string location = PickNextLocation(relatedLocations, data.CulpritPath[^1], random);
            data.CulpritPath.Add(location);
        }
    }

    private string PickNextLocation(List<string> candidates, string previous, System.Random random)
    {
        if (candidates.Count <= 1)
        {
            return candidates[0];
        }

        string pick;
        do
        {
            pick = candidates[random.Next(candidates.Count)];
        } while (pick == previous);

        return pick;
    }
}