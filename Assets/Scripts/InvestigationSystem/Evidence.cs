using System;
using UnityEngine;

public enum EvidenceType
{
    Physical,
    Witness,
    Trace,
    Generated
}

[Serializable]
public class Evidence
{
    public string Id;

    public Vector3 Position;

    public EvidenceType Type;

    public string Source;

    public bool IsVisible;

    public bool TruthValue;

    public string Description;

    public string LinkedSuspect;

    public string LinkedLocation;

    public bool IsCollected;
}