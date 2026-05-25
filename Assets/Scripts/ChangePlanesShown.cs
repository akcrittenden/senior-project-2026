using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ChangePlanesShown : MonoBehaviour
{
    // get XRorigin
    public GameObject XROrigin;
    public Dropdown dropdown;
    public ARPlaneManager planeManager;
    public PlaneDetectionMode planeDetection;

    void Start()
    {
        var existingXROrigin = FindFirstObjectByType<XROrigin>();
        if (existingXROrigin != null)
        {
            // Get the plane manager from XR origin
            planeManager = existingXROrigin.GetComponent<ARPlaneManager>();
            if (planeManager != null)
            {
                // Get the current plane detection mode
                var currentMode = planeManager.requestedDetectionMode;
                // Set the requested plane detection mode via enum
                planeManager.requestedDetectionMode = currentMode;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //check which settings options are toggled?
    }

    public void OnChangePlanesShown(int dropdownValue)
    {
        if (planeManager == null)
            return;

        switch (dropdownValue)
        {
            case 0:
                planeManager.requestedDetectionMode =
                    PlaneDetectionMode.Horizontal |
                    PlaneDetectionMode.Vertical |
                    PlaneDetectionMode.NotAxisAligned;
                break;

            case 1:
                // Show horizontal planes only
                planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
                break;

            case 2:
                // Show vertical surfaces only
                planeManager.requestedDetectionMode = PlaneDetectionMode.Vertical;
                break;

            case 3:
                // Show nothing
                planeManager.requestedDetectionMode = PlaneDetectionMode.None;
                break;
        }
    }
}
