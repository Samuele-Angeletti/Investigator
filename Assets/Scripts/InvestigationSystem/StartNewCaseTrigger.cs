using UnityEngine;

public class StartNewCaseTrigger : MonoBehaviour
{
    int seed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CaseManager.Instance.StartNewCase(seed);
    }

    
}
