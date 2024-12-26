using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, IMoveChangeDirection
{
    private Move move;
    private Walk walk;
    private Run run;
    private ReactiveProperty<Vector3> moveDirection = new ReactiveProperty<Vector3>();

    private void Awake()
    {
        move = GetComponent<Move>();
        walk = GetComponent<Walk>();
        run = GetComponent<Run>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveDirection.Value = new Vector3(input.x, 0, input.y);

        if (context.started) 
        {
            move.Direction.AddProcessor(ChangeDirection, 0);
            move.Direction.AddDisposableToProcessor(ChangeDirection, moveDirection.Subscribe(_ => move.Direction.ProcessValue()));
            walk.IsChangingSpeed = true;
        }
        if (context.canceled) 
        {
            move.Direction.RemoveProcessor(ChangeDirection);
            walk.IsChangingSpeed = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
            run.IsChangingSpeed = true;
        if (context.canceled)
            run.IsChangingSpeed = false;
    }

    public void OnCrouch()
    {

    }

    public void ChangeDirection (ref Vector3 value)
    {
        Debug.Log(moveDirection);
        value += moveDirection.Value;
    }
}