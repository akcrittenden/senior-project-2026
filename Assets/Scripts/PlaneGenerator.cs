using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Oculus.Interaction.Input;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(MeshFilter))]
public class PlaneGenerator : MonoBehaviour
{
    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    [SerializeField] private XRDirectInteractor controller;
    [SerializeField] private InputActionProperty pointButton;
    [SerializeField] private GameObject pointObject;
    // line renderer to connect points

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

    }

    private void Update()
    {
        bool triggerValue = pointButton.action.ReadValue<bool>();
        // when button pressed, call createpoint
        if (pointButton.action.ReadValue<bool>())
        {
            //CreatePoint();
            Debug.Log(triggerValue);
        }

    }

    void CreatePoint()
    {
        // Does this need to be an anchor??
        // make point at controller front position and add to vertices list
        // get controller position
        Vector3 controllerPosition = controller.transform.position;
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

        if (vertices.Length >= 3)
        {
            triangles = new int[]
            {
            0, 1, 2
            };
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}