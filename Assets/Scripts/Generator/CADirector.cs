using UnityEngine;

public class CADirector : MonoBehaviour
{
    [Header("Cellular Automata Settings")]
    [SerializeField] int caWidth = 50;
    [SerializeField] int caHeight = 50;
    [SerializeField] int caInitialWallChance = 45;
    [SerializeField] int caSteps = 5;
    [SerializeField] int caBirthLimit = 4;
    [SerializeField] int caDeathLimit = 3;
    [SerializeField] bool caSolidBorder = true;
    [SerializeField] int caSeed = 45;
    [SerializeField] bool caRandomSeed = false;
}
