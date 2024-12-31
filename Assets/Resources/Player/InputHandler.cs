using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private ReactiveProperty<Vector3> _moveDirection = new ReactiveProperty<Vector3>();

    private Move _move;
    private Walk _walk;
    private Run _run;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _walk = GetComponent<Walk>();
        _run = GetComponent<Run>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _moveDirection.Value = new Vector3(input.x, 0, input.y);

        if (context.started) 
        {
            _move.Direction.AddProcessor(ChangeDirection, 0);
            object[] reactiveProperties = new object[]{ _moveDirection };
            _move.Direction.AddSubscriptionsToReactiveProperties(ChangeDirection, reactiveProperties);

            _walk.IsWalking.Value = true;
        }
        if (context.canceled) 
        {
            _move.Direction.RemoveProcessor(ChangeDirection);
            _walk.IsWalking.Value = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
            _run.IsRunning.AddProcessor(ChangeIsRunning, 0);
        if (context.canceled)
            _run.IsRunning.RemoveProcessor(ChangeIsRunning);
    }

    public void ChangeDirection (ref Vector3 value) => value += _moveDirection.Value;

    public void ChangeIsRunning (ref bool value) => value = true; //не совсем правильно так делать
}