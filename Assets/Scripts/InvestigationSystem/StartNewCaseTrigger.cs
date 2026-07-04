using UnityEngine;

public class StartNewCaseTrigger : MonoBehaviour
{
    System.Random random = new System.Random();
    [Header("If seed is set to random manul input is null")]
    [SerializeField] bool IsRandom = true;
    [SerializeField] int seed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (IsRandom) seed = random.Next(1,1111111);
        CaseManager.Instance.StartNewCase(seed);
    }

  
}
