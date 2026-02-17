using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
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
        public int furniturePrefabIndex;
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

    private XROrigin xrOrigin;
    public ObjectSpawner objectSpawner;

    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();
        objectSpawner = FindFirstObjectByType<ObjectSpawner>();

        if (save1Button != null)
        {
            save1Button.onClick.AddListener(OnSave1ButtonClicked);
            Debug.Log("Save1 button listener added");
        }
        else
            Debug.LogError("Save1 button not assigned in Inspector.");

        if (save2Button != null)
        {
            save2Button.onClick.AddListener(OnSave2ButtonClicked);
            Debug.Log("save2Button listener added");
        }
        else
            Debug.LogError("Save2 button not assigned in Inspector.");

        if (load1Button != null)
        {
            load1Button.onClick.AddListener(OnLoad1ButtonClicked);
            Debug.Log("Load1 button listener added");
        }
        else
            Debug.LogError("Load1 button not assigned in Inspector.");

        if (load2Button != null)
        {
            load2Button.onClick.AddListener(OnLoad2ButtonClicked);
            Debug.Log("load2Button listener added");
        }
        else
            Debug.LogError("Load2 button not assigned in Inspector.");
    }

    private void OnSave1ButtonClicked()
    {
        Debug.Log(">>> Save 1 button clicked <<<");
        SaveData("savefile1.json");
    }

    private void OnSave2ButtonClicked()
    {
        Debug.Log(">>> Save 2 button clicked <<<");
        SaveData("savefile2.json");
    }

    private void OnLoad1ButtonClicked()
    {
        Debug.Log(">>> Load 1 button clicked <<<");
        LoadData("savefile1.json");
    }

    private void OnLoad2ButtonClicked()
    {
        Debug.Log(">>> Load 2 button clicked <<<");
        LoadData("savefile2.json");
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
                    furniturePrefabIndex = objectSpawner.furnitureEntries.IndexOf(furnitureEntry)
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

        // check if we've already loaded this save file
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
                if (furnitureData.furniturePrefabIndex >= 0 && furnitureData.furniturePrefabIndex < objectSpawner.furnitureEntries.Count)
                {
                    var prefab = objectSpawner.furnitureEntries[furnitureData.furniturePrefabIndex].prefab;
                    Debug.Log($"Respawning furniture at {furnitureData.position}");
                    var instance =  Instantiate(prefab, furnitureData.position, furnitureData.rotation);

                    var interactable = instance.GetComponent<XRBaseInteractable>();
                    if (interactable != null)
                    {
                        objectSpawner.furnitureEntries[furnitureData.furniturePrefabIndex].instances.Add(interactable);
                        Debug.Log($"Added loaded instance to entry {furnitureData.furniturePrefabIndex} (total now: {objectSpawner.furnitureEntries[furnitureData.furniturePrefabIndex].instances.Count})");
                    }
                    else
                    {
                        Debug.LogWarning($"Spawned furniture does not have XRBaseInteractable component.");
                    }
                }
            }
        }
        currentlyLoadedFile = filename;
        lastLoadedFileTime = File.GetLastWriteTime(filePath);
    }

    private void OnDestroy()
    {
        if (save1Button != null)
            save1Button.onClick.RemoveListener(OnSave1ButtonClicked);
        if (save2Button != null)
            save2Button.onClick.RemoveListener(OnSave2ButtonClicked);
        if (load1Button != null)
            load1Button.onClick.RemoveListener(OnLoad1ButtonClicked);
        if (load2Button != null)
            load2Button.onClick.RemoveListener(OnLoad2ButtonClicked);
    }
}
