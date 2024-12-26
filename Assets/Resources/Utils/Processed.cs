using UnityEngine;
using System;
using System.Collections.Generic;
using R3;
using Sirenix.OdinInspector;

public delegate void Processor<T>(ref T value);

[System.Serializable]
public class Processed<T>
{
    public bool ProcessOnGet = false;
    public T BaseValue;
    [ReadOnly][SerializeField] private T _value;
    public T Value 
    { 
        get
        {
            if (ProcessOnGet) ProcessValue();
            return _value;
        }
        private set => _value = value;
    }
    private ReactiveProperty<SortedList<int, List<Processor<T>>>> Processors = new ReactiveProperty<SortedList<int, List<Processor<T>>>>(new SortedList<int, List<Processor<T>>>());
    private Dictionary<Processor<T>, List<IDisposable>> ProcessorDisposables = new Dictionary<Processor<T>, List<IDisposable>>();
    
    public Processed(T initialValue, bool processOnGet)
    {
        ProcessOnGet = processOnGet;
        BaseValue = initialValue;
        _value = initialValue;
        Processors.DistinctUntilChanged().Subscribe(_ => ProcessValue()); //не работает по какой-то причине (скорее всего не срабатывает, т.к. объект остаётся тот же)
    }

    public Processed(T initialValue)
    {
        BaseValue = initialValue;
        _value = initialValue;
        Processors.DistinctUntilChanged().Subscribe(_ => ProcessValue()); //не работает по какой-то причине
    }

    public Processed()
    {
        BaseValue = default;
        _value = default;
        Processors.DistinctUntilChanged().Subscribe(_ => ProcessValue()); //не работает по какой-то причине
    }

    public void AddProcessor(Processor<T> processor, int priority)
    {
        if (Processors.Value.ContainsKey(priority)) 
            Processors.Value[priority].Add(processor);
        else 
            Processors.Value.Add(priority, new List<Processor<T>>(){processor});

        ProcessValue(); //костыль
    }

    public void RemoveProcessor(Processor<T> processor)
    {
        for (int i = 0; i < Processors.Value.Count; i++)
        {
            if (Processors.Value[i].Contains(processor))
            {
                Processors.Value[i].Remove(processor);
                if (ProcessorDisposables.ContainsKey(processor))
                {
                    foreach (var disposable in ProcessorDisposables[processor])
                        disposable.Dispose();
                    ProcessorDisposables.Remove(processor);
                }

            }
        }

        ProcessValue(); //костыль
    }

    public void AddDisposableToProcessor(Processor<T> processor, IDisposable disposable)
    {
        if (ProcessorDisposables.ContainsKey(processor))
            ProcessorDisposables[processor].Add(disposable);
        else 
            ProcessorDisposables.Add(processor, new List<IDisposable>(){disposable});
    }

    public void ProcessValue()
    {
        _value = BaseValue;
        foreach (var priorityList in Processors.Value)
            foreach (var processor in priorityList.Value)
                processor(ref _value);
    } 
}