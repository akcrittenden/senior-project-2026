using Unity.AppUI.UI;
using Unity.Multiplayer.Center.Common;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(XRGrabInteractable))]
public class TriggerSquareGenerator : MonoBehaviour
{
    public XRDirectInteractor controller;
    [SerializeField] private InputActionReference triggerAction;
    [SerializeField] private Material squareMaterial;
    [SerializeField] private XRRayInteractor ray;
    [SerializeField] private LineRenderer rayLineRenderer;
    [SerializeField] private float rayLength = 0.3f;

    private Mesh squareMesh;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    [SerializeField] private float cubeHeight = 0.02f;
    private Vector3 startPosition;
    private Vector3 planeEndPosition;
    private bool isPlaneFinalized = false;
   // private bool isCubeMode = false;
    private bool triggerWasPressed = false;
    private Rigidbody rb;

    void Start()
    {
        squareMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = squareMesh;
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();


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

        if (rayLineRenderer != null)
        {
            UpdateRayVisualization();
        }

        bool triggerCurrentlyPressed = triggerAction.action.IsPressed();
        bool triggerJustPressed = triggerCurrentlyPressed && !triggerWasPressed;
        bool triggerJustReleased = !triggerCurrentlyPressed && triggerWasPressed;
        triggerWasPressed = triggerCurrentlyPressed;

        // Drawing plane: click and drag
        //if (!isPlaneFinalized && !isCubeMode)
        if (!isPlaneFinalized)
        {
            if (triggerJustPressed && !ray.IsOverUIGameObject())
            {
                // Start plane
                startPosition = GetRayPointAtDistance();
                Debug.Log("Plane started at: " + startPosition);
            }

            if (triggerCurrentlyPressed)
            {
                // Drag to preview plane
                Vector3 currentPosition = GetRayPointAtDistance();
                GeneratePreview(startPosition, currentPosition);
            }

            if (triggerJustReleased)
            {
                // Release to finalize plane
                planeEndPosition = GetRayPointAtDistance();
                isPlaneFinalized = true; 
                Debug.Log("Plane finished at: " + planeEndPosition);
                GenerateFrame(startPosition, planeEndPosition, cubeHeight);
                SpawnCube(squareMesh);
                squareMesh.Clear(); // Clear the preview mesh after spawning the cube
                isPlaneFinalized = false; // reset plane mode after generating cube
            }
            //here add pre-programmed depth to plane so it mimicks a picture frame, maybe like 0.02f?

        }
        //// Drawing cube: drag vertically and release to finish
        //else if (isCubeMode && isPlaneFinalized)
        //{
        //    // Always show cube preview based on current ray position
        //    Vector3 currentPosition = GetRayPointAtDistance();
        //    float height = currentPosition.y - planeEndPosition.y;
        //    GenerateCube(startPosition, planeEndPosition, height);

        //    // Release to finalize cube
        //    if (triggerJustReleased)
        //    {
        //        isCubeMode = false;
        //        isPlaneFinalized = false;
        //        UpdateCollider();
        //        Debug.Log("Cube finished");
        //    }
        //}
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
        return rayStart + rayDirection;
    }

    void GeneratePreview(Vector3 start, Vector3 end)
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

    void GenerateFrame(Vector3 start, Vector3 end, float height)
    {
        Vector3 localStart = transform.InverseTransformPoint(start);
        Vector3 localEnd = transform.InverseTransformPoint(end);

        Vector3 xDir = new Vector3(localEnd.x - localStart.x, 0, 0);
        Vector3 zDir = new Vector3(0, 0, localEnd.z - localStart.z);
        Vector3 yDir = new Vector3(0, height, 0);

        Vector3[] vertices = new Vector3[8]
        {
            localStart,
            localStart + xDir,
            localStart + xDir + zDir,
            localStart + zDir,
            localStart + yDir,
            localStart + xDir + yDir,
            localStart + xDir + zDir + yDir,
            localStart + zDir + yDir
        };

        int[] triangles = new int[36]
        {
            0, 2, 1,
            0, 3, 2,
            4, 5, 6,
            4, 6, 7,
            0, 1, 5,
            0, 5, 4,
            2, 3, 7,
            2, 7, 6,
            0, 4, 7,
            0, 7, 3,
            1, 2, 6,
            1, 6, 5
        };

        squareMesh.Clear();
        squareMesh.vertices = vertices;
        squareMesh.triangles = triangles;
        squareMesh.RecalculateNormals();
        squareMesh.RecalculateBounds();
    }

    void SpawnCube(Mesh cubeMesh)
    {
        // Create a new GameObject for this cube
        GameObject newCube = new GameObject("GeneratedCube");
        newCube.transform.position = transform.position;
        newCube.transform.rotation = transform.rotation;

        // Add mesh components
        MeshFilter meshFilter = newCube.AddComponent<MeshFilter>();
        meshFilter.mesh = Instantiate(cubeMesh);

        MeshRenderer meshRenderer = newCube.AddComponent<MeshRenderer>();
        meshRenderer.material = squareMaterial;

        // Add collider and rigidbody
        BoxCollider boxCollider = newCube.AddComponent<BoxCollider>();
        Rigidbody newRb = newCube.AddComponent<Rigidbody>();
        newRb.useGravity = true;

        // Add XRGrabInteractable
        newCube.AddComponent<XRGrabInteractable>();

        // Fit collider to mesh
        boxCollider.size = cubeMesh.bounds.size;
        boxCollider.center = cubeMesh.bounds.center;
    }

    void UpdateCollider()
    {
        if (boxCollider != null && squareMesh.vertices.Length > 0)
        {
            // Fit the collider to the current mesh bounds
            boxCollider.size = squareMesh.bounds.size;
            boxCollider.center = squareMesh.bounds.center;
            squareMesh.AddComponent<BoxCollider>();
            rb.useGravity = true;
            squareMesh.AddComponent<XRGrabInteractable>(); // add to script object, already applied preset

        }
    }
}