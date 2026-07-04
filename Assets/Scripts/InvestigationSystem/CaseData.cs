using System;
using System.Collections.Generic;

[Serializable]
public class CaseData
{
    public int Seed;

    public string Victim;

    public Suspect Culprit;

    public string CrimeLocation;

    public List<string> CulpritPath = new();

    public List<EvidenceNode> Evidences = new();
}