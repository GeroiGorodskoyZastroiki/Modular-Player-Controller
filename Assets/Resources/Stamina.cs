using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class Stamina : MonoBehaviour, IStaminaChangeValue
{
    //private Scaled<SerializableReactiveProperty<float>> _stamina = new Scaled<SerializableReactiveProperty<float>>(0f, 0f, 100f);
    public Scaled<SerializableReactiveProperty<float>> stamina //стоит реально вернуть интерефейс
    {
        get => stamina; 
        set
        {
            //_stamina = value;
            if (value.Value.Value < staminaBreak) move.Speed.AddProcessor((ref float speed) => speed = 0, 100);
            else move.Speed.RemoveProcessor((ref float speed) => speed = 0);
        }
    } 
    public float staminaBreak = 1f;
    [SerializeField] private float _staminaIncSpeed;
    Move move;

    private void Awake() 
    {
        move = GetComponent<Move>();
    }

    private void Update() 
    {
        ChangeStamina();
    }

    public void ChangeStamina() => stamina.Value.Value = Mathf.Clamp(stamina.Value.Value + _staminaIncSpeed * Time.deltaTime, 0, stamina.MaxValue.Value);
}
