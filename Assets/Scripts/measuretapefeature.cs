using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class measuretapefeature : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [UnityEngine.Range(0.005f, 0.05f)]
    [SerializeField] private float tapeWidth = 0.01f;
    [SerializeField] private Material tapeMaterial;
    [SerializeField] private Transform leftControllerPoint;
    [SerializeField] private Transform rightControllerPoint;
    [SerializeField] private InputActionReference leftTapeAction;
    [SerializeField] private InputActionReference rightTapeAction;

    private readonly List<GameObject> savedTapeLines = new();
    private LineRenderer lastTapeLineRenderer;

    private ActiveController activeController = ActiveController.None;

    private enum ActiveController
    {
        None,
        Left,
        Right
    }

    private void Update()
    {
        if (activeController == ActiveController.None)
        {
            if (WasPressedThisFrame(leftTapeAction))
            {
                activeController = ActiveController.Left;
                HandleDownAction(leftControllerPoint);
            }
            else if (WasPressedThisFrame(rightTapeAction))
            {
                activeController = ActiveController.Right;
                HandleDownAction(rightControllerPoint);
            }

            return;
        }

        if (activeController == ActiveController.Left)
        {
            if (IsPressed(leftTapeAction))
            {
                HandleHoldAction(leftControllerPoint);
            }
            else if (WasReleasedThisFrame(leftTapeAction))
            {
                HandleUpAction(leftControllerPoint);
                activeController = ActiveController.None;
            }

            return;
        }

        if (IsPressed(rightTapeAction))
        {
            HandleHoldAction(rightControllerPoint);
        }
        else if (WasReleasedThisFrame(rightTapeAction))
        {
            HandleUpAction(rightControllerPoint);
            activeController = ActiveController.None;
        }
    }

    private static bool WasPressedThisFrame(InputActionReference actionReference)
    {
        var action = actionReference?.action;
        return action != null && action.WasPressedThisFrame();
    }

    private static bool WasReleasedThisFrame(InputActionReference actionReference)
    {
        var action = actionReference?.action;
        return action != null && action.WasReleasedThisFrame();
    }

    private static bool IsPressed(InputActionReference actionReference)
    {
        var action = actionReference?.action;
        return action != null && action.IsPressed();
    }

    private void HandleDownAction(Transform tapeArea)
    {
        if (tapeArea == null)
        {
            return;
        }

        CreateNewTapeLine(tapeArea.position);
    }

    private void HandleHoldAction(Transform tapeArea)
    {
        if (tapeArea == null || lastTapeLineRenderer == null)
        {
            return;
        }

        lastTapeLineRenderer.SetPosition(1, tapeArea.position);
    }

    private void HandleUpAction(Transform tapeArea)
    {
        HandleHoldAction(tapeArea);
    }

    private void CreateNewTapeLine(Vector3 initialPosition)
    {
        var newTapeLine = new GameObject($"TapeLine_{savedTapeLines.Count}", typeof(LineRenderer));
        lastTapeLineRenderer = newTapeLine.GetComponent<LineRenderer>();
        lastTapeLineRenderer.positionCount = 2;
        lastTapeLineRenderer.startWidth = tapeWidth;
        lastTapeLineRenderer.endWidth = tapeWidth;
        lastTapeLineRenderer.material = tapeMaterial;
        lastTapeLineRenderer.SetPosition(0, initialPosition);
        lastTapeLineRenderer.SetPosition(1, initialPosition);

        savedTapeLines.Add(newTapeLine);
    }
}
