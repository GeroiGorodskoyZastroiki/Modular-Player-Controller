using UnityEngine;
using R3;

public class Walk : MonoBehaviour
{
    public SerializableReactiveProperty<bool> IsWalking;
    [SerializeField] private float _walkSpeed;

    Move _move;

    private void Awake() 
    {
        _move = GetComponent<Move>();

        MoveSpeedOnIsWalking();
    }

    private void MoveSpeedOnIsWalking()
    {
        void ChangeSpeed(ref float speed) => speed += _walkSpeed;

        IsWalking.Subscribe(value =>
        {
            if (value == true) _move.Speed.AddProcessor(ChangeSpeed, 0);
            else _move.Speed.RemoveProcessor(ChangeSpeed);
        }).AddTo(this);
    }
}