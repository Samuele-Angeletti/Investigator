using System;
using System.Collections.Generic;

[Serializable]
public class Suspect
{
    public string Name;

    public List<string> RelatedLocations = new();

    public List<string> UniqueTraits = new();

    public string Fingerprint;
}