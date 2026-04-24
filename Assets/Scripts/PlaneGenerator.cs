using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;

//[RequireComponent(typeof(MeshFilter))]
public class PlaneGenerator : MonoBehaviour
{
    //Vector3[] vertices;
    //int[] triangles;

    //Mesh mesh;
    [SerializeField] private InputActionReference triggerAction;

    public XRController controller;
    //[SerializeField] public InputActionReference pointButton;
    //[SerializeField] public GameObject pointObject;
    // line renderer to connect points

    void Start()
    {
        //mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh;
        
    }
    private void Update()
    {

        if (triggerAction?.action == null)
        {
            Debug.LogWarning("Trigger action not assigned!");
            return;
        }

        // Method 1: Check if performed this frame
        if (triggerAction.action.WasPerformedThisFrame())
        {
            Debug.Log("Trigger pressed!");
            CreatePoint();
        }

        // Method 2: Check held state (binary)
        if (triggerAction.action.IsPressed())
        {
            Debug.Log("Trigger held");
        }

        // Method 3: Get analog value (0-1)
        float triggerValue = triggerAction.action.ReadValue<float>();
        if (triggerValue > 0.1f)
        {
            Debug.Log($"Trigger analog: {triggerValue}");
        }

        }
        //bool triggerValue = pointButton.was;
        //// when button pressed, call createpoint
        //if (pointButton.action.ReadValue<bool>())
        //{
        //    //CreatePoint();
        //    Debug.Log(triggerValue);
        //}


    void CreatePoint()
    {
        // Does this need to be an anchor??
        // make point at controller front position and add to vertices list
        // get controller position
        //Vector3 controllerPosition = controller.transform.position;
        // instantiate point object at controller position
        // make first point a snapPoint
        // instantiate line renderer to first point
        // maybe keep track of which point came first? or instantiate a new one
        // snappoint: (Create empty gameobject to act as a socket, then use Vector3.Distance or sqrMagnitude to see if
        // object is within snap range
        // when in range, set object's position to snapPoint position)

    }

    void CreateShape()
    {
        //vertices = new Vector3[]
        //{
        //    //new Vector3(0, 0, 0),
        //    //new Vector3(0, 1, 0),
        //    //new Vector3(1, 0, 0),

        //};

        //    if (vertices.Length >= 3)
        //    {
        //        triangles = new int[]
        //        {
        //        0, 1, 2
        //        };
        //    }

        //}

        //void UpdateMesh()
        //{
        //    mesh.Clear();

        //    mesh.vertices = vertices;
        //    mesh.triangles = triangles;

        //    mesh.RecalculateNormals();
        //}
    }
}