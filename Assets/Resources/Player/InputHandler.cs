using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private ReactiveProperty<Vector3> _moveDirection = new ReactiveProperty<Vector3>();
    private ReactiveProperty<bool> _isRunnning = new ReactiveProperty<bool>();
    ReactiveProcessor<Vector3> _moveDirectionOnInput;
    ReactiveProcessor<bool> _isRunningOnInput;

    private Move _move;
    private Walk _walk;
    private Run _run;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _walk = GetComponent<Walk>();
        _run = GetComponent<Run>();

        RegisterMoveDirectionOnInput();
        RegisterIsRunningOnInput();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _moveDirection.Value = new Vector3(input.x, 0, input.y);

        if (context.started) 
            _walk.IsWalking.Value = true;
        if (context.canceled) 
            _walk.IsWalking.Value = false;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
            _isRunnning.Value = true;
        if (context.canceled)
            _isRunnning.Value = false;
    }

    void RegisterMoveDirectionOnInput()
    {
        _moveDirectionOnInput ??= new ReactiveProcessor<Vector3>((ref Vector3 value) => value += _moveDirection.Value, new object[] { _moveDirection });
        _move.Direction.AddProcessor(_moveDirectionOnInput, 0);
    }

    void RegisterIsRunningOnInput()
    {
        _isRunningOnInput ??= new ReactiveProcessor<bool>((ref bool value) => value = _isRunnning.Value, new object[] { _isRunnning });
        _run.IsRunning.AddProcessor(_isRunningOnInput, 0);
    }
}