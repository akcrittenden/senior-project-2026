using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class DeleteAndDupe : MonoBehaviour
{
    [SerializeField] private XRRayInteractor rayInteractor;
    private ObjectSpawner objectSpawner;
    public GameObject objectAtRay;

    public InputActionReference xButton;
    public InputActionReference yButton;

    void Start()
    {
        rayInteractor = FindFirstObjectByType<XRRayInteractor>();
        objectSpawner = FindFirstObjectByType<ObjectSpawner>();
        rayInteractor.hoverEntered.AddListener(OnHoverEntered);

        //xButton.action.performed += XButton;
        //yButton.action.performed += YButton;
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        // get current gameObject that is being hovered over
        objectAtRay = args.interactableObject.transform.gameObject;
        // print id of hoveredObject
        Debug.Log($"Hovered over: {objectAtRay.GetInstanceID()}");
    }

    //private void YButton(InputAction.CallbackContext callback)
    //{
    //    Debug.Log("Y button pressed - from event");
    //    DeleteObject();
    //}

    //private void XButton(InputAction.CallbackContext callback)
    //{
    //    Debug.Log("X button pressed - from event");
    //    DuplicateObject();
    //}

    //void DeleteObject()
    //{
    //    // get gameobject from ray interactor
    //    Debug.Log("Deleting Object...");
    //    GameObject objectToDelete = objectAtRay;
    //    // remove object from list of furnitureEntries in ObjectSpawner script
    //    if (objectToDelete != null)
    //    {
    //        var interactableToRemove = objectToDelete.GetComponent<XRBaseInteractable>();
    //        if (interactableToRemove != null && objectSpawner != null)
    //        {
    //            //find and remove from furnitureEntries
    //            foreach(var furnitureEntry in objectSpawner.furnitureEntries)
    //            {
    //                if (furnitureEntry.instances.Contains(interactableToRemove))
    //                {
    //                    furnitureEntry.instances.Remove(interactableToRemove);
    //                    Debug.Log($"Removed object {objectToDelete.GetInstanceID()} from furniture entry.");
    //                    break;
    //                }
    //            }
    //        }

    //        // Destroy the object from the scene
    //        Destroy(objectToDelete);
    //        Debug.Log($"Destroyed object: {objectToDelete.GetInstanceID()}");
    //        objectAtRay = null;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No object to delete.");
    //    }
    //}

    //private void DuplicateObject()
    //{
    //    Debug.Log("Duplicating Object...");
    //    GameObject duplicatedObject = null;
    //    //copy that gameobject and make another one and have it spawn next to the pointed at object
    //    if (objectAtRay != null)
    //    {
    //        //get position of hovered object
    //        Vector3 hoverPosition = objectAtRay.transform.position;
    //        // set spawn position of new object to be slightly offset from hovered object
    //        Vector3 spawnPosition = hoverPosition + new Vector3(1f, 0, 0);
    //        // instantiate new object
    //        duplicatedObject = Instantiate(objectAtRay, spawnPosition, objectAtRay.transform.rotation);

    //        if (objectSpawner != null && duplicatedObject != null)
    //        {
    //            var interactable = duplicatedObject.GetComponent<XRBaseInteractable>();
    //            if (interactable != null)
    //            {
    //                foreach(var furnitureEntry in objectSpawner.furnitureEntries)
    //                {
    //                    if (furnitureEntry.instances.Contains(objectAtRay.GetComponent<XRBaseInteractable>()))
    //                    {
    //                        furnitureEntry.instances.Add(interactable);
    //                        break;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    // Debug log the ID of the duplicated object
    //    Debug.Log($"Duplicated object: {duplicatedObject.GetInstanceID()}");
       
    //}
    // Update is called once per frame
    void Update()
    {
        
    }
    //private void OnDestroy()
    //{
    //    xButton.action.performed -= XButton;
    //    yButton.action.performed -= YButton;
    //}
}
