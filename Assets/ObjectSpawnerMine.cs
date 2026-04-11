using Oculus.Interaction;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
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

    [SerializeField]
    [Tooltip("Assign the XR interactor (XRDirectInteractor, XRRayInteractor, etc.) from your rig. The interactor's attachTransform or transform will be used for spawn pose.")]
    private XRRayInteractor controllerInteractor;

    [SerializeField]
    private XROrigin xrOrigin;

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

    // Ensure prefab has an entry in furnitureEntries and return its index.
    // If prefab already exists in the list, returns its index; otherwise adds a new entry.
    public int EnsurePrefabRegistered(GameObject prefab)
    {
        if (prefab == null)
            return -1;

        for (int i = 0; i < furnitureEntries.Count; i++)
        {
            if (furnitureEntries[i].prefab == prefab)
                return i;
        }

        var newEntry = new FurnitureEntry { prefab = prefab };
        furnitureEntries.Add(newEntry);
        Debug.Log($"[ObjectSpawner] Registered new prefab '{prefab.name}' at index {furnitureEntries.Count - 1}");
        return furnitureEntries.Count - 1;
    }

    // Find index by prefab name. Returns -1 if not found.
    public int FindPrefabIndexByName(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName))
            return -1;
        for (int i = 0; i < furnitureEntries.Count; i++)
        {
            var p = furnitureEntries[i].prefab;
            if (p != null && p.name == prefabName)
                return i;
        }
        return -1;
    }

    void Update()
    {
        // spawn via UI buttons / manager now — no per-frame logic here
    }

    // Primary: spawn by prefab reference (used by FurnitureButtonManager)
    public void SpawnFurniture(GameObject furniturePrefab)
    {
        if (furniturePrefab == null)
        {
            Debug.LogWarning("[ObjectSpawner] Furniture prefab to spawn is null.");
            return;
        }

        //// Block spawn when pointing at UI
        //if (controllerInteractor != null && controllerInteractor.IsOverUIGameObject())
        //{
        //    Debug.Log("[ObjectSpawner] Spawn blocked: controller interactor is over UI.");
        //    return;
        //}

        Transform poseTransform = xrOrigin != null
            ? (xrOrigin.transform != null ? xrOrigin.transform : xrOrigin.transform)
            : transform; // fallback to this GameObject if no interactor assigned

        var spawnPos = poseTransform.position + poseTransform.forward * 0.6f + poseTransform.up * 0.3f;
        var spawnRot = poseTransform.rotation;

        var instance = Instantiate(furniturePrefab, spawnPos, spawnRot);

        var interactable = instance.GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            // Ensure prefab is registered and track the instance
            int index = EnsurePrefabRegistered(furniturePrefab);
            if (index >= 0)
            {
                furnitureEntries[index].instances.Add(interactable);
                Debug.Log($"[ObjectSpawner] Added spawned instance to entry {index} ('{furniturePrefab.name}'), total instances: {furnitureEntries[index].instances.Count}");
            }
        }
        else
        {
            Debug.LogWarning($"[ObjectSpawner] Spawned furniture '{furniturePrefab.name}' does not contain XRBaseInteractable component.");
        }
    }

    // Convenience: spawn by index (useful for inspector wiring)
    public void SpawnFurnitureByIndex(int index)
    {
        if (index < 0 || index >= furnitureEntries.Count)
        {
            Debug.LogWarning($"[ObjectSpawner] SpawnFurnitureByIndex: invalid index {index}.");
            return;
        }
        SpawnFurniture(furnitureEntries[index].prefab);
    }

    // Convenience: spawn by prefab name
    public void SpawnFurnitureByName(string prefabName)
    {
        var idx = FindPrefabIndexByName(prefabName);
        if (idx >= 0)
        {
            SpawnFurniture(furnitureEntries[idx].prefab);
            return;
        }

        Debug.LogWarning($"[ObjectSpawner] SpawnFurnitureByName: prefab '{prefabName}' not found in furnitureEntries.");
    }
}

