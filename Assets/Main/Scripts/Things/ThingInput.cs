using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThingInput : GameThing
{
    public Vector2 movement
    {
        get => _movement;
        set
        {
            _movement = value;
            
            _moving = value.magnitude > 0;

            if (attachedThing.thing != null)
                attachedThing.thing.SendMessage("Move", _movement, SendMessageOptions.DontRequireReceiver);
        }
    }
    private Vector2 _movement;
    private bool _moving;

    public bool primaryAction
    {
        get => _primaryAction;
        set
        {
            _primaryAction = value;
            
            if (attachedThing.thing != null)
                attachedThing.thing.SendMessage("PrimaryAction", _primaryAction, SendMessageOptions.DontRequireReceiver);
        }
    }
    private bool _primaryAction;

    public bool secondaryAction
    {
        get => _secondaryAction;
        set
        {
            _secondaryAction = value;
            
            if (attachedThing.thing != null)
                attachedThing.thing.SendMessage("SecondaryAction", _secondaryAction, SendMessageOptions.DontRequireReceiver);
        }
    }
    private bool _secondaryAction;

    public void Move(InputValue value)
    {
        movement = value.Get<Vector2>();
    }
    
    public void PrimaryAction(InputValue value)
    {
        primaryAction = value.isPressed;
    }

    public void SecondaryAction(InputValue value)
    {
        secondaryAction = value.isPressed;
    }
}
