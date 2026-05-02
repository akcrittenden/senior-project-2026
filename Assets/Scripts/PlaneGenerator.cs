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

    List<Vector3> newVertices;
    int[] triangles;
    Vector3[] vertices;
    List<int> trianglesList;
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
        newVertices = new List<Vector3>();
        trianglesList = new List<int>();

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

        Vector3 pointLocation = pointGameObject.transform.position;
        // Create VertexPoint object and add to list
        newVertices.Add(pointLocation);

        Debug.Log($"Point added. Total points: {newVertices.Count}");

        if (newVertices.Count >= 3)
        {
            Debug.Log("At least 3 points created. You can now create a shape.");
            CreateShape();
        }
    }

    void CreateShape()
    {
        // Convert from world space to local space WITHOUT modifying the original list
        Vector3[] localVertices = new Vector3[newVertices.Count];
        for (int i = 0; i < newVertices.Count; i++)
        {
            localVertices[i] = transform.InverseTransformPoint(newVertices[i]);
        }

        // Find closest point to centroid
        int hub = FindClosestPointToCentroid(localVertices);
        Debug.Log($"Using vertex {hub} as hub for triangulation");
        
        // Create triangles - fan triangulation from the hub point
        trianglesList.Clear();
        for (int i = 1; i < newVertices.Count - 1; i++)
        {
            trianglesList.Add(hub);
            trianglesList.Add((hub + i) % newVertices.Count);           // Wrap around with modulo
            trianglesList.Add((hub + i + 1) % newVertices.Count);       // Wrap around with modulo
        }
        triangles = trianglesList.ToArray();

        // Apply to mesh
        mesh.Clear();
        mesh.vertices = localVertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Apply material
        if (meshMaterial != null)
        {
            meshRenderer.material = meshMaterial;
            Debug.Log("Applied custom material to mesh");
        }
        else
        {
            Material newMat = new Material(Shader.Find("Standard"));
            newMat.color = Color.red;
            meshRenderer.material = newMat;
            Debug.Log("Applied debug material to mesh");
        }

        GetComponent<MeshFilter>().mesh = mesh;

        Debug.Log($"Mesh created with {newVertices.Count} vertices and {triangles.Length / 3} triangles");
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Debug.Log($"  Triangle: {triangles[i]}, {triangles[i+1]}, {triangles[i+2]}");
        }
    }

    int FindClosestPointToCentroid(Vector3[] vertices)
    {
        Vector3 centroid = Vector3.zero;
        foreach (var v in vertices)
        {
            centroid += v;
        }
        centroid /= vertices.Length;

        int closestIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], centroid);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        Debug.Log($"Centroid: {centroid}, Closest point index: {closestIndex} at {vertices[closestIndex]}");
        return closestIndex;
    }
}