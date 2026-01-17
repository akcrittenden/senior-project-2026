using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    public InputActionProperty testActionValue;
    public InputActionProperty testActionButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float value = testActionValue.action.ReadValue<float>(); // we mapped testActionValue to the Left Select button
        Debug.Log("Input Action Value: " + value);

        bool button = testActionButton.action.IsPressed(); // we mapped testActionValue to the Left Select button
        Debug.Log("Input Action Button: " + button);





    }
}
