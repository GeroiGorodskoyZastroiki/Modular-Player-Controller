using UnityEngine;
using R3;

public class Run : MonoBehaviour, IMoveChangeSpeed
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
    [SerializeField] private float speedFactor = 6f;

    private Move move;

    private void Awake()
    {
        move = GetComponent<Move>();
    }

    public void ChangeSpeed(ref float speed) => speed *= speedFactor;
}