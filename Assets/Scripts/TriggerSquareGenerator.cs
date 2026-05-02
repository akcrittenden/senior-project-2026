using Unity.AppUI.UI;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TriggerSquareGenerator : MonoBehaviour
{
    public XRDirectInteractor controller;
    [SerializeField] private InputActionReference triggerAction;
    [SerializeField] private Material squareMaterial;
    [SerializeField] private XRRayInteractor ray;
    [SerializeField] private LineRenderer rayLineRenderer;
    [SerializeField] private float rayLength = 0.5f;

    private Mesh squareMesh;
    private MeshRenderer meshRenderer;
    private Vector3 startPosition;
    private bool isSquareStarted = false;

    void Start()
    {
        squareMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = squareMesh;
        meshRenderer = GetComponent<MeshRenderer>();

        // Apply material if not assigned
        if (squareMaterial == null)
        {
            squareMaterial = new Material(Shader.Find("Standard"));
            squareMaterial.color = Color.cyan;
            squareMaterial.SetFloat("_Metallic", 0.5f);
        }
        meshRenderer.material = squareMaterial;
    }

    void Update()
    {
        if (triggerAction?.action == null)
        {
            Debug.LogWarning("Trigger action not assigned!");
            return;
        }

        // Visualize the ray if LineRenderer is assigned
        if (rayLineRenderer != null)
        {
            UpdateRayVisualization();
        }

        // Detect trigger tap
        if (triggerAction.action.WasPerformedThisFrame() && !ray.IsOverUIGameObject())
        {
            if (!isSquareStarted)
            {
                // First tap - start the square at fixed ray distance
                startPosition = GetRayPointAtDistance();
                isSquareStarted = true;
                Debug.Log("Square started at: " + startPosition);
            }
            else
            {
                // Second tap - finish the square at fixed ray distance
                Vector3 endPosition = GetRayPointAtDistance();
                GenerateSquare(startPosition, endPosition);
                isSquareStarted = false;
                Debug.Log("Square finished at: " + endPosition);
            }
        }

        // Update square preview in real-time while square is being drawn
        if (isSquareStarted)
        {
            Vector3 currentPosition = GetRayPointAtDistance();
            GenerateSquare(startPosition, currentPosition);
        }
    }

    void UpdateRayVisualization()
    {
        Vector3 rayStart = GetRayOrigin();
        Vector3 rayEnd = GetRayPointAtDistance();

        rayLineRenderer.SetPosition(0, rayStart);
        rayLineRenderer.SetPosition(1, rayEnd);
    }

    Vector3 GetRayOrigin()
    {
        Transform rayOrigin = ray.rayOriginTransform ?? ray.transform;
        return rayOrigin.position;
    }

    Vector3 GetRayPointAtDistance()
    {
        Vector3 rayStart = GetRayOrigin();
        Vector3 rayDirection = (ray.rayOriginTransform ?? ray.transform).forward;

        return rayStart + rayDirection * rayLength;
    }

    void GenerateSquare(Vector3 start, Vector3 end)
    {
        Vector3 localStart = transform.InverseTransformPoint(start);
        Vector3 localEnd = transform.InverseTransformPoint(end);

        Vector3 xDir = new Vector3(localEnd.x - localStart.x, 0, 0);
        Vector3 zDir = new Vector3(0, 0, localEnd.z - localStart.z);

        Vector3[] vertices = new Vector3[4]
        {
            localStart,
            localStart + xDir,
            localStart + xDir + zDir,
            localStart + zDir
        };

        int[] triangles = new int[6]
        {
            0, 2, 1,
            0, 3, 2
        };

        squareMesh.Clear();
        squareMesh.vertices = vertices;
        squareMesh.triangles = triangles;
        squareMesh.RecalculateNormals();
        squareMesh.RecalculateBounds();
    }
}