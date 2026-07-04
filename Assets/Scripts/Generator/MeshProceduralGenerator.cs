using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DirectionFaceQuad
{
    X, Y, Z, RevX, RevY, RevZ
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshProceduralGenerator : MonoBehaviour
{
    private void AddQuad(DirectionFaceQuad directionFace, Vector2Int pos, float width, float _height, Dictionary<Vector3, int> vertexDictionary, List<int> triangles)
    {
        // generate quad

        Vector3 leftBottomPoint = Vector3.zero;
        Vector3 upLeftPoint = Vector3.zero;
        Vector3 rightBottomPoint = Vector3.zero;
        Vector3 upRightPoint = Vector3.zero;

        bool reverseFace = false;

        switch (directionFace)
        {
            case DirectionFaceQuad.X:
                leftBottomPoint = new Vector3(pos.x - 0.5f, -width / 2 +0.5f, pos.y - width / 2);
                upLeftPoint = leftBottomPoint + new Vector3(0, _height, 0);
                rightBottomPoint = leftBottomPoint + new Vector3(0, 0, width);
                upRightPoint = leftBottomPoint + new Vector3(0, _height, width);
                break;
            case DirectionFaceQuad.Y:
                leftBottomPoint = new(pos.x - width / 2, 0, pos.y - width / 2);
                upLeftPoint = leftBottomPoint + new Vector3(0, 0, width);
                rightBottomPoint = leftBottomPoint + new Vector3(width, 0, 0);
                upRightPoint = leftBottomPoint + new Vector3(width, 0, width);
                break;
            case DirectionFaceQuad.Z:
                leftBottomPoint = new Vector3(pos.x - width / 2, -width / 2 + 0.5f, pos.y + 0.5f);
                upLeftPoint = leftBottomPoint + new Vector3(0, _height, 0);
                rightBottomPoint = leftBottomPoint + new Vector3(width, 0, 0);
                upRightPoint = leftBottomPoint + new Vector3(width, _height, 0);
                break;
            case DirectionFaceQuad.RevX:
                leftBottomPoint = new Vector3(pos.x + 0.5f, -width / 2 + 0.5f, pos.y - width / 2);
                upLeftPoint = leftBottomPoint + new Vector3(0, _height, 0);
                rightBottomPoint = leftBottomPoint + new Vector3(0, 0, width);
                upRightPoint = leftBottomPoint + new Vector3(0, _height, width);
                reverseFace = true;
                break;
            case DirectionFaceQuad.RevY:
                leftBottomPoint = new(pos.x - width / 2, 0, pos.y - width / 2);
                upLeftPoint = leftBottomPoint + new Vector3(0, 0, width);
                rightBottomPoint = leftBottomPoint + new Vector3(width, 0, 0);
                upRightPoint = leftBottomPoint + new Vector3(width, 0, width);
                reverseFace = true;
                break;
            case DirectionFaceQuad.RevZ:
                leftBottomPoint = new Vector3(pos.x - width / 2, -width / 2 + 0.5f, pos.y - 0.5f);
                upLeftPoint = leftBottomPoint + new Vector3(0, _height, 0);
                rightBottomPoint = leftBottomPoint + new Vector3(width, 0, 0);
                upRightPoint = leftBottomPoint + new Vector3(width, _height, 0);
                reverseFace = true;
                break;
        }

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

        if (!reverseFace)
        {
            triangles.Add(vertexDictionary[leftBottomPoint]);
            triangles.Add(vertexDictionary[upLeftPoint]);
            triangles.Add(vertexDictionary[rightBottomPoint]);

            triangles.Add(vertexDictionary[upRightPoint]);
            triangles.Add(vertexDictionary[rightBottomPoint]);
            triangles.Add(vertexDictionary[upLeftPoint]);
        }
        else
        {
            triangles.Add(vertexDictionary[leftBottomPoint]);
            triangles.Add(vertexDictionary[rightBottomPoint]);
            triangles.Add(vertexDictionary[upLeftPoint]);

            triangles.Add(vertexDictionary[upRightPoint]);
            triangles.Add(vertexDictionary[upLeftPoint]);
            triangles.Add(vertexDictionary[rightBottomPoint]);
        }
    }

    public void Generate(GenerationResult generationResult, bool _isCeiling = false, bool _wall = false, float Height = 1)
    {
        Dictionary<Vector3, int> vertexDictionary = new();
        List<int> triangles = new();
        if (!_wall)
        {
            generationResult.ForEachCell((x, y, value) =>
            {
                switch (value)
                {
                    // room
                    case 0:
                        if(!_isCeiling)
                        AddQuad(DirectionFaceQuad.Y, new Vector2Int(x, y), 1, 1, vertexDictionary, triangles);
                        else
                            AddQuad(DirectionFaceQuad.RevY, new Vector2Int(x, y), 1, 1, vertexDictionary, triangles);
                        break;
                    // wall
                    case 1:
                        break;
                }
            }
        );
        }
        else
        {
            generationResult.ForEachCell((x, y, value) =>
            {
                switch (value)
                {
                    // room
                    case 0:
                        //AddQuad(DirectionFaceQuad.Y, new Vector2Int(x, y), 1, vertexDictionary, triangles);
                        //se sono in room allora controllo accanto a me se ho un wall
                        if (generationResult.IsWall(x, y - 1))
                        {
                            AddQuad(DirectionFaceQuad.RevZ, new Vector2Int(x, y), 1, Height, vertexDictionary, triangles);
                        }
                        if (generationResult.IsWall(x, y + 1))
                        {
                            AddQuad(DirectionFaceQuad.Z, new Vector2Int(x, y), 1, Height, vertexDictionary, triangles);
                        }
                        if (generationResult.IsWall(x - 1, y))
                        {
                            AddQuad(DirectionFaceQuad.X, new Vector2Int(x, y), 1, Height, vertexDictionary, triangles);
                        }
                        if (generationResult.IsWall(x + 1, y))
                        {
                            AddQuad(DirectionFaceQuad.RevX, new Vector2Int(x, y), 1, Height, vertexDictionary, triangles);
                        }
                        break;
                    // wall
                    case 1:
                        break;
                }
            }
        );
        }

        var mesh = new Mesh
        {
            name = "Procedural Mesh"
        };

        mesh.vertices = vertexDictionary.Keys.ToArray();
        mesh.triangles = triangles.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}