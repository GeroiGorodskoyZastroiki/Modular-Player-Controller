using UnityEngine;

public class Move : MonoBehaviour
{
    public Processed<Vector3> Direction;
    public Processed<float> Speed;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        characterController.Move(Direction.Value * Speed.Value * Time.deltaTime);
    }
}