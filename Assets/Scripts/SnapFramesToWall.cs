using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Meta.XR.MRUtilityKit;

public class SnapFramesToWall : MonoBehaviour
{
    // need AR plane probably
    // and maybe arplanelistener?
    // 
    // private InputActionReference gripAction;

    public Transform rayStartPoint;
    public float rayLength = 5;
    public MRUKAnchor.SceneLabels labelFilter;
    [SerializeField] private TMPro.TextMeshPro text;
    //private InputActionAsset inputActions;

    private bool gripWasPressed = false;
    private string frameTag = "Frame";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //try
        //{
        //    InputAction gripAction = inputActions.FindActionMap("XRI Right Interaction").FindAction("Select");
        //}
        //catch (System.Exception e)
        //{
        //    Debug.LogError("Could not find grip action: " + e.Message);
        //}
    }

    // Update is called once per frame
    void Update()
        {
        Ray ray = new Ray(rayStartPoint.position, rayStartPoint.forward);


        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        var wallFilter = new LabelFilter(MRUKAnchor.SceneLabels.WALL_FACE);
        bool hasHit = room.Raycast(ray, rayLength, wallFilter, out RaycastHit hit, out MRUKAnchor anchor);

        if (hasHit)
        {
            Vector3 hitpoint = hit.point;
            Vector3 hitNormal = hit.normal;
            string label = anchor.HasAnyLabel(MRUKAnchor.SceneLabels.WALL_FACE) ? "WALL_FACE" : "UNKNOWN";

            text.transform.position = hitpoint;
            text.transform.rotation = Quaternion.LookRotation(-hitNormal);
            text.text = "DEBUG: " + label; 
        }

        //bool gripCurrentlyPressed = gripAction.action.IsPressed();
        //bool gripJustPressed = gripCurrentlyPressed && !gripWasPressed;
        //bool gripJustReleased = !gripCurrentlyPressed && gripWasPressed;
        //gripWasPressed = gripCurrentlyPressed;
        // get raycast hit WHILE holding something, maybe tag frames with "frame" or only named "User Generated Frame"
        // get wall normal
        // make sure wall is vertical or maybe the notaxisaligned with a few degrees of tolerance
        // set rotation of frame to match wall normal
        // 
        // get object being held
        //if (gripCurrentlyPressed)
        //{
        //    if (ray.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        //    {
        //        if (hit.collider.CompareTag(frameTag))
        //        {
        //            Vector3 wallNormal = hit.normal;
        //            // Check if the wall is vertical (within a certain angle tolerance)
        //            float angleTolerance = 10f; // degrees
        //            if (Vector3.Angle(wallNormal, Vector3.up) > angleTolerance)
        //            {
        //                // Snap the frame to the wall
        //                Transform frameTransform = hit.collider.transform;
        //                frameTransform.rotation = Quaternion.LookRotation(wallNormal);
        //            }
        //        }
        //    }
        //}

    }
}

