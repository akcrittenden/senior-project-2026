
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;


public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public InputActionProperty spawnButton;

    // Update is called once per frame
    void Update()
    {
       if (spawnButton.action.IsPressed())
       {
            SpawnObject();
       }
    }

    public void SpawnObject()
    {
        GameObject furniture = Instantiate(prefabToSpawn, OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch));
        if (furniture != null)
        {
            Vector3 spawnPosition = transform.position + transform.forward * 0.5f; // Spawn 0.5 units in front of the spawner
            Quaternion spawnRotation = Quaternion.identity; // No rotation
            Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
        }
        else
        {
            Debug.LogError("Prefab not found in Resources folder!");
}
    }
}
