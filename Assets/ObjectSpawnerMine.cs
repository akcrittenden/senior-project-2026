
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

    private InputActionMap xriActionMap;

    [SerializeField]
    [Tooltip("List of furniture prefabs that can be spawned")]
    private List<FurnitureEntry> furnitureEntries = new List<FurnitureEntry>();

    [Tooltip("If >=0 and within range, this index will be used to spawn that specific furniture prefab.")]
    [SerializeField]
    private int spawnOptionIndex = -1;

    [Tooltip("Input Action configured for spawn (e.g. trigger).")]
    public InputActionProperty spawnButton;

    [Tooltip("Input Action configured for grip button.")]
    public InputActionProperty gripButton;

    [SerializeField]
    [Tooltip("Assign the XR interactor (XRDirectInteractor, XRRayInteractor, etc.) from your rig. The interactor's attachTransform or transform will be used for spawn pose.")]
    private XRBaseInteractor controllerInteractor;

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
        var spawnAction = spawnButton.action;
        var gripAction = gripButton.action;
        

        if (spawnAction == null)
            return;

        // Don't spawn if grip button is held down while trigger is pressed
        if (gripAction != null && gripAction.IsPressed())
            return;

        if (spawnAction.WasReleasedThisFrame())
        {
            SpawnObject();
        }
    }

    public void SpawnObject()
    {
        if (furnitureEntries == null || furnitureEntries.Count == 0)
        {
            Debug.LogWarning("No furniture prefabs assigned to spawn.");
            return;
        }

        FurnitureEntry entryToSpawn;
        if (spawnOptionIndex >= 0 && spawnOptionIndex < furnitureEntries.Count)
        {
            entryToSpawn = furnitureEntries[spawnOptionIndex];
        }
        else
        {
            entryToSpawn = furnitureEntries[Random.Range(0, furnitureEntries.Count)];
        }
        if (entryToSpawn == null)
        {
            Debug.LogWarning("Selected furniture prefab is null.");
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

        var spawnPos = poseTransform.position + poseTransform.forward * 1.5f;
        var spawnRot = poseTransform.rotation;

        var instance = Instantiate(entryToSpawn.prefab, spawnPos, spawnRot);

        var interactable = instance.GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            entryToSpawn.instances.Add(interactable);
        }

    }
}
