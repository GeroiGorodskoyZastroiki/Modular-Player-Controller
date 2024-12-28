using R3;
using UnityEngine;

public class Stamina : MonoBehaviour, IScale<SerializableReactiveProperty<float>>, IStaminaChangeValue
{
    [field: SerializeField] public SerializableReactiveProperty<float> MinValue { get; set; }
    [field: SerializeField] public SerializableReactiveProperty<float> Value { get; set; }
    [field: SerializeField] public SerializableReactiveProperty<float> MaxValue { get; set; }
    [SerializeField] private float _staminaIncSpeed;

    private void Update() 
    {
        ChangeStamina();
    }

    public void ChangeStamina() => Value.Value = Mathf.Clamp(Value.Value + _staminaIncSpeed * Time.deltaTime, 0, MaxValue.Value);
}
