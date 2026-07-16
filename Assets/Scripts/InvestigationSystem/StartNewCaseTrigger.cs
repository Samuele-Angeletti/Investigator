using UnityEngine;

public class StartNewCaseTrigger : MonoBehaviour
{
    System.Random random = new System.Random();
    [Header("If seed is set to random, manual input is ignored")]
    [SerializeField] bool IsRandom = true;
    [SerializeField] int seed = 1;

    void Start()
    {
        if (IsRandom) seed = random.Next(1, 1111111);

        if (CaseManager.Instance == null)
        {
            Debug.LogError("CaseManager.Instance is null! Ensure a CaseManager exists in the scene.");
            return;
        }

        CaseManager.Instance.StartNewCase(seed);
    }
}