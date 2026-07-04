using UnityEngine;

public class BSPAreaData : MonoBehaviour
{
    [SerializeField] Vector2Int[] BSPArea;
    [SerializeField] int buildBSP_Seed;
    [SerializeField] bool generateJustOnceSeed;

    [Header("Refs")]
    [SerializeField] BSPDirector bspDirector;
    [SerializeField] Transform entrancePoint;
    [SerializeField] Transform pointOfInterest;
    /// <summary>
    /// chiamata quando entra nel trigger dell'area del building
    /// </summary>
    [ContextMenu("Debug_BSPArea")]
    public void GenerateBSPArea()
    {
        if (generateJustOnceSeed)
        {
            buildBSP_Seed = Random.Range(int.MinValue, int.MaxValue);
            generateJustOnceSeed = false;
        }

        Vector3 newPos = transform.position + new Vector3(BSPArea[BSPArea.Length - 1].x, 0, BSPArea[BSPArea.Length - 1].y);
        bspDirector.transform.position = newPos;

        Vector2Int size = GetSize();
        bspDirector.GenerateBSPWithAreaDataa(new BSPSettings
        {
            Width = size.x,
            Height = size.y,
            MinPartitioningWidth = size.x/2,
            MinPartitioningHeight = size.y / 2,
            RoomPadding = 1,
            Seed = buildBSP_Seed,
            RandomSeed = false
        });

        entrancePoint.transform.position = transform.position + new Vector3(BSPArea[3].x, 0, BSPArea[3].y) + new Vector3(bspDirector.firstPoint.x, 0, bspDirector.firstPoint.y);
        pointOfInterest.transform.position = transform.position + new Vector3(BSPArea[3].x, 0, BSPArea[3].y) + new Vector3(bspDirector.randomPoint.x, 0, bspDirector.randomPoint.y);
        //genero il punto di interesse
    }
    /// <summary>
    /// chiamata quando esce dall'area del triggere del building
    /// </summary>
    public void HideArea()
    {
        bspDirector.HideGeneratedArea();
    }

    Vector2Int GetSize()
    {
        if (BSPArea == null || BSPArea.Length == 0)
            return Vector2Int.zero;

        int minX = BSPArea[0].x;
        int maxX = BSPArea[0].x;
        int minY = BSPArea[0].y;
        int maxY = BSPArea[0].y;

        foreach (Vector2Int point in BSPArea)
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
    private void OnDrawGizmosSelected()
    {
        if (BSPArea == null || BSPArea.Length < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < BSPArea.Length; i++)
        {
            Vector3 currentPoint = transform.position + new Vector3(BSPArea[i].x, 0, BSPArea[i].y);
            Vector3 nextPoint = transform.position + new Vector3(BSPArea[(i + 1) % BSPArea.Length].x, 0, BSPArea[(i + 1) % BSPArea.Length].y);

            Gizmos.DrawLine(currentPoint, nextPoint);
        }
    }
}
