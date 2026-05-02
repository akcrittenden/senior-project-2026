using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(XRController))]
public class PlaneGenerator : MonoBehaviour
{
    public XRDirectInteractor controller;
    public XRRayInteractor ray;
    protected Vector3 controllerPosition;
    protected Quaternion controllerRotation;

    List<VertexPoint> vertices; // Changed from GameObject to VertexPoint
    int[] triangles;
    Mesh mesh; 

    [SerializeField] private InputActionReference triggerAction;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private LineRenderer measurementLine;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new List<VertexPoint>();

    }
    private void Update()
    {
        if (triggerAction?.action == null)
        {
            Debug.LogWarning("Trigger action not assigned!");
            return;
        }

        // Check if performed this frame
        if (triggerAction.action.WasPerformedThisFrame())
        {
            // check if ray is over the UI on the wrist, if not, create point
            if (!ray.IsOverUIGameObject())
            {
                Debug.Log("Trigger pressed! Getting position...");
                CreatePoint();
            }
        }
    }


    void CreatePoint()
    {
        // get controller position and rotation
        controllerPosition = controller.transform.position;
        controllerRotation = controller.transform.rotation;

        // instantiate point prefab at controller position and rotation
        GameObject pointGameObject = Instantiate(pointPrefab, controllerPosition, controllerRotation);
        Debug.Log("Sphere created at: " + controllerPosition);

        // Create VertexPoint object and add to list
        VertexPoint newPoint = new VertexPoint(pointGameObject, vertices.Count);
        vertices.Add(newPoint);

        Debug.Log($"Point {newPoint.index} added. Total points: {vertices.Count}");

        if (vertices.Count >= 3)
        {
            Debug.Log("At least 3 points created. You can now create a shape.");
            CreateShape();
        }
    }

    void CreateShape()
    {
        Vector3[] meshVertices = new Vector3[vertices.Count];
        for (int i = 0; i < vertices.Count; i++ )
        {
            meshVertices[i] = vertices[i].position;
        }
        //create triangle
        triangles = new int[] {
            0
        };

        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Debug.Log("mesh was created with vertices: " + meshVertices.Length + " and triangles: " + triangles.Length);
    }
}