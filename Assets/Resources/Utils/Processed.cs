using UnityEngine;
using System;
using System.Collections.Generic;
using R3;
using System.Linq;

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
    private Dictionary<Processor<T>, DisposableBag> Subscriptions = new Dictionary<Processor<T>, DisposableBag>();
    
    public Processed(T initialValue, bool processOnGet = false)
    {
        ProcessOnGet = processOnGet;
        BaseValue = initialValue;
        _value = initialValue;
    }

    public Processed() {} //не удалять, в unity вызывается пустой конструктор

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
                if (Subscriptions.ContainsKey(processor))
                {
                    Subscriptions[processor].Dispose();
                    Subscriptions.Remove(processor);
                }
                processorList.Remove(processor);
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

    public void AddSubscriptionToReactiveProperty<R>(Processor<T> processor, ref ReactiveProperty<R> changingValue)
    {
        if (!Subscriptions.ContainsKey(processor)) 
            Subscriptions.Add(processor, new DisposableBag());
        Subscriptions[processor].Add(changingValue.Subscribe(value => ProcessValue()));
    }

    public void AddSubscriptionsToReactiveProperties(Processor<T> processor, IEnumerable<object> reactiveProperties)
    {
        for (int i = 0; i < reactiveProperties.Count(); i++)
        {
            var item = reactiveProperties.ElementAt(i);

            if (item == null) 
            {
                Debug.LogError($"Array element at {i} is skipped because it's null.");
                continue;
            }

            Type itemType = item.GetType();

            if (IsTypeReactiveProperty(itemType))
            {
                Type innerType = itemType.GetGenericArguments()[0];

                var method = this.GetType().GetMethod("AddSubscriptionToReactiveProperty")
                                    .MakeGenericMethod(innerType);

                method.Invoke(this, new object[] { processor, item });
            }
            else Debug.LogError($"Array element at {i} is skipped because it's not ReactiveProperty<> or its descendant.");
        }
    }

    private bool IsTypeReactiveProperty(Type type) =>
        (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ReactiveProperty<>)) || 
        (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>));

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

    public ReactiveProcessed() : base() {} 

    public override void ProcessValue()
    {
        base.ProcessValue();
        _reactiveValue.Value = _value;
    }
}