using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThingInput : GameThing
{
    public bool canControl;

    void OnEnable()
    {
        GameManager.AddPlayer(this);
    }

    void OnDisable()
    {
        GameManager.RemovePlayer(this);
    }

    public Vector2 movement
    {
        get => _movement;
        set
        {
            _movement = value;
            
            _moving = value.magnitude > 0;

            if (attachedThing.thing)
                attachedThing.thing.Move(_movement);
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
            
            if (attachedThing.thing)
                attachedThing.thing.PrimaryAction(_primaryAction);
        }
    }
    private bool _primaryAction;

    public bool secondaryAction
    {
        get => _secondaryAction;
        set
        {
            _secondaryAction = value;
            
            if (attachedThing.thing)
                attachedThing.thing.SecondaryAction(_secondaryAction);
        }
    }
    private bool _secondaryAction;

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }
    
    public void OnButton1(InputValue value)
    {
        primaryAction = value.isPressed;
    }

    public void OnButton2(InputValue value)
    {
        secondaryAction = value.isPressed;
    }
}
