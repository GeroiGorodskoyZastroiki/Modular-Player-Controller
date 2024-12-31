using R3;
using UnityEngine;

public class Stamina : MonoBehaviour //, IScale<SerializableReactiveProperty<float>>
{
    public SerializableReactiveProperty<float> MinValue;
    public SerializableReactiveProperty<float> Value;
    public SerializableReactiveProperty<float> MaxValue;
    [SerializeField] private float _staminaIncSpeed;

    private void Update() 
    {
        ChangeStamina();
    }

    public void ChangeStamina() => Value.Value = Mathf.Clamp(Value.Value + _staminaIncSpeed * Time.deltaTime, 0, MaxValue.Value);
}
