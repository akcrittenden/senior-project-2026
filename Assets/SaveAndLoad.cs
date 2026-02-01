using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoad : MonoBehaviour
{
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
        public string playerName = "Gage";
        public float health = 69.0f;
        public List<FurnitureData> furnitureInstances = new List<FurnitureData>();
    }

    [SerializeField]
    private Button save1Button;

    [SerializeField]
    private Button load1Button;

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

        if (load1Button != null)
        {
            load1Button.onClick.AddListener(OnLoad1ButtonClicked);
            Debug.Log("Load1 button listener added");
        }
        else
            Debug.LogError("Load1 button not assigned in Inspector.");
    }

    private void OnSave1ButtonClicked()
    {
        Debug.Log(">>> Save button clicked <<<");
        SaveData();
    }

    private void OnLoad1ButtonClicked()
    {
        Debug.Log(">>> Load button clicked <<<");
        LoadData();
    }

    public void SaveData()
    {
        if (objectSpawner == null)
        {
            Debug.LogError("objectSpawner is null! SaveData aborted.");
            return;
        }
        SaveDataModel model = new SaveDataModel();
        model.playerName = "Amanda";
        model.health = 100.0f;

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
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log($"Data Saved. Furniture Count: {model.furnitureInstances.Count}");
    }

    public void LoadData()
    {
        SaveDataModel model = JsonUtility.FromJson<SaveDataModel>(File.ReadAllText(Application.persistentDataPath + "/savefile.json"));
        Debug.Log($"Data Loaded. Furniture instances in save: {model.furnitureInstances.Count}");

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
                    Instantiate(prefab, furnitureData.position, furnitureData.rotation);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (save1Button != null)
            save1Button.onClick.RemoveListener(OnSave1ButtonClicked);
        if (load1Button != null)
            load1Button.onClick.RemoveListener(OnLoad1ButtonClicked);
    }
}
