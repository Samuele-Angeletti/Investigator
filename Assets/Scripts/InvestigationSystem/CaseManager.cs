using UnityEngine;

public class CaseManager : MonoBehaviour
{
    public static CaseManager Instance;
    
    public CaseData CurrentCase;

    private void Awake()
    {
        Instance = this;
    }
    
}