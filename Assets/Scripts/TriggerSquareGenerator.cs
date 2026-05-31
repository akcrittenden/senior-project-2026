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
    [SerializeField] private Material previewMaterial;
    [SerializeField] private Material squareMaterial;
    [SerializeField] private LineRenderer rayLineRenderer;
    [SerializeField] private Transform leftControllerPoint;
    [SerializeField] private Transform rightControllerPoint;

    [SerializeField] private InputActionReference leftTapeAction;
    [SerializeField] private InputActionReference rightTapeAction;

    [SerializeField] private float minimumFrameArea = 0.0009f;

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

    private ActiveController activeController = ActiveController.None;

    private enum ActiveController
    {
        None,
        Left,
        Right
    }

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
        //if (rayLineRenderer != null)
        //{
        //    UpdateRayVisualization();
        //}

        // Decide which controller started the interaction
        if (activeController == ActiveController.None)
        {
            if (WasPressedThisFrame(leftTapeAction))
            {
                activeController = ActiveController.Left;
                HandleDownAction(leftControllerPoint);
            }
            else if (WasPressedThisFrame(rightTapeAction))
            {
                activeController = ActiveController.Right;
                HandleDownAction(rightControllerPoint);
            }

            return;
        }

        // LEFT HAND
        if (activeController == ActiveController.Left)
        {
            if (IsPressed(leftTapeAction))
            {
                HandleHoldAction(leftControllerPoint);
            }
            else if (WasReleasedThisFrame(leftTapeAction))
            {
                HandleUpAction(leftControllerPoint);
                activeController = ActiveController.None;
            }

            return;
        }

        // RIGHT HAND
        if (IsPressed(rightTapeAction))
        {
            HandleHoldAction(rightControllerPoint);
        }
        else if (WasReleasedThisFrame(rightTapeAction))
        {
            HandleUpAction(rightControllerPoint);
            activeController = ActiveController.None;
        }
    }

    //void UpdateRayVisualization()
    //{
    //    Transform activeTransform = activeController switch
    //    {
    //        ActiveController.Left => leftControllerPoint,
    //        ActiveController.Right => rightControllerPoint,
    //        _ => rightControllerPoint // idle fallback
    //    };

    //    if (activeTransform == null)
    //    {
    //        return;
    //    }

    //    rayLineRenderer.SetPosition(0, activeTransform.position);
    //    rayLineRenderer.SetPosition(1, activeTransform.position);
    //}


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
        //// Create a new GameObject for this cube
        //GameObject newCube = new GameObject("User Generated Frame");
        //newCube.layer = gameObject.layer;
        //newCube.tag = "Frame"; 
        GameObject newFrame;
        newFrame = Instantiate(framePrefab);
        newFrame.layer = gameObject.layer;
        newFrame.tag = "Frame";

        Mesh newMesh = Instantiate(cubeMesh);

        // Keep mesh exactly as previewed
        newFrame.transform.position = transform.position;
        newFrame.transform.rotation = transform.rotation;
        newFrame.transform.localScale = transform.localScale;

        // Add mesh components
        MeshFilter meshFilter = newFrame.AddComponent<MeshFilter>();
        meshFilter.mesh = newMesh;

        MeshRenderer newMeshRenderer = newFrame.AddComponent<MeshRenderer>();
        newMeshRenderer.material = squareMaterial;

        // Add rigidbody first (XRGrabInteractable requires it)
        Rigidbody newRb = newFrame.AddComponent<Rigidbody>();
        newRb.useGravity = false;
        newRb.isKinematic = true; // make finished frame not fall to ground

        // Add collider and fit to recentered mesh
        BoxCollider newBoxCollider = newFrame.AddComponent<BoxCollider>();
        newBoxCollider.size = newMesh.bounds.size;
        newBoxCollider.center = newMesh.bounds.center;

        // Add XRGrabInteractable and match interaction layers to this object's interactable
        XRGrabInteractable grabInteractable = newFrame.AddComponent<XRGrabInteractable>();
        XRGrabInteractable sourceInteractable = GetComponent<XRGrabInteractable>();
        if (sourceInteractable != null)
        {
            grabInteractable.interactionLayers = sourceInteractable.interactionLayers;
            grabInteractable.selectMode = InteractableSelectMode.Multiple;
        }

        // Fixed attach point on the larger face, rotated 90 degrees
        GameObject attachPoint = new GameObject("Attach Point");
        attachPoint.transform.SetParent(newFrame.transform, false);
        attachPoint.transform.localPosition =
            newMesh.bounds.center + Vector3.up * newMesh.bounds.extents.y;
        //attachPoint.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        XRGeneralGrabTransformer grabTransformer = newFrame.AddComponent<XRGeneralGrabTransformer>();
        XRGeneralGrabTransformer sourceTransformer = GetComponent<XRGeneralGrabTransformer>();
        if (sourceTransformer != null)
        {
            {
                grabTransformer.allowTwoHandedRotation = sourceTransformer.allowTwoHandedRotation;
            }
        }
        
        grabInteractable.attachTransform = attachPoint.transform;
        grabInteractable.useDynamicAttach = true;
        // TODO: can't rotate with two hands for some reason
        // TODO: make point the user started with the UP direction and make sure that always faces up

        //newCube.AddComponent<SnapFramesToWall>();
        //Debug.Log("Added snap frames script to component I THINK I SHOULD COME FIRST");


    }

    private static bool WasPressedThisFrame(InputActionReference actionReference)
    {
        var action = actionReference?.action;
        return action != null && action.WasPressedThisFrame();
    }

    private static bool WasReleasedThisFrame(InputActionReference actionReference)
    {
        var action = actionReference?.action;
        return action != null && action.WasReleasedThisFrame();
    }

    private static bool IsPressed(InputActionReference actionReference)
    {
        var action = actionReference?.action;
        return action != null && action.IsPressed();
    }

    private void HandleDownAction(Transform controllerPoint)
    {
        if (controllerPoint == null || isPlaneFinalized)
        {
            return;
        }

        startPosition = controllerPoint.position;
        Debug.Log("Plane started at: " + startPosition);
    }

    private void HandleHoldAction(Transform controllerPoint)
    {
        if (controllerPoint == null || isPlaneFinalized)
        {
            return;
        }

        GeneratePreview(startPosition, controllerPoint.position);
    }

    private void HandleUpAction(Transform controllerPoint)
    {
        if (controllerPoint == null || isPlaneFinalized)
        {
            return;
        }

        planeEndPosition = controllerPoint.position;

        // Calculate dimensions
        float width = Mathf.Abs(planeEndPosition.x - startPosition.x);
        float depth = Mathf.Abs(planeEndPosition.z - startPosition.z);

        // Calculate area
        float area = width * depth;

        // Reject tiny accidental frames
        if (area < minimumFrameArea)
        {
            Debug.Log(
                $"Frame too small. Area: {area:F4} m²"
            );

            squareMesh.Clear();
            return;
        }

        isPlaneFinalized = true;

        Debug.Log("Plane finished at: " + planeEndPosition);

        GenerateFrame(startPosition, planeEndPosition, cubeHeight);
        SpawnCube(squareMesh);

        squareMesh.Clear();
        isPlaneFinalized = false;
    }
}