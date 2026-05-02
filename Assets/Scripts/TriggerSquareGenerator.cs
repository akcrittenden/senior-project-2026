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

        // Detect trigger tap
        if (triggerAction.action.WasPerformedThisFrame())
        {
            if (!isSquareStarted && !ray.IsOverUIGameObject())
            {
                // First tap - start the square
                startPosition = controller.transform.position;
                isSquareStarted = true;
                Debug.Log("Square started at: " + startPosition);
            }
            else if (!ray.IsOverUIGameObject())
            {
                // Second tap - finish the square
                Vector3 endPosition = controller.transform.position;
                GenerateSquare(startPosition, endPosition);
                isSquareStarted = false;
                Debug.Log("Square finished at: " + endPosition);
            }
        }

        // Update square preview in real-time while square is being drawn
        if (isSquareStarted)
        {
            Vector3 currentPosition = controller.transform.position;
            GenerateSquare(startPosition, currentPosition);
        }
    }

    void GenerateSquare(Vector3 start, Vector3 end)
    {
        // Convert to local space
        Vector3 localStart = transform.InverseTransformPoint(start);
        Vector3 localEnd = transform.InverseTransformPoint(end);

        // Calculate the two vectors along each axis
        Vector3 xDir = new Vector3(localEnd.x - localStart.x, 0, 0);
        Vector3 zDir = new Vector3(0, 0, localEnd.z - localStart.z);

        // Define the four corners of the square
        Vector3[] vertices = new Vector3[4]
        {
            localStart,                          // Bottom-left (0,0)
            localStart + xDir,                   // Bottom-right (1,0)
            localStart + xDir + zDir,            // Top-right (1,1)
            localStart + zDir                    // Top-left (0,1)
        };

        // Define triangles (two triangles to form a square)
        int[] triangles = new int[6]
        {
            0, 2, 1,  // First triangle
            0, 3, 2   // Second triangle
        };

        // Apply to mesh
        squareMesh.Clear();
        squareMesh.vertices = vertices;
        squareMesh.triangles = triangles;
        squareMesh.RecalculateNormals();
        squareMesh.RecalculateBounds();
    }
}