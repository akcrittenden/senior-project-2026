using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class SaveAndLoad : MonoBehaviour
{
    private string currentlyLoadedFile;
    private System.DateTime lastLoadedFileTime;
    [SerializeField]
    private GameObject framePrefab;
    
    [SerializeField]
    private Material squareMaterial;

    [SerializeField]
    private TriggerSquareGenerator triggerSquareGenerator;

    [Serializable]
    public class GeneratedFrameData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public List<SerializableVector3> vertices =
            new List<SerializableVector3>();

        public List<int> triangles =
            new List<int>();
    }

    [Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3() { }

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }


    [Serializable]
    public class FurnitureData
    {
        public Vector3 position;
        public Quaternion rotation;
        public string prefabName;
        public Vector3 scale;
    }

    [Serializable]
    public class SaveDataModel
    {
        public List<FurnitureData> furnitureInstances =
            new List<FurnitureData>();

        public List<GeneratedFrameData> generatedFrames =
            new List<GeneratedFrameData>();
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

        // create and store actions, then make listeners for each button action
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
                    prefabName = furnitureEntry.prefab != null ? furnitureEntry.prefab.name : string.Empty,
                    scale = instance.transform.localScale
                };

                model.furnitureInstances.Add(furnitureData);
            }
        }

        GameObject[] frames = GameObject.FindGameObjectsWithTag("Frame");

        foreach (GameObject frame in frames)
        {
            MeshFilter meshFilter = frame.GetComponent<MeshFilter>();

            if (meshFilter == null || meshFilter.sharedMesh == null)
                continue;

            Mesh mesh = meshFilter.sharedMesh;

            GeneratedFrameData frameData =
                new GeneratedFrameData
                {
                    position = frame.transform.position,
                    rotation = frame.transform.rotation,
                    scale = frame.transform.localScale
                };

            foreach (Vector3 vertex in mesh.vertices)
            {
                frameData.vertices.Add(
                    new SerializableVector3(vertex));
            }

            frameData.triangles.AddRange(mesh.triangles);

            model.generatedFrames.Add(frameData);
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

        GameObject[] existingFrames = GameObject.FindGameObjectsWithTag("Frame");

        foreach (GameObject frame in existingFrames)
        {
            Destroy(frame);
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
                    instance.transform.localScale = furnitureData.scale; //set scale after instantiation

                    var interactable = instance.GetComponent<XRBaseInteractable>();
                    if (interactable != null)
                    {
                        objectSpawner.furnitureEntries[idx].instances.Add(interactable); // add to list to keep track for saving
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

        foreach (var frameData in model.generatedFrames)
        {
            GameObject newFrame =
                Instantiate(
                    framePrefab,
                    frameData.position,
                    frameData.rotation);

            newFrame.transform.localScale =
                frameData.scale;

            newFrame.layer = gameObject.layer;
            newFrame.tag = "Frame";

            // rebuild mesh
            Mesh mesh = new Mesh();

            Vector3[] vertices =
                new Vector3[
                    frameData.vertices.Count];

            for (int i = 0;
                 i < frameData.vertices.Count;
                 i++)
            {
                vertices[i] =
                    frameData.vertices[i]
                    .ToVector3();
            }

            mesh.vertices = vertices;
            mesh.triangles =
                frameData.triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // mesh filter
            MeshFilter meshFilter =
                newFrame.AddComponent<MeshFilter>();

            meshFilter.mesh = mesh;

            // renderer
            MeshRenderer renderer =
                newFrame.AddComponent<MeshRenderer>();

            renderer.material = squareMaterial;

            // rigidbody
            Rigidbody rb =
                newFrame.AddComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = true;

            // collider
            BoxCollider collider =
                newFrame.AddComponent<BoxCollider>();

            collider.size =
                mesh.bounds.size;

            collider.center =
                mesh.bounds.center;

            XRGrabInteractable grabInteractable =
                newFrame.AddComponent<
                    XRGrabInteractable>();

            XRGrabInteractable sourceInteractable =
                triggerSquareGenerator
                    .GetComponent<XRGrabInteractable>();

            if (sourceInteractable != null)
            {
                grabInteractable.interactionLayers =
                    sourceInteractable.interactionLayers;

                grabInteractable.selectMode =
                    sourceInteractable.selectMode;
            }

            grabInteractable.useDynamicAttach = true;

            // grab transformer
            XRGeneralGrabTransformer
                grabTransformer =
                newFrame.AddComponent<
                    XRGeneralGrabTransformer>();

            //grabTransformer.allowTwoHandedRotation();
        }

        currentlyLoadedFile = filename;
        lastLoadedFileTime = File.GetLastWriteTime(filePath);
    }
}
