using UnityEngine;
using System;
using System.Collections.Generic;
using R3;
using System.Linq;

public delegate void Processor<T>(ref T value);

public class ReactiveProcessor<T> : IDisposable
{
    public Processor<T> Processor;
    private DisposableBag Subscriptions = new DisposableBag();
    public List<Processed<T>> Processeds = new List<Processed<T>>();
    
    public ReactiveProcessor(Processor<T> processor, IEnumerable<object> reactiveProperties)
    {
        Processor = processor;
        AddSubscriptionsToReactiveProperties(reactiveProperties);
    }

    public void Dispose()
    {
        foreach (var processed in Processeds)
            processed.RemoveProcessor(this);
        Subscriptions.Dispose();
    }

    ~ReactiveProcessor() => Dispose(); //проверить

    public void AddSubscriptionToReactiveProperty<R>(ref ReactiveProperty<R> changingValue) =>
        Subscriptions.Add(changingValue.Subscribe(value => ProcessValue()));

    public void AddSubscriptionsToReactiveProperties(IEnumerable<object> reactiveProperties)
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

                method.Invoke(this, new object[] { item });
            }
            else Debug.LogError($"Array element at {i} is skipped because it's not ReactiveProperty<> or its descendant.");
        }
    }

    private bool IsTypeReactiveProperty(Type type) =>
        (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ReactiveProperty<>)) || 
        (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>));

    void ProcessValue()
    {
        foreach (var processed in Processeds)
            processed.ProcessValue();
    }
}

[System.Serializable]
public class Processed<T> : IDisposable
{
    public bool ProcessOnGet;
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
    [SerializeReference] private SortedList<int, List<ReactiveProcessor<T>>> Processors = new SortedList<int, List<ReactiveProcessor<T>>>();
    
    public Processed(T initialValue, bool processOnGet = false)
    {
        ProcessOnGet = processOnGet;
        BaseValue = initialValue;
        _value = initialValue;
    }

    public Processed() {} //не удалять, в unity вызывается пустой конструктор

    ~Processed() => Dispose();//проверить как работает

    public void Dispose()
    {
        foreach (var processorList in Processors.Values)
            foreach (var processor in processorList)
                processor.Processeds.Remove(this);
    }

    public void AddProcessor(ReactiveProcessor<T> processor, int priority)
    {
        processor.Processeds.Add(this);

        if (Processors.ContainsKey(priority)) 
            Processors[priority].Add(processor);
        else 
            Processors.Add(priority, new List<ReactiveProcessor<T>>(){processor});

        ProcessValue();
    }

    public void RemoveProcessor(ReactiveProcessor<T> processor)
    {
        foreach (var processorList in Processors.Values)
            if (processorList.Contains(processor)) 
                processorList.Remove(processor);    
    }

    public void RemoveProcessor(ReactiveProcessor<T> processor, int priority)
    {
        if (Processors[priority].FirstOrDefault(x => x == processor) == null) 
        {
            Debug.LogWarning("Can't find processor to remove.");    
            return;
        }
        else 
            Processors[priority].Remove(processor);

        if (!ContainsProcessor(processor)) 
            processor.Processeds.Remove(this);

        ProcessValue();            
    }

    private bool ContainsProcessor(ReactiveProcessor<T> processor) =>
        Processors.Any(x => x.Value.Contains(processor) == true);

    public virtual void ProcessValue()
    {
        _value = BaseValue;
        foreach (var priorityList in Processors)
            foreach (var processor in priorityList.Value)
                processor.Processor(ref _value);
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