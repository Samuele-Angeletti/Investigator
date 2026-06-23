using UnityEngine;
using System;
using System.Collections.Generic;

public class CaseGenerator : MonoBehaviour
{
    [SerializeField]
    private List<Suspect> suspects = new();

    [SerializeField]
    private List<string> locations = new();

    public CaseData CurrentCase { get; private set; }
    public event Action<Evidence> OnEvidenceGenerated;
   
    public void GenerateCase(int seed)
    {
        System.Random random = new System.Random(seed);

        CurrentCase = new CaseData();

        CurrentCase.Seed = seed;
        CurrentCase.Victim = "Spider-Man";
        if (suspects.Count == 0)
    {
        Debug.LogError("No suspects assigned!");
        return;
    }
        CurrentCase.Culprit =
            suspects[random.Next(suspects.Count)];

        CurrentCase.CrimeLocation =
            locations[random.Next(locations.Count)];

        GeneratePath(CurrentCase, random);

        GenerateEvidence(CurrentCase, random);
    }
    private void GeneratePath(CaseData data, System.Random random)
    {
        int pathLength = random.Next(3, 6);

        for (int i = 0; i < pathLength; i++)
        {
            string location =
                data.Culprit.RelatedLocations[random.Next(data.Culprit.RelatedLocations.Count)];

            data.CulpritPath.Add(location);
        }
    }
    private void GenerateEvidence(CaseData data, System.Random random)
    {
        data.Evidences.Add(new Evidence
        {
            Id = Guid.NewGuid().ToString(),
            Type = EvidenceType.Physical,
            Description =
                $"Fingerprint matches {data.Culprit.Name}",
            LinkedSuspect =
                data.Culprit.Name,
            LinkedLocation =
                data.CrimeLocation,
            TruthValue = true,
            IsVisible = true
        }       );

        data.Evidences.Add(new Evidence
        {
            Id = Guid.NewGuid().ToString(),
            Type = EvidenceType.Trace,
            Description =
                $"{data.Culprit.UniqueTraits[0]} found at scene",
            LinkedSuspect =
                data.Culprit.Name,
            LinkedLocation =
                data.CrimeLocation,
            TruthValue = true,
            IsVisible = true
        }   );

        data.Evidences.Add(new Evidence
        {
            Id = Guid.NewGuid().ToString(),
            Type = EvidenceType.Witness,
            Description =
                $"Witness saw suspect near {data.Culprit.RelatedLocations[0]}",
            LinkedSuspect =
                data.Culprit.Name,
            LinkedLocation =
                data.Culprit.RelatedLocations[0],
            TruthValue = true,
            IsVisible = true
        }   );
        
    }
}