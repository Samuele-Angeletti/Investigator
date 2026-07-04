using System.Collections.Generic;
using DesignPatterns.Generics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Journal : Singleton<Journal>
{
    [Header("UI References")]
    [SerializeField] private Transform _parentSuspects;
    [SerializeField] private SuspectRow _suspectRowPrefab;
    [SerializeField] private Transform _parentEvidences;
    [SerializeField] private EvidenceRow _evidenceRowPrefab;
    [SerializeField] private EvidenceDetailsUI _evidenceDetails;
    private List<EvidenceNode> _collectedEvidences;
    //TODO: change GameObject with actual suspect class
    private Dictionary<GameObject, List<EvidenceRow>> _suspectsEvidences;
    private Dictionary<EvidenceRow, EvidenceNode> _evidenceRowsNodes;
    private Dictionary<SuspectRow, GameObject> _suspectRowSuspects;

    private SuspectRow _currentSuspectRow;

    public override void Awake()
    {
        _collectedEvidences = new();
    }

    public void AddEvidence(EvidenceNode newEvidence)
    {
        if (newEvidence == null) return;
        if (_collectedEvidences == null) _collectedEvidences = new();
        if (_collectedEvidences.Contains(newEvidence)) return;

        _collectedEvidences.Add(newEvidence);
        RefreshUI();
        // if (_suspectsEvidences.TryGetValue(newEvidence.LinkedSuspect, out var listEvidences))
        // {
        //     listEvidences.Add(newEvidence);
        // }
        // else
        // {
        //     listEvidences = new()
        //     {
        //         newEvidence
        //     };
        //     _suspectsEvidences[newEvidence.LinkedSuspect] = listEvidences;
        // }
    }

    public void RefreshUI()
    {
        _evidenceRowsNodes = new();
        _suspectRowSuspects = new();
        _suspectsEvidences = new();

        var suspectsRows = _parentSuspects.GetComponentsInChildren<SuspectRow>();
        for (int i = suspectsRows.Length - 1; i >= 0; i--)
        {
            Destroy(suspectsRows[i].gameObject);
        }
        var evidencesRows = _parentEvidences.GetComponentsInChildren<EvidenceRow>();
        for (int i = evidencesRows.Length - 1; i >= 0; i--)
        {
            Destroy(evidencesRows[i].gameObject);
        }

        _evidenceDetails.SetUp(null);

        for (int i = 0; i < _collectedEvidences.Count; i++)
        {
            EvidenceNode evidenceNode = _collectedEvidences[i];

            // spawn and setup evidence row
            EvidenceRow evidenceRow = Instantiate(_evidenceRowPrefab, _parentEvidences);
            evidenceRow.SetUp($"#{i + 1} {evidenceNode.name}");
            _evidenceRowsNodes[evidenceRow] = evidenceNode;
            evidenceRow.OnClicked += DisplayEvidenceDetails;

            // add evidence to suspect
            if (evidenceNode.LinkedSuspect != null)
            {
                //if (!_suspectsEvidences.ContainsKey(evidenceNode.LinkedSuspect))
                //{
                //    _suspectsEvidences[evidenceNode.LinkedSuspect] = new();
                //}
                //_suspectsEvidences[evidenceNode.LinkedSuspect].Add(evidenceRow);
            }
        }

        foreach (var item in _suspectsEvidences)
        {
            SuspectRow suspectRow = Instantiate(_suspectRowPrefab, _parentSuspects);
            //TODO: add the sprite from the actual suspect clas
            suspectRow.SetUp(null, item.Key.name);
            _suspectRowSuspects[suspectRow] = item.Key;
            suspectRow.OnClicked += DisplayEvidences;
        }
    }

    private void DisplayEvidenceDetails(EvidenceRow evidenceRow)
    {
        if (evidenceRow == null)
        {
            _evidenceDetails.SetUp(null);
            return;
        }

        _evidenceDetails.SetUp(_evidenceRowsNodes[evidenceRow]);
    }

    private void DisplayEvidences(SuspectRow suspectRow)
    {
        // if clicked same suspect remove focus
        if (_currentSuspectRow == suspectRow)
        {
            EventSystem.current.SetSelectedGameObject(null);
            _currentSuspectRow = null;
            foreach (var item in _evidenceRowsNodes)
            {
                item.Key.gameObject.SetActive(true);
            }
            return;
        }

        _currentSuspectRow = suspectRow;
        DisplayEvidenceDetails(null);

        foreach (var item in _evidenceRowsNodes)
        {
            item.Key.gameObject.SetActive(false);
        }

        foreach (var item in _suspectsEvidences[_suspectRowSuspects[suspectRow]])
        {
            item.gameObject.SetActive(true);
        }
    }
}
