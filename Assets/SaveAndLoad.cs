using System;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

public class SaveAndLoad : MonoBehaviour
{
    [Serializable]
    public class SaveDataModel
    {
        public string playerName = "Gage";
        public float health = 69.0f;
        public Vector3 playerPosition;
    }

    private XROrigin xrOrigin;

    public InputActionReference aButton;
    public InputActionReference bButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();
        aButton.action.performed += AButton;
        bButton.action.performed += BButton;
    }

    private void BButton(InputAction.CallbackContext callback)
    {
        Debug.Log("B button pressed - from event");
    }

    private void AButton(InputAction.CallbackContext callback)
    {
        Debug.Log("A button pressed - from event");
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

        string json = JsonUtility.ToJson(model);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("Data Saved");
    }

    void LoadData()
    {
        SaveDataModel model = JsonUtility.FromJson<SaveDataModel>(File.ReadAllText(Application.persistentDataPath + "/savefile.json"));
        Debug.Log("Data Loaded");
        Debug.Log($"Last position: {model.playerPosition}");
    }

}
