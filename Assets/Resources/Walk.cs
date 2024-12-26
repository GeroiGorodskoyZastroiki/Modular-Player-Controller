using UnityEngine;

public class Walk : MonoBehaviour, IMoveChangeSpeed
{
    private bool _isChangingSpeed = false;
    public bool IsChangingSpeed
    {
        get { return _isChangingSpeed; }
        set 
        {
            _isChangingSpeed = value;
            if (value == true) move.Speed.AddProcessor(ChangeSpeed, 0);
            else move.Speed.RemoveProcessor(ChangeSpeed);
        }
    }
    [SerializeField] private float walkSpeed = 100f;

    Move move;

    private void Start() 
    {
        move = GetComponent<Move>();
    }

    public void ChangeSpeed(ref float speed) => speed += walkSpeed;
}