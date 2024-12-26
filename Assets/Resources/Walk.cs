using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using R3;

public class Walk : MonoBehaviour//, IChangeSpeed
{
    private bool _isChangingSpeed = false;
    public bool isChangingSpeed
    {
        get { return _isChangingSpeed; }
        set 
        {
            _isChangingSpeed = value;
            if (value == true) move.Speed.Processors.Value.Add(ChangeSpeed);//.AddProcessor(ChangeSpeed);
            else move.Speed.Processors.Value.Remove(ChangeSpeed);//.RemoveProcessor(ChangeSpeed);
        }
    }

    public ReactiveProperty<bool> reactive = new ReactiveProperty<bool>();

    public float Speed = 100f;
    Move move;

    private void Start() 
    {
        //reactive.Subscribe(_ => ) //передавать подписку в сам processed
        move = GetComponent<Move>();
    }

    public void ChangeSpeed(ref float speed) => speed += Speed;
}