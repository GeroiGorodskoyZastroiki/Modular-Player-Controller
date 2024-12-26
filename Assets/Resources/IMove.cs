using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveChangeSpeed
{
    void ChangeSpeed(ref float speed);
}

public interface IMoveChangeDirection
{
    void ChangeDirection(ref Vector3 speed);
}
