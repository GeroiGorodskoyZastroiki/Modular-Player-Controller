using UnityEngine;
using System;
using System.Collections.Generic;
using R3;

public delegate void Processor<T>(ref T value);

[System.Serializable]
public class Processed<T>
{
    public bool ProcessOnGet = false;
    [SerializeReference] public T BaseValue;
    [SerializeReference] protected T _value;
    public T Value 
    { 
        get
        {
            if (ProcessOnGet) ProcessValue();
            return _value;
        }
    }
    [SerializeReference] private SortedList<int, List<Processor<T>>> Processors = new SortedList<int, List<Processor<T>>>();
    private Dictionary<Processor<T>, List<IDisposable>> ProcessorDisposables = new Dictionary<Processor<T>, List<IDisposable>>();
    
    public Processed(T initialValue, bool processOnGet = false)
    {
        ProcessOnGet = processOnGet;
        BaseValue = initialValue;
        _value = initialValue;
    }

    public Processed() {} //нужен (видимо единственный вид конструктора, который срабатывает в редакторе?)

    public void AddProcessor(Processor<T> processor, int priority)
    {
        if (processor.Method.Name.Contains("b__")) 
        {
            Debug.LogError("Processor was not added. Only named not anonymous methods are supported.");
            return;
        }

        if (ContainsProcessor(processor))
        {
            Debug.LogWarning("Equal proccessors can not be added. For now...");
            return;
        }

        if (Processors.ContainsKey(priority)) 
            Processors[priority].Add(processor);
        else 
            Processors.Add(priority, new List<Processor<T>>(){processor});

        ProcessValue();
    }

    public void RemoveProcessor(Processor<T> processor)
    {
        foreach (var processorList in Processors.Values)
        {
            if (processorList.Contains(processor))
            {
                processorList.Remove(processor);
                if (ProcessorDisposables.ContainsKey(processor))
                {
                    foreach (var disposable in ProcessorDisposables[processor])
                        disposable.Dispose();
                    ProcessorDisposables.Remove(processor);
                }
                ProcessValue();
                return;
            }
        }
        Debug.LogWarning("Can't find processor to remove. This might happen when RemoveProcessor() called in subscription where it was not added at first. Normal at startup.");        
    }

    public bool ContainsProcessor(Processor<T> processor)
    {
        foreach (var processorList in Processors.Values)
            if (processorList.Contains(processor))
                return true;
        return false;
    }

    public void AddDisposableToProcessor(Processor<T> processor, IDisposable disposable)
    {
        if (ProcessorDisposables.ContainsKey(processor))
            ProcessorDisposables[processor].Add(disposable);
        else 
            ProcessorDisposables.Add(processor, new List<IDisposable>(){disposable});
    }

    public virtual void ProcessValue()
    {
        _value = BaseValue;
        foreach (var priorityList in Processors)
            foreach (var processor in priorityList.Value)
                processor(ref _value);
    } 
}

[System.Serializable]
public class ReactiveProcessed<T> : Processed<T>
{
    private ReactiveProperty<T> _reactiveValue = new ReactiveProperty<T>();
    public new ReactiveProperty<T> Value
    {
        get
        {
            if (ProcessOnGet) ProcessValue();
            return _reactiveValue;
        }
    }

    public ReactiveProcessed(T initialValue, bool processOnGet = false) : base(initialValue, processOnGet)
    {
        _reactiveValue = new ReactiveProperty<T>(initialValue);
    }

    public override void ProcessValue()
    {
        base.ProcessValue();
        _reactiveValue.Value = _value;
    } 
}