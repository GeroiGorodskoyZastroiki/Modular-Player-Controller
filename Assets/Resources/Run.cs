using UnityEngine;
using R3;

public class Run : MonoBehaviour, IMoveChangeSpeed, IStaminaChangeValue
{
    private bool _isChangingSpeed = false;
    public bool IsChangingSpeed
    {
        get { return _isChangingSpeed; }
        set 
        {
            _isChangingSpeed = value;
            if (value == true) move.Speed.AddProcessor(ChangeSpeed, 1);
            else move.Speed.RemoveProcessor(ChangeSpeed);
        }
    }
    [SerializeField] private float _speedFactor = 2f;
    public Processed<float> StaminaDecSpeed = new Processed<float>();

    Move move;
    Stamina stamina;

    private void Awake()
    {
        move = GetComponent<Move>();
        stamina = GetComponent<Stamina>();
    }

    private void Update() 
    {
        ChangeStamina();
    }

    public void ChangeSpeed(ref float speed) => speed *= _speedFactor;

    public void ChangeStamina() => stamina.stamina.Value.Value = Mathf.Clamp(stamina.stamina.Value.Value - (IsChangingSpeed ? StaminaDecSpeed.Value : 0f) * Time.deltaTime, stamina.stamina.MinValue.Value, stamina.stamina.MaxValue.Value);
}