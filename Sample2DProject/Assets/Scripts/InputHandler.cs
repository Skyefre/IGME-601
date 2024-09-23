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
        Spell,
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
        {Inputs.Spell, InputState.UnPressed},
        {Inputs.Shield, InputState.UnPressed},
        {Inputs.Pause, InputState.UnPressed},
    };

    public Dictionary<Inputs, InputState> prevInputs = new Dictionary<Inputs, InputState>()
    {
        {Inputs.Left, InputState.UnPressed},
        {Inputs.Right, InputState.UnPressed},
        {Inputs.Jump, InputState.UnPressed},
        {Inputs.Attack, InputState.UnPressed},
        {Inputs.Spell, InputState.UnPressed},
        {Inputs.Shield, InputState.UnPressed},
        {Inputs.Pause, InputState.UnPressed},
    };

    public void UpdateMovement(InputAction.CallbackContext context)
    {
        Vector2 movementVector = context.ReadValue<Vector2>();

        //set left and right inputs
        if (movementVector.x < 0)
        {
            keyBindings[Inputs.Left] = InputState.Held;
            keyBindings[Inputs.Right] = InputState.UnPressed;
        }
        else if (movementVector.x > 0)
        {
            keyBindings[Inputs.Right] = InputState.Held;
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
            keyBindings[Inputs.Down] = InputState.Held;
            keyBindings[Inputs.Up] = InputState.UnPressed;
        }
        else if (movementVector.y > 0)
        {
            keyBindings[Inputs.Up] = InputState.Held;
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
                if (context.performed)
                {
                    if (prevInputs[Inputs.Jump] == InputState.UnPressed || prevInputs[Inputs.Jump] == InputState.Released)
                    {
                        keyBindings[Inputs.Jump] = InputState.Pressed;
                    }
                    else
                    {
                        keyBindings[Inputs.Jump] = InputState.Held;
                    }

                }
                else
                {
                    if (prevInputs[Inputs.Jump] == InputState.Pressed || prevInputs[Inputs.Jump] == InputState.Held)
                    {
                        keyBindings[Inputs.Jump] = InputState.Released;
                    }
                    else
                    {
                        keyBindings[Inputs.Jump] = InputState.UnPressed;
                    }
                }
                break;
            case "Attack":
                if (context.performed)
                {
                    if (prevInputs[Inputs.Attack] == InputState.UnPressed || prevInputs[Inputs.Attack] == InputState.Released)
                    {
                        keyBindings[Inputs.Attack] = InputState.Pressed;
                    }
                    else
                    {
                        keyBindings[Inputs.Attack] = InputState.Held;
                    }

                }
                else
                {
                    if (prevInputs[Inputs.Attack] == InputState.Pressed || prevInputs[Inputs.Attack] == InputState.Held)
                    {
                        keyBindings[Inputs.Attack] = InputState.Released;
                    }
                    else
                    {
                        keyBindings[Inputs.Attack] = InputState.UnPressed;
                    }
                }
                break;
            case "Spell":
                if (context.performed)
                {
                    if (prevInputs[Inputs.Spell] == InputState.UnPressed || prevInputs[Inputs.Spell] == InputState.Released)
                    {
                        keyBindings[Inputs.Spell] = InputState.Pressed;
                    }
                    else
                    {
                        keyBindings[Inputs.Spell] = InputState.Held;
                    }

                }
                else
                {
                    if (prevInputs[Inputs.Spell] == InputState.Pressed || prevInputs[Inputs.Spell] == InputState.Held)
                    {
                        keyBindings[Inputs.Spell] = InputState.Released;
                    }
                    else
                    {
                        keyBindings[Inputs.Spell] = InputState.UnPressed;
                    }
                }
                break;
            case "Shield":
                if (context.performed)
                {
                    if (prevInputs[Inputs.Shield] == InputState.UnPressed || prevInputs[Inputs.Shield] == InputState.Released)
                    {
                        keyBindings[Inputs.Shield] = InputState.Pressed;
                    }
                    else
                    {
                        keyBindings[Inputs.Shield] = InputState.Held;
                    }

                }
                else
                {
                    if (prevInputs[Inputs.Shield] == InputState.Pressed || prevInputs[Inputs.Shield] == InputState.Held)
                    {
                        keyBindings[Inputs.Shield] = InputState.Released;
                    }
                    else
                    {
                        keyBindings[Inputs.Shield] = InputState.UnPressed;
                    }
                }
                break;
            case "Pause":
                if (context.performed)
                {
                    if (prevInputs[Inputs.Pause] == InputState.UnPressed || prevInputs[Inputs.Pause] == InputState.Released)
                    {
                        keyBindings[Inputs.Pause] = InputState.Pressed;
                    }
                    else
                    {
                        keyBindings[Inputs.Pause] = InputState.Held;
                    }

                }
                else
                {
                    if (prevInputs[Inputs.Pause] == InputState.Pressed || prevInputs[Inputs.Pause] == InputState.Held)
                    {
                        keyBindings[Inputs.Pause] = InputState.Released;
                    }
                    else
                    {
                        keyBindings[Inputs.Pause] = InputState.UnPressed;
                    }
                }
                break;
        }
    }

    public void FixedUpdate()
    {
        List<Inputs> keys = new List<Inputs>(keyBindings.Keys);
        foreach (Inputs key in keys)
        {
            prevInputs[key] = keyBindings[key];
            if (keyBindings[key] == InputState.Pressed)
            {
                keyBindings[key] = InputState.Held;
            }
            if (keyBindings[key] == InputState.Released)
            {
                keyBindings[key] = InputState.UnPressed;
            }
        }
    }
}
