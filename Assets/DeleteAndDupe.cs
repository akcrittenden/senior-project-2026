using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class DeleteAndDupe : MonoBehaviour
{

    [SerializeField] private XRRayInteractor rayInteractor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rayInteractor = FindFirstObjectByType<XRRayInteractor>();
        rayInteractor.hoverEntered.AddListener(OnHoverEntered);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        // get current gameObject that is being hovered over
        GameObject hoveredObject = args.interactableObject.transform.gameObject;
        // print id of hoveredObject
        Debug.Log("Hovered over: " + hoveredObject.name);
    }

    private void OnDestroy()
    {
        rayInteractor.hoverEntered.RemoveListener(OnHoverEntered);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
