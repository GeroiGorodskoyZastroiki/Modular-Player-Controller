using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Move move;
    private Walk walk;
    private ReactiveProperty<Vector3> moveDirection = new ReactiveProperty<Vector3>();
    private ReactiveProperty<Vector2> inputDirection = new ReactiveProperty<Vector2>();

    private void Awake()
    {
        move = GetComponent<Move>();
        walk = GetComponent<Walk>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveDirection.Value = new Vector3(input.x, 0, input.y);
        inputDirection.Value = input;

        if (context.started) 
        {
            move.Direction.Processors.Value.Add(ChangeDirection);// .AddProcessor(ChangeDirection); //if (!move.Direction.ContainsProcessor(ChangeDirection))
            move.Direction.ProcessOnChange(inputDirection);
            walk.isChangingSpeed = true;
        }
        if (context.canceled) 
        {
            move.Direction.Processors.Value.Remove(ChangeDirection);//move.Direction.RemoveProcessor(ChangeDirection);
            walk.isChangingSpeed = false;
        }
    }
    
    void Update()
    {
        //Debug.Log(moveDirection);
    }

    public void ChangeDirection (ref Vector3 value)
    {
        Debug.Log(moveDirection);
        value += moveDirection.Value;
    }
}