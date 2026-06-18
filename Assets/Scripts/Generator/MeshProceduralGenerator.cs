using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshProceduralGenerator : MonoBehaviour
{
    private void AddQuad(Vector2Int pos, float size, Dictionary<Vector3, int> vertexDictionary, List<int> triangles)
    {
        // generate quad

        Vector3 leftBottomPoint = new(pos.x - size / 2, 0, pos.y - size / 2);
        Vector3 upLeftPoint = leftBottomPoint + new Vector3(0, 0, size);
        Vector3 rightBottomPoint = leftBottomPoint + new Vector3(size, 0, 0);
        Vector3 upRightPoint = leftBottomPoint + new Vector3(size, 0, size);

        // check if vertices already exist  in the dictionary, if not add them with their index, then add the triangle to the list

        // left bottom
        if (!vertexDictionary.ContainsKey(leftBottomPoint))
        {
            vertexDictionary.Add(leftBottomPoint, vertexDictionary.Count);
        }

        // up left
        if (!vertexDictionary.ContainsKey(upLeftPoint))
        {
            vertexDictionary.Add(upLeftPoint, vertexDictionary.Count);
        }

        // right bottom
        if (!vertexDictionary.ContainsKey(rightBottomPoint))
        {
            vertexDictionary.Add(rightBottomPoint, vertexDictionary.Count);
        }

        // up right
        if (!vertexDictionary.ContainsKey(upRightPoint))
        {
            vertexDictionary.Add(upRightPoint, vertexDictionary.Count);
        }

        triangles.Add(vertexDictionary[leftBottomPoint]);
        triangles.Add(vertexDictionary[upLeftPoint]);
        triangles.Add(vertexDictionary[rightBottomPoint]);
        triangles.Add(vertexDictionary[upRightPoint]);
        triangles.Add(vertexDictionary[rightBottomPoint]);
        triangles.Add(vertexDictionary[upLeftPoint]);
    }

    public void Generate(GenerationResult generationResult)
    {
        Dictionary<Vector3, int> vertexDictionary = new();
        List<int> triangles = new();
        generationResult.ForEachCell((x, y, value) =>
        {
            switch (value)
            {
                // room
                case 0:
                    AddQuad(new Vector2Int(x, y), 1, vertexDictionary, triangles);
                    break;
                // wall
                case 1:
                    break;
            }
        }
        );

        var mesh = new Mesh
        {
            name = "Procedural Mesh"
        };

        mesh.vertices = vertexDictionary.Keys.ToArray();
        mesh.triangles = triangles.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}