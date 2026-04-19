using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class PlaneGenerator : MonoBehaviour
{
    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    //button

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

    }

    private void Update()
    {
        // when button pressed, call createpoint
        // if button.wasReleasedThisFrame
        // CreatePoint()
    }

    void CreatePoint()
    {
        // make point at controller front position and add to vertices list
    }

    void CreateShape()
    {
        //vertices = new Vector3[]
        //{
        //    //new Vector3(0, 0, 0),
        //    //new Vector3(0, 1, 0),
        //    //new Vector3(1, 0, 0),

        //};

        if (vertices.Length >= 3)
        {
            triangles = new int[]
            {
            0, 1, 2
            };
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}