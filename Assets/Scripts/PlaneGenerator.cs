using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(XRController))]

public class PlaneGenerator : MonoBehaviour
{
    public XRDirectInteractor controller;
    public XRRayInteractor ray;
    protected Vector3 controllerPosition;
    protected Quaternion controllerRotation;

    Vector3[] vertices;
    int[] triangles;
    Mesh mesh; 

    [SerializeField] private InputActionReference triggerAction;
    [SerializeField] private GameObject pointPrefab;


    // line renderer to connect points

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

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
            if (!ray.IsOverUIGameObject())
            {
                Debug.Log("Trigger pressed! Getting position...");
                CreatePoint();
            }
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
        controllerPosition = controller.transform.position;
        controllerRotation = controller.transform.rotation;
        GameObject point = Instantiate(pointPrefab, controllerPosition, controllerRotation);
        Debug.Log("Sphere created at: " + controllerPosition);
        // make point at controller front position and add to vertices list

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