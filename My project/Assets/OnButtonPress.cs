using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class OnButtonPress : MonoBehaviour
{
    [Tooltip("Actions to check")]
    public InputAction action = null;

    public UnityEvent OnPress = new UnityEvent();

    public UnityEvent OnRelease = new UnityEvent();

    private void Awake()
    {
        action.started += Pressed;
        action.canceled += Released;
    }

    private void OnDestroy()
    {
        action.started -= Pressed;
        action.canceled -= Released;
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    private void Pressed(InputAction.CallbackContext context)
    {
        OnPress.Invoke();
    }

    private void Released(InputAction.CallbackContext context)
    {
        OnRelease.Invoke();
    }

}
