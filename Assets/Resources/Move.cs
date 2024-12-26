using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Processed<Vector3> Direction = new Processed<Vector3>(Vector3.zero);
    public Processed<float> Speed = new Processed<float>(0);

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