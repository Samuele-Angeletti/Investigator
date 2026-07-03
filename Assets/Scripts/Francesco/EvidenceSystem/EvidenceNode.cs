using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EvidenceSystem/" + nameof(EvidenceNode))]
public class EvidenceNode : ScriptableObject
{
    public int Id;
    [TextArea]
    public string Description;
    public GameObject EvidenceModel;
    public EEvidenceType EvidenceType;
    [Range(0f,1f)]
    public float TruthValue;
    public Suspect LinkedSuspect;
    public List<EvidenceTag> EvidenceTags;
}