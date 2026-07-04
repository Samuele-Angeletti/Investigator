using System.Collections;
using UnityEngine;

public class CA_AreaManager : MonoBehaviour
{
    [SerializeField] Vector2Int[] caPArea;
    [SerializeField] float repeatRate = 5f;
    Vector2Int size;

    [SerializeField] bool showQuadGizmos = true;

    [Header("Refs")]
    [SerializeField] CADirector caDirector;

    private void Start()
    {
        size = GetSize();
        StartCoroutine(StartCheckingArea());
    }

    [ContextMenu("Debug_BSPArea")]
    public void GenerateBSPArea()
    {

    }
    Vector2Int GetSize()
    {
        if (caPArea == null || caPArea.Length == 0)
            return Vector2Int.zero;

        int minX = caPArea[0].x;
        int maxX = caPArea[0].x;
        int minY = caPArea[0].y;
        int maxY = caPArea[0].y;

        foreach (Vector2Int point in caPArea)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);

            minY = Mathf.Min(minY, point.y);
            maxY = Mathf.Max(maxY, point.y);
        }

        return new Vector2Int(
            maxX - minX,
            maxY - minY
        );
    }
    IEnumerator StartCheckingArea()
    {
               while (true)
        {
            caDirector.GenerateBSPDebug(size);
            //caDirector.CheckIfPlayerInArea(caPArea);
            yield return new WaitForSeconds(repeatRate);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (caPArea == null || caPArea.Length < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < caPArea.Length; i++)
        {
            Vector3 currentPoint = transform.position + new Vector3(caPArea[i].x, 0, caPArea[i].y);
            Vector3 nextPoint = transform.position + new Vector3(caPArea[(i + 1) % caPArea.Length].x, 0, caPArea[(i + 1) % caPArea.Length].y);

            Gizmos.DrawLine(currentPoint, nextPoint);
        }

        
    }
}
