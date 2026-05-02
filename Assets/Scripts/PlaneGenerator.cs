using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
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
    [SerializeField] private Material meshMaterial;

    private MeshRenderer meshRenderer;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>(); // Get the mesh renderer
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

        if (vertices.Count < 3)
        {
            Debug.LogWarning("Need at least 3 points to create a mesh");
            return;
        } else {
            Debug.Log("At least 3 points created. You can now create a shape.");
            CreateShape();
        }
    }

    void CreateShape()
    {
        // Convert List<VertexPoint> to Vector3[] array
        // IMPORTANT: Convert from world space to local space
        Vector3[] meshVertices = new Vector3[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            // Convert world position to local position relative to this GameObject
            meshVertices[i] = transform.InverseTransformPoint(vertices[i].position);
        }

        // Create triangles - fan triangulation from first point
        List<int> trianglesList = new List<int>();
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            trianglesList.Add(0);
            trianglesList.Add(i);
            trianglesList.Add(i + 1);
        }
        triangles = trianglesList.ToArray();

        // Apply to mesh
        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Apply material to mesh
        if (meshMaterial != null)
        {
            meshRenderer.material = meshMaterial;
            Debug.Log("applied custom material to mesh");
        } else
        {
            Material newMat = new Material(Shader.Find("Standard"));
            newMat.color = Color.red;
            meshRenderer.material = newMat;
            Debug.Log("applied debug material to mesh");
        }

        Debug.Log($"Mesh created with {meshVertices.Length} vertices and {triangles.Length / 3} triangles");
    }
}