
using Oculus.Interaction;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class FurnitureEntry
    {
        [Tooltip("Furniture prefab to spawn")]
        public GameObject prefab;

        [System.NonSerialized]
        public List<XRBaseInteractable> instances = new List<XRBaseInteractable>();
    }

    [SerializeField]
    [Tooltip("List of furniture prefabs that can be spawned")]
    public List<FurnitureEntry> furnitureEntries = new List<FurnitureEntry>();

    //[Tooltip("Input Action configured for spawn (e.g. trigger).")]
    //public InputActionProperty spawnButton;

    //[Tooltip("Input Action configured for grip button.")]
    //public InputActionProperty gripButton;

    [SerializeField]
    [Tooltip("Assign the XR interactor (XRDirectInteractor, XRRayInteractor, etc.) from your rig. The interactor's attachTransform or transform will be used for spawn pose.")]
    // private XRBaseInteractor controllerInteractor;
    private XRRayInteractor controllerInteractor;

    public IReadOnlyList<GameObject> FurniturePrefabs
    {
        get
        {
            var list = new List<GameObject>(furnitureEntries.Count);
            foreach (var e in furnitureEntries)
                list.Add(e.prefab);
            return list;
        }
    }
    void Update()
    {
        //var spawnAction = spawnButton.action;
        //var gripAction = gripButton.action;
        ////Debug.Log($"IsOverUIGameObject: {controllerInteractor.IsOverUIGameObject()}");

        //if (spawnAction == null)
        //    return;

        //if (spawnAction.WasReleasedThisFrame())
        //{
        //    if (!gripAction.IsPressed())
        //        SpawnFurniture();
        //}
    }

    public void SpawnFurniture(GameObject furniturePrefab)
    {
        if (furniturePrefab == null)
        {
            Debug.LogWarning("Furniture prefab to spawn is null.");
            return;
        }

        if (controllerInteractor && !controllerInteractor.IsOverUIGameObject())
            {
                Transform poseTransform = controllerInteractor.attachTransform != null
                    ? controllerInteractor.attachTransform : controllerInteractor.transform;

                var spawnPos = poseTransform.position + poseTransform.forward * 1.5f;
                var spawnRot = poseTransform.rotation;

                var instance = Instantiate(furniturePrefab, spawnPos, spawnRot);

                var interactable = instance.GetComponent<XRBaseInteractable>();
                if (interactable != null)
                {
                // Find which FurnitureEntry this prefab belongs to and add the instance
                    for (int i = 0; i < furnitureEntries.Count; i++)
                    {
                        if (furnitureEntries[i].prefab == furniturePrefab)
                        {
                            furnitureEntries[i].instances.Add(interactable);
                            break;
                        }
                    }
                }
            }
    } 
}
