using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

public class FurnitureButtonManager : MonoBehaviour
{
    [SerializeField]
    private List<Button> furnitureButtons = new List<Button>();

    [SerializeField]
    private ObjectSpawner objectSpawner; // assign in Inspector or let fallback find it

    void Start()
    {
        if (objectSpawner == null)
            objectSpawner = FindFirstObjectByType<ObjectSpawner>();

        foreach (var button in furnitureButtons)
        {
            if (button == null) continue;

            // Try handler on the Button GameObject first, then in children
            var handler = button.GetComponent<FurnitureButtonHandler>() ?? button.GetComponentInChildren<FurnitureButtonHandler>();

            if (handler != null)
            {
                button.onClick.AddListener(() =>
                {
                    if (objectSpawner != null)
                        objectSpawner.SpawnFurniture(handler.GetFurniturePrefab());
                    else
                        Debug.LogError("ObjectSpawner not assigned or found.");
                });
            }
            else
            {
                Debug.LogWarning($"Button '{button.name}' is missing a FurnitureButtonHandler.");
            }
        }
    }
}