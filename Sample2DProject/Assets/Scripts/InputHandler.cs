using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputHandler : MonoBehaviour
{
    public enum Inputs
    {
        Left,
        Right,
        Up,
        Down,
        Jump,
        Attack,
        Grapple,
        Shield,
        Pause,
    }
    public enum InputState
    {
        UnPressed,
        Pressed,
        Held,
        Released,
    }
    public Dictionary<Inputs, InputState> keyBindings = new Dictionary<Inputs, InputState>()
    {
        {Inputs.Left, InputState.UnPressed},
        {Inputs.Right, InputState.UnPressed},
        {Inputs.Jump, InputState.UnPressed},
        {Inputs.Attack, InputState.UnPressed},
        {Inputs.Grapple, InputState.UnPressed},
        {Inputs.Shield, InputState.UnPressed},
        {Inputs.Pause, InputState.UnPressed},
    };

    public void UpdateMovement(InputAction.CallbackContext context)
    {
        Vector2 movementVector = context.ReadValue<Vector2>();

        //set left and right inputs
        if (movementVector.x < 0)
        {
            keyBindings[Inputs.Left] = InputState.Pressed;
            keyBindings[Inputs.Right] = InputState.UnPressed;
        }
        else if (movementVector.x > 0)
        {
            keyBindings[Inputs.Right] = InputState.Pressed;
            keyBindings[Inputs.Left] = InputState.UnPressed;
        }
        else
        {
            keyBindings[Inputs.Right] = InputState.UnPressed;
            keyBindings[Inputs.Left] = InputState.UnPressed;
        }

        //set up and down inputs
        if (movementVector.y < 0)
        {
            keyBindings[Inputs.Down] = InputState.Pressed;
            keyBindings[Inputs.Up] = InputState.UnPressed;
        }
        else if (movementVector.y > 0)
        {
            keyBindings[Inputs.Up] = InputState.Pressed;
            keyBindings[Inputs.Down] = InputState.UnPressed;
        }
        else
        {
            keyBindings[Inputs.Up] = InputState.UnPressed;
            keyBindings[Inputs.Down] = InputState.UnPressed;
        }


    }
    public void UpdateButton(InputAction.CallbackContext context)
    {
        InputAction button = context.action;
        //Debug.Log("buttonDisplay: " + button.name);

        //set button input states
        switch (button.name)
        {
            case "Jump":
                if (context.started)
                {
                    keyBindings[Inputs.Jump] = InputState.Pressed;
                }
                else if (context.canceled)
                {
                    keyBindings[Inputs.Jump] = InputState.Released;
                }
                else if (context.performed)
                {
                    keyBindings[Inputs.Jump] = InputState.Held;
                }
                else
                {
                    keyBindings[Inputs.Jump] = InputState.UnPressed;
                }
                break;
            case "Attack":
                if (context.started)
                {
                    keyBindings[Inputs.Attack] = InputState.Pressed;
                }
                else if (context.canceled)
                {
                    keyBindings[Inputs.Attack] = InputState.Released;
                }
                else if (context.performed)
                {
                    keyBindings[Inputs.Attack] = InputState.Held;
                }
                else
                {
                    keyBindings[Inputs.Attack] = InputState.UnPressed;
                }
                break;
            case "Grapple":
                if (context.started)
                {
                    keyBindings[Inputs.Grapple] = InputState.Pressed;
                }
                else if (context.canceled)
                {
                    keyBindings[Inputs.Grapple] = InputState.Released;
                }
                else if (context.performed)
                {
                    keyBindings[Inputs.Grapple] = InputState.Held;
                }
                else
                {
                    keyBindings[Inputs.Grapple] = InputState.UnPressed;
                }
                break;
            case "Shield":
                if (context.started)
                {
                    keyBindings[Inputs.Shield] = InputState.Pressed;
                }
                else if (context.canceled)
                {
                    keyBindings[Inputs.Shield] = InputState.Released;
                }
                else if (context.performed)
                {
                    keyBindings[Inputs.Shield] = InputState.Held;
                }
                else
                {
                    keyBindings[Inputs.Shield] = InputState.UnPressed;
                }
                break;
            case "Pause":
                if (context.started)
                {
                    keyBindings[Inputs.Pause] = InputState.Pressed;
                }
                else if (context.canceled)
                {
                    keyBindings[Inputs.Pause] = InputState.Released;
                }
                else if (context.performed)
                {
                    keyBindings[Inputs.Pause] = InputState.Held;
                }
                else
                {
                    keyBindings[Inputs.Pause] = InputState.UnPressed;
                }
                break;
        }
    }

}
