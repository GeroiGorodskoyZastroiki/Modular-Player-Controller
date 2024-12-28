using UnityEngine;
using R3;

public class Run : MonoBehaviour, IStaminaChangeValue //IMoveChangeSpeed
{
    public ReactiveProcessed<bool> IsRunning = new ReactiveProcessed<bool>(false);
    public ReactiveProcessed<bool> CanRun = new ReactiveProcessed<bool>(true);
    [SerializeField] private float _runSpeedFactor;
    public Processed<float> StaminaDecSpeed;
    [SerializeField] private float _staminaToStop = 1f;
    [SerializeField] private float _staminaForRun = 10f;

    private Move _move;
    private Stamina _stamina;
    private Walk _walk;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _stamina = GetComponent<Stamina>();
        _walk = GetComponent<Walk>();

        CanRunOnIsWalking();
        CanRunOnStamina();
        IsRunningOnCanRun();
        MoveSpeedOnIsRunning();
    }

    private void Update() 
    {
        ChangeStamina();
    }

    private void CanRunOnIsWalking()
    {
        void ChangeCanRun(ref bool value) => value = false;

        _walk.IsWalking.Subscribe(value => //это Subscribe style Processor, подходит для случаев, когда нужно именно удалять и добавлять процессор, а не пересчитывать при изменении
        {
            if (value == false) CanRun.AddProcessor(ChangeCanRun, 0);
            else CanRun.RemoveProcessor(ChangeCanRun);
        }).AddTo(this);
    }

    private void CanRunOnStamina()
    {
        void ChangeCanRunOnStamina(ref bool value) => value = (_stamina.Value.Value > _staminaForRun) || (IsRunning.Value.Value && _stamina.Value.Value > _staminaToStop);

        CanRun.AddProcessor(ChangeCanRunOnStamina, 1); //(это Manual style Processor)
        CanRun.AddDisposableToProcessor(ChangeCanRunOnStamina, _stamina.Value.Subscribe(_ => CanRun.ProcessValue()).AddTo(this));
    }

    private void IsRunningOnCanRun()
    {
        void ChangeIsRunning(ref bool value) => value = false; //неправильно использовать фиксированные значения, но ладно

        CanRun.Value.Subscribe(value =>
        {
            if (value == false) IsRunning.AddProcessor(ChangeIsRunning, 1);
            else IsRunning.RemoveProcessor(ChangeIsRunning);
        }).AddTo(this);
    }

    private void MoveSpeedOnIsRunning()
    {
        void ChangeSpeed(ref float speed) => speed *= _runSpeedFactor;

        IsRunning.Value.Subscribe(value => 
        {
            if (value == true) _move.Speed.AddProcessor(ChangeSpeed, 1);
            else _move.Speed.RemoveProcessor(ChangeSpeed);
        }).AddTo(this);
    }

    public void ChangeStamina() => _stamina.Value.Value = Mathf.Clamp(_stamina.Value.Value - (IsRunning.Value.Value ? StaminaDecSpeed.Value : 0f) * Time.deltaTime, _stamina.MinValue.Value, _stamina.MaxValue.Value);
}