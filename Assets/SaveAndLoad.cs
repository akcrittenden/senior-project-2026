using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

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
        public Vector3 playerPosition;
        public List<FurnitureData> furnitureInstances = new List<FurnitureData>();
    }

    private XROrigin xrOrigin;
    public ObjectSpawner objectSpawner;

    //not recommended: public InputAction lets you directly bind a key to this action, but this means I can't use the binding mapping we configured in the Default Input Action asset
    // and also I think you can't call this in code?
    public InputActionReference aButton;
    public InputActionReference bButton; // reference lets you bind an action from the Default Input Action Asset in the inspector
    // public InputActionProperty lets you EITHER bind a key directly OR use a reference to the Default Input Action asset (pick with three dots)


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();
        objectSpawner = FindFirstObjectByType<ObjectSpawner>();

        // next, determine when action will be triggered.
        // in input system, timing is deternmined through event mechanism. when system detects input bound to action has occured, action is triggered and event provided by input
        // System is invoked. We can bind our custom methods to this event so when system detects action being triggered, it will execute method we have bound to that event
        // aButton.action.performed is the event that is invoked when the action is performed (button pressed)
        // bind custom method to it with `+= AButton`
        aButton.action.performed += AButton; // capital AButton is the name of the mapping in the Default Input Action asset (performed is actually a delegate type but not really relevant here)
        bButton.action.performed += BButton; // REMEMBER TO UNBIND LATER
    }

    private void OnDestroy()
    {
        // unbind methods from events to prevent memory leaks
        aButton.action.performed -= AButton;
        bButton.action.performed -= BButton;
    }

    private void AButton(InputAction.CallbackContext callback)
    {
        // if this was the trigger and we need the exactly value:
        // float value = callback.ReadValue<float>();
        // output the value if needed:
        // print($"Trigger pressed with value: {value}");
        Debug.Log("A button pressed - from event");
        SaveData();
    }

    private void BButton(InputAction.CallbackContext callback)
    {
        Debug.Log("B button pressed - from event");
        LoadData();
    }

    void Update()
    {
        // For demonstration, we print the loaded data every frame
        //Debug.Log($"Player Position: {xrOrigin.transform.position}");
        
        // if press A, SaveData();
 

        // if press B, LoadData();


    }
    void SaveData()
    {
        SaveDataModel model = new SaveDataModel();
        model.playerName = "Amanda";
        model.health = 100.0f;
        model.playerPosition = xrOrigin.transform.position;

        //save all furniture
        if (objectSpawner != null)
        {
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
        }

        string json = JsonUtility.ToJson(model);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log($"Data Saved. Furniture Count: {model.furnitureInstances.Count}");
    }

    void LoadData()
    {
        SaveDataModel model = JsonUtility.FromJson<SaveDataModel>(File.ReadAllText(Application.persistentDataPath + "/savefile.json"));
        Debug.Log("Data Loaded");
        Debug.Log($"Moving player to saved position: {model.playerPosition}");
        //xrOrigin.transform.position = model.playerPosition;

        // delete current furniture first
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

        // respawn furniture at saved positions

        if (objectSpawner != null && model.furnitureInstances.Count > 0)
        {
            foreach (var furnitureData in model.furnitureInstances)
            {
                if (furnitureData.furniturePrefabIndex >=0 && furnitureData.furniturePrefabIndex < objectSpawner.furnitureEntries.Count)
                {
                    var prefab = objectSpawner.furnitureEntries[furnitureData.furniturePrefabIndex].prefab;
                    Instantiate(prefab, furnitureData.position, furnitureData.rotation);
                }
            }
        }
    }

}
