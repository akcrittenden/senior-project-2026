
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class ObjectSpawner : MonoBehaviour
{
    [Tooltip("Prefab to spawn.")]
    public GameObject prefabToSpawn;

    [Tooltip("Input Action configured for spawn (e.g. trigger).")]
    public InputActionProperty spawnButton;

    [SerializeField]
    [Tooltip("Assign the XR interactor (XRDirectInteractor, XRRayInteractor, etc.) from your rig. The interactor's attachTransform or transform will be used for spawn pose.")]
    XRBaseInteractor controllerInteractor;

    void Update()
    {
        var action = spawnButton.action;
        if (action != null && action.WasReleasedThisFrame())
        {
            SpawnObject();
        }
    }

    public void SpawnObject()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Prefab to spawn is not assigned.");
            return;
        }

        Transform poseTransform = null;
        if (controllerInteractor != null)
        {
            poseTransform = controllerInteractor.attachTransform != null
                ? controllerInteractor.attachTransform : controllerInteractor.transform;
        }
        else
        {
            poseTransform = transform; //fallback
        }

        var spawnPos = poseTransform.position + poseTransform.forward * 0.5f;
        var spawnRot = poseTransform.rotation;

        Instantiate(prefabToSpawn, spawnPos, spawnRot);

    }
}
