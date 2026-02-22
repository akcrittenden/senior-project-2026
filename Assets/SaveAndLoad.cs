using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SaveAndLoad : MonoBehaviour
{
    private string currentlyLoadedFile;
    private System.DateTime lastLoadedFileTime;

    [Serializable]
    public class FurnitureData
    {
        public Vector3 position;
        public Quaternion rotation;
        public string prefabName;
    }

    [Serializable]
    public class SaveDataModel
    {
        public List<FurnitureData> furnitureInstances = new List<FurnitureData>();
    }

    [SerializeField]
    private Button save1Button;

    [SerializeField]
    private Button save2Button;

    [SerializeField]
    private Button load1Button;

    [SerializeField]
    private Button load2Button;

    // stored UnityActions so we can remove them later
    private UnityAction save1Action;
    private UnityAction save2Action;
    private UnityAction load1Action;
    private UnityAction load2Action;

    private XROrigin xrOrigin;
    public ObjectSpawner objectSpawner;

    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();
        objectSpawner = FindFirstObjectByType<ObjectSpawner>();

        // create and store actions so RemoveListener works
        if (save1Button != null)
        {
            save1Action = () => SaveData("savefile1.json");
            save1Button.onClick.AddListener(save1Action);
            Debug.Log("Save1 button listener added");
        }
        else
            Debug.LogError("Save1 button not assigned in Inspector.");

        if (save2Button != null)
        {
            save2Action = () => SaveData("savefile2.json");
            save2Button.onClick.AddListener(save2Action);
            Debug.Log("Save2 button listener added");
        }
        else
            Debug.LogError("Save2 button not assigned in Inspector.");

        if (load1Button != null)
        {
            load1Action = () => LoadData("savefile1.json");
            load1Button.onClick.AddListener(load1Action);
            Debug.Log("Load1 button listener added");
        }
        else
            Debug.LogError("Load1 button not assigned in Inspector.");

        if (load2Button != null)
        {
            load2Action = () => LoadData("savefile2.json");
            load2Button.onClick.AddListener(load2Action);
            Debug.Log("Load2 button listener added");
        }
        else
            Debug.LogError("Load2 button not assigned in Inspector.");
    }

    private void OnDestroy()
    {
        // remove listeners using the stored delegates
        if (save1Button != null && save1Action != null)
            save1Button.onClick.RemoveListener(save1Action);
        if (save2Button != null && save2Action != null)
            save2Button.onClick.RemoveListener(save2Action);
        if (load1Button != null && load1Action != null)
            load1Button.onClick.RemoveListener(load1Action);
        if (load2Button != null && load2Action != null)
            load2Button.onClick.RemoveListener(load2Action);
    }

    public void SaveData(string filename)
    {
        if (objectSpawner == null)
        {
            Debug.LogError("objectSpawner is null! SaveData aborted.");
            return;
        }
        SaveDataModel model = new SaveDataModel();

        foreach (var furnitureEntry in objectSpawner.furnitureEntries)
        {
            foreach (var instance in furnitureEntry.instances)
            {
                var furnitureData = new FurnitureData
                {
                    position = instance.transform.position,
                    rotation = instance.transform.rotation,
                    prefabName = furnitureEntry.prefab != null ? furnitureEntry.prefab.name : string.Empty
                };

                model.furnitureInstances.Add(furnitureData);
            }
        }

        string json = JsonUtility.ToJson(model);
        string filePath = Path.Combine(Application.persistentDataPath, filename);
        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"Data Saved to {filename}. Furniture Count: {model.furnitureInstances.Count}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save data to {filePath}: {ex.Message}");
        }
    }

    public void LoadData(string filename)
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Save file not found: {filePath}");
            return;
        }

        if (currentlyLoadedFile == filename)
        {
            System.DateTime fileModifiedTime = File.GetLastWriteTime(filePath);
            if (fileModifiedTime == lastLoadedFileTime)
            {
                Debug.Log($"{filename} is already loaded. Skipping load.");
                return;
            }
        }

        SaveDataModel model;
        try
        {
            var fileText = File.ReadAllText(filePath);
            model = JsonUtility.FromJson<SaveDataModel>(fileText);
            if (model == null)
            {
                Debug.LogError($"Failed to parse save data from {filename}. File may be corrupted.");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load data from {filePath}: {ex.Message}");
            return;
        }

        // clear out all objects
        if (objectSpawner != null)
        {
            foreach (var furnitureEntry in objectSpawner.furnitureEntries)
            {
                foreach (var instance in furnitureEntry.instances)
                {
                    Debug.Log("Destroying instance of furniture");
                    Destroy(instance.gameObject);
                }
                furnitureEntry.instances.Clear();
            }
        }

        if (objectSpawner != null && model.furnitureInstances.Count > 0)
        {
            foreach (var furnitureData in model.furnitureInstances)
            {
                if (string.IsNullOrEmpty(furnitureData.prefabName))
                    continue;

                int idx = objectSpawner.FindPrefabIndexByName(furnitureData.prefabName);
                if (idx >= 0)
                {
                    var prefab = objectSpawner.furnitureEntries[idx].prefab;
                    Debug.Log($"Respawning furniture '{furnitureData.prefabName}' at {furnitureData.position}");
                    var instance = Instantiate(prefab, furnitureData.position, furnitureData.rotation);

                    var interactable = instance.GetComponent<XRBaseInteractable>();
                    if (interactable != null)
                    {
                        objectSpawner.furnitureEntries[idx].instances.Add(interactable);
                        Debug.Log($"Added loaded instance to entry {idx} (total now: {objectSpawner.furnitureEntries[idx].instances.Count})");
                    }
                    else
                    {
                        Debug.LogWarning($"Spawned furniture does not have XRBaseInteractable component.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Prefab '{furnitureData.prefabName}' not found in objectSpawner.furnitureEntries. Skipping spawn.");
                }
            }
        }

        currentlyLoadedFile = filename;
        lastLoadedFileTime = File.GetLastWriteTime(filePath);
    }
}
