using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public Processed<float> Energy = new Processed<float>(100f);
}
