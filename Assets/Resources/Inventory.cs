using System;
using R3;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Scaled<SerializableReactiveProperty<float>> mass;
    Run run;

    private void Awake() 
    {
        run = GetComponent<Run>();
        Processor<float> processor = (ref float speed) => speed *= 1 - mass.Value.Value/100;
        run.StaminaDecSpeed.AddProcessor(processor, 0);
        IDisposable disposable = mass.Value.Subscribe(_ => run.StaminaDecSpeed.ProcessValue());
        run.StaminaDecSpeed.AddDisposableToProcessor(processor, disposable);
    }
}