using UnityEngine;
using R3;

public class Walk : MonoBehaviour
{
    public SerializableReactiveProperty<bool> IsWalking;
    [SerializeField] private float _walkSpeed;
    ReactiveProcessor<float> _moveSpeedOnIsWalking;

    Move _move;

    private void Awake() 
    {
        _move = GetComponent<Move>();
    }

    private void OnEnable() 
    {
        RegisterMoveSpeedOnIsWalking();
    }

    private void RegisterMoveSpeedOnIsWalking()
    {
        //void ChangeSpeed(ref float speed) => speed += _walkSpeed;

        _moveSpeedOnIsWalking ??= new ReactiveProcessor<float>((ref float speed) => speed += IsWalking.Value ? _walkSpeed : 0, new object[] { IsWalking });
        _move.Speed.AddProcessor(_moveSpeedOnIsWalking, 0);
    }

    private void OnDestroy() //автоматизировать как-то
    {
        _moveSpeedOnIsWalking.Dispose();
    }

    private void OnDisable() 
    {
        _moveSpeedOnIsWalking.Dispose();
    }
}