using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccusationManager : MonoBehaviour
{
    [SerializeField] Suspect selectedCulprit;
    [SerializeField] Suspect trueCulprit;

    [Header("Refs")]
    [SerializeField] CaseGenerator caseGenerator;

    [SerializeField] Button[] accusationButton;
    [SerializeField] TMP_Text[] accusationName;

    [SerializeField] GameObject confirmButtonsContainer;

    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject restartGamePanel;
    public void ShowButtons ()
    {
        int buttonIndex = 0;
        foreach(Suspect s in caseGenerator.suspects)
        {
            accusationButton[buttonIndex].gameObject.SetActive(true);
            accusationButton[buttonIndex].onClick.AddListener(() => { selectedCulprit = s; });
            accusationName[buttonIndex].text = s.Name;

            confirmButtonsContainer.SetActive(true);
        }
    }
    public void ConfirmButtons()
    {
        CheckIfGuilty();
    }
    public void CancelButton()
    {
        selectedCulprit = null;
        confirmButtonsContainer.SetActive(false);
    }
    void CheckIfGuilty()
    {
        if (selectedCulprit == null)
            return;

        if(selectedCulprit == trueCulprit)
        {
            winPanel.SetActive(true);
            restartGamePanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
            restartGamePanel.SetActive(true);
        }
    }
}
