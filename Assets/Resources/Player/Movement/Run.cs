using UnityEngine;
using R3;
using System.Collections.Generic;

public class Run : MonoBehaviour
{
    public ReactiveProcessed<bool> IsRunning = new ReactiveProcessed<bool>(false);
    public ReactiveProcessed<bool> CanRun = new ReactiveProcessed<bool>(true);
    [SerializeField] private float _runSpeedFactor;
    public Processed<float> StaminaDecSpeed;
    [SerializeField] private float _staminaToStop;
    [SerializeField] private float _staminaForRun;
    ReactiveProcessor<bool> _canRunOnIsWalking;
    ReactiveProcessor<bool> _canRunOnStamina;
    ReactiveProcessor<bool> _isRunningOnCanRun;
    ReactiveProcessor<float> _moveSpeedOnIsRunning;

    private Move _move;
    private Stamina _stamina;
    private Walk _walk;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _stamina = GetComponent<Stamina>();
        _walk = GetComponent<Walk>();

        RegisterCanRunOnIsWalking();
        RegisterCanRunOnStamina();
        RegisterIsRunningOnCanRun();
        RegisterMoveSpeedOnIsRunning();
    }

    private void Update() 
    {
        ChangeStamina();
    }

    private void RegisterCanRunOnIsWalking()
    {
        //void ChangeCanRun(ref bool value) => value = false;

        _canRunOnIsWalking ??= new ReactiveProcessor<bool>((ref bool value) => value = _walk.IsWalking.Value, new object[] { _walk.IsWalking });
        CanRun.AddProcessor(_canRunOnIsWalking, 0);
    }

    private void RegisterCanRunOnStamina()
    {
        //void ChangeCanRunOnStamina(ref bool value) => value = (_stamina.Value.Value > _staminaForRun) || (IsRunning.Value.Value && _stamina.Value.Value > _staminaToStop);

        _canRunOnStamina ??= new ReactiveProcessor<bool>((ref bool value) => value = (_stamina.Value.Value > _staminaForRun) || (IsRunning.Value.Value && _stamina.Value.Value > _staminaToStop), new object[] { _walk.IsWalking, _stamina.Value });
        CanRun.AddProcessor(_canRunOnStamina, 1);
    }

    private void RegisterIsRunningOnCanRun()
    {
        _isRunningOnCanRun ??= new ReactiveProcessor<bool>((ref bool value) => value = CanRun.Value.Value ? value : false, new object[] { CanRun.Value });
        IsRunning.AddProcessor(_isRunningOnCanRun, 1);

        //void ChangeIsRunning(ref bool value) => value = false;
    }

    private void RegisterMoveSpeedOnIsRunning()
    {
        _moveSpeedOnIsRunning ??= new ReactiveProcessor<float>((ref float speed) => speed *= IsRunning.Value.Value ? _runSpeedFactor : 1, new object[] { IsRunning.Value });
        _move.Speed.AddProcessor(_moveSpeedOnIsRunning, 1);

        // void ChangeSpeed(ref float speed) => speed *= _runSpeedFactor;
    }

    public void ChangeStamina() => _stamina.Value.Value = Mathf.Clamp(_stamina.Value.Value - (IsRunning.Value.Value ? StaminaDecSpeed.Value : 0f) * Time.deltaTime, _stamina.MinValue.Value, _stamina.MaxValue.Value);
}