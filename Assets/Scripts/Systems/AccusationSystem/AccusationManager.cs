using UnityEngine;
using UnityEngine.UI;

public class AccusationManager : MonoBehaviour
{
    [SerializeField] CaseGenerator caseGenerator;

    [SerializeField] Button[] accusationButton;

    public void ShowButtons ()
    {
        foreach(Button button in accusationButton)
        {
            button.gameObject.SetActive(true);
        }
    }
}
