using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GamepadVirtualMouse : MonoBehaviour
{
    public RectTransform cursorGraphic;
    public float speed = 1000f;

    private Mouse virtualMouse;
    private Vector2 position;

    void OnEnable()
    {
        // Crea un mouse virtual si no existe
        if (Mouse.current == null)
        {
            InputSystem.AddDevice<Mouse>();
        }

        virtualMouse = Mouse.current;
        position = new Vector2(Screen.width / 2f, Screen.height / 2f);

        InputState.Change(virtualMouse.position, position);
    }

    void Update()
    {
        if (Gamepad.current == null) return;

        // Movimiento
        Vector2 input = Gamepad.current.rightStick.ReadValue();
        position += input * speed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, 0, Screen.width);
        position.y = Mathf.Clamp(position.y, 0, Screen.height);

        InputState.Change(virtualMouse.position, position);

        if (cursorGraphic != null)
            cursorGraphic.position = position;

        // Clic con botón A
        bool pressed = Gamepad.current.buttonSouth.isPressed;

        if (pressed)
            InputSystem.QueueStateEvent(Mouse.current, new MouseState { buttons = 1 });
        else
            InputSystem.QueueStateEvent(Mouse.current, new MouseState { buttons = 0 });

        InputSystem.Update();
    }
}