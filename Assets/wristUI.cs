using UnityEngine;
using UnityEngine.InputSystem;

public class wristUI : MonoBehaviour
{
    private Canvas _wristUICanvas;
    [SerializeField] public InputActionReference _menuButton;

    private void Start()
    {
        _wristUICanvas = GetComponent<Canvas>();
        if (_menuButton != null)
        {
            _menuButton.action.performed += ToggleMenu;
        }
    }

    private void OnDestroy()
    {
        _menuButton.action.performed -= ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        _wristUICanvas.enabled = !_wristUICanvas.enabled;
    }
}
