using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EvidenceSystem/" + nameof(Suspect))]
public class Suspect : ScriptableObject
{
    public string Name;

    public List<string> RelatedLocations = new();

    public List<string> UniqueTraits = new();

    public string Fingerprint;
}
