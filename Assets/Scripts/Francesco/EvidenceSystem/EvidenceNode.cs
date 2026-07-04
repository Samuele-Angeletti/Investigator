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

    /// <summary>
    /// Set at runtime on cloned nodes: the source asset this node was instantiated from.
    /// Null on authored assets. Lets the system resolve a runtime clone back to the
    /// original template (e.g. to check whether a dialogue's linked evidence is active).
    /// </summary>
    [NonSerialized] public EvidenceNode SourceTemplate;
}
