using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(XRGrabInteractable))]
public class TriggerSquareGenerator : MonoBehaviour
{
    [SerializeField] private GameObject framePrefab;
    [SerializeField] private InputActionReference triggerAction;
    [SerializeField] private Material previewMaterial;
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


        if (previewMaterial == null)
        {
            previewMaterial = new Material(Shader.Find("Standard"));
            previewMaterial.color = Color.cyan;
            previewMaterial.SetFloat("_Metallic", 0.5f);
        }

        meshRenderer.material = previewMaterial;
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
            //here add pre-programmed depth to plane so it mimics a picture frame, maybe like 0.02f?

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
        Vector3 fixedEnd = GetRayPointAtDistance(); // fixed length
        Vector3 rayEnd = fixedEnd;

        // UI hit takes priority
        if (ray.IsOverUIGameObject() && ray.TryGetCurrentUIRaycastResult(out RaycastResult uiHit))
        {
            rayEnd = uiHit.worldPosition;
        }
        // Otherwise only snap if it's an XR interactable
        else if (ray.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider.GetComponentInParent<XRBaseInteractable>() != null)
                rayEnd = hit.point;
        }

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
        return rayStart + (rayDirection * rayLength);
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
        GameObject newCube = new GameObject("User Generated Frame");
        newCube.layer = gameObject.layer;
        newCube.tag = "Frame"; 

        // Instantiate and recenter the mesh so the pivot is at its center
        Mesh newMesh = Instantiate(cubeMesh);
        Vector3 meshCenter = newMesh.bounds.center;

        // Shift all vertices so the mesh is centered at local origin
        Vector3[] vertices = newMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= meshCenter;
        }
        newMesh.vertices = vertices;
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        // Position the object at the world-space center of where the mesh was
        newCube.transform.rotation = transform.rotation;
        newCube.transform.position = transform.TransformPoint(meshCenter);

        // Add mesh components
        MeshFilter meshFilter = newCube.AddComponent<MeshFilter>();
        meshFilter.mesh = newMesh;

        MeshRenderer newMeshRenderer = newCube.AddComponent<MeshRenderer>();
        newMeshRenderer.material = squareMaterial;

        // Add rigidbody first (XRGrabInteractable requires it)
        Rigidbody newRb = newCube.AddComponent<Rigidbody>();
        newRb.useGravity = false;
        newRb.isKinematic = true; // make finished frame not fall to ground

        // Add collider and fit to recentered mesh
        BoxCollider newBoxCollider = newCube.AddComponent<BoxCollider>();
        newBoxCollider.size = newMesh.bounds.size;
        newBoxCollider.center = newMesh.bounds.center;

        // Add XRGrabInteractable and match interaction layers to this object's interactable
        XRGrabInteractable grabInteractable = newCube.AddComponent<XRGrabInteractable>();
        XRGrabInteractable sourceInteractable = GetComponent<XRGrabInteractable>();
        if (sourceInteractable != null)
        {
            grabInteractable.interactionLayers = sourceInteractable.interactionLayers;
            grabInteractable.selectMode = InteractableSelectMode.Multiple;
        }

        // Fixed attach point on the larger face, rotated 90 degrees
        GameObject attachPoint = new GameObject("Attach Point");
        attachPoint.transform.SetParent(newCube.transform, false);
        attachPoint.transform.localPosition = new Vector3(0f, newMesh.bounds.extents.y, 0f);
        attachPoint.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        XRGeneralGrabTransformer grabTransformer = newCube.AddComponent<XRGeneralGrabTransformer>();
        XRGeneralGrabTransformer sourceTransformer = GetComponent<XRGeneralGrabTransformer>();
        if (sourceTransformer != null)
        {
            {
                grabTransformer.allowTwoHandedRotation = sourceTransformer.allowTwoHandedRotation;
            }
        }
            grabInteractable.attachTransform = attachPoint.transform;
        // TODO: can't rotate with two hands for some reason
        // TODO: make point the user started with the UP direction and make sure that always faces up

        newCube.AddComponent<SnapFramesToWall>();
        Debug.Log("Added snap frames script to component I THINK I SHOULD COME FIRST");


    }


}