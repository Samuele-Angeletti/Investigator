using System;
using System.Collections.Generic;
using System.Linq;
using DesignPatterns.Generics;
using UnityEngine;
using Random = System.Random;

public class EvidenceSystem : Singleton<EvidenceSystem>
{
    private List<EvidenceNode> _allGeneratedEvidences = new();
    private HashSet<EvidenceNode> _collectedEvidences = new();
    [SerializeField] private List<EvidenceNode> _evidences = new();

    [Header("Testing")]
    [SerializeField] private EvidenceNode _testNode;
    [SerializeField] private List<EvidenceNode> _foundNodes = new();

    private Dictionary<HashSet<EvidenceTag>, List<EvidenceNode>> _evidencesLookup = new(HashSet<EvidenceTag>.CreateSetComparer());

    private Random _random = new(0);

    public override void Awake()
    {
        // initalize lookup
        foreach (var node in _evidences)
        {
            for (int size = node.EvidenceTags.Count; size >= 1; size--)
            {
                List<HashSet<EvidenceTag>> subgroups = new();
                GenerateSubgroups(node.EvidenceTags, size, 0, new HashSet<EvidenceTag>(), subgroups);

                foreach (var subgroup in subgroups)
                {
                    if (!_evidencesLookup.TryGetValue(subgroup, out var evidencesList))
                    {
                        evidencesList = new();
                        _evidencesLookup[subgroup] = evidencesList;
                    }

                    if (!evidencesList.Contains(node))
                        evidencesList.Add(node);
                }
            }
        }

        _foundNodes = GetPossibleEvidences(_testNode, _random);
    }

    public bool RegisterEvidence(EvidenceNode newEvidenceNode)
    {
        if (_allGeneratedEvidences.Contains(newEvidenceNode)) return false;

        _allGeneratedEvidences.Add(newEvidenceNode);
        return true;
    }

    public void CollectEvidence(EvidenceNode evidenceNode)
    {
        if (_collectedEvidences.Contains(evidenceNode)) return;

        _collectedEvidences.Add(evidenceNode);
        Journal.Instance.AddEvidence(evidenceNode);
    }

    [ContextMenu("TestGetPossibleEvidences")]
    public void GetPossibleEvidences()
    {
        _foundNodes = GetPossibleEvidences(_testNode, _random);
    }

    /// <summary>
    /// Returns the list of the closest matches of the passed evidenceNode, if no match is found it returns a random list (No results contain the passed node)
    /// </summary>
    /// <param name="evidenceNode"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public List<EvidenceNode> GetPossibleEvidences(EvidenceNode evidenceNode, Random random)
    {
        if (_evidencesLookup.Count == 0) return null;

        HashSet<EvidenceTag> evidenceTags = evidenceNode.EvidenceTags.ToHashSet();

        // we have a perfect match, immediately return
        if (_evidencesLookup.TryGetValue(evidenceTags, out var evidencesList))
        {
            var filteredList = new List<EvidenceNode>(evidencesList);
            // remove itself
            if (filteredList.Contains(evidenceNode))
            {
                filteredList.Remove(evidenceNode);
            }

            if (filteredList.Count > 0)
            {
                return filteredList;
            }
        }

        // not a perfect match, we check all subgroups starting from the biggest size Count - 1
        for (int i = evidenceTags.Count - 1; i >= 1; i--)
        {
            List<HashSet<EvidenceTag>> subgroups = new();
            GenerateSubgroups(evidenceNode.EvidenceTags, i, 0, new HashSet<EvidenceTag>(), subgroups);

            foreach (var subgroup in subgroups)
            {
                if (_evidencesLookup.TryGetValue(subgroup, out evidencesList))
                {
                    var filteredList = new List<EvidenceNode>(evidencesList);
                    // remove itself
                    if (filteredList.Contains(evidenceNode))
                    {
                        filteredList.Remove(evidenceNode);
                    }

                    if (filteredList.Count > 0)
                    {
                        return filteredList;
                    }
                }
            }
        }

        // no matches found, return a random one
        return GetRandomList(evidenceNode, random);
    }

    [ContextMenu("TestGetRandomList")]
    public void GetRandomList()
    {
        _foundNodes = GetRandomList(_testNode, _random);
    }

    /// <summary>
    /// Returns a List of Evidence nodes from the dictionary which doesn't contain the given node
    /// </summary>
    /// <param name="evidenceNode"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public List<EvidenceNode> GetRandomList(EvidenceNode evidenceNode, Random random)
    {
        if (_evidencesLookup.Count == 0) return null;

        HashSet<EvidenceTag> evidenceTags = evidenceNode.EvidenceTags.ToHashSet();

        // find all keys where there are at least 2 values or that don't contain evidenceNode
        var validKeys = _evidencesLookup
            .Where(kvp => kvp.Value.Count > 1 || !kvp.Value.Contains(evidenceNode))
            .Select(kvp => kvp.Key)
            .ToList();

        if (validKeys.Count == 0)
        {
            return null;
        }

        HashSet<EvidenceTag> randomKey = validKeys[random.Next(0, validKeys.Count)];

        var randomList = new List<EvidenceNode>(_evidencesLookup[randomKey]);
        randomList.Remove(evidenceNode);

        return randomList;
    }

    /// <summary>
    /// Given a targetSize it returns all of possible subgroups
    /// </summary>
    /// <param name="original">The original list</param>
    /// <param name="targetSize">The target size of each subgroup</param>
    /// <param name="startIndex">The start index</param>
    /// <param name="current">The current added nodes</param>
    /// <param name="results">The list holding the found subgroups</param>
    private void GenerateSubgroups(List<EvidenceTag> original, int targetSize, int startIndex, HashSet<EvidenceTag> current, List<HashSet<EvidenceTag>> results)
    {
        if (current.Count == targetSize)
        {
            results.Add(new HashSet<EvidenceTag>(current));
            return;
        }

        for (int i = startIndex; i < original.Count; i++)
        {
            current.Add(original[i]);
            GenerateSubgroups(original, targetSize, i + 1, current, results);
            current.Remove(original[i]);
        }
    }
}