using System.Collections.Generic;
using R3;

public enum ProcessValueMode
{
    OnGet,
    OnAddRemoveProcessors,
    OnBind
}

public class Processed<T>
{
    public ProcessValueMode ProcessValueMode;
    public T BaseValue;
    private T _value;
    public T Value 
    { 
        get
        {
            if (ProcessValueMode == ProcessValueMode.OnGet) ProcessValue();
            return _value;
        }
        private set => _value = value;
    }
    public ReactiveProperty<List<Processor<T>>> Processors = new ReactiveProperty<List<Processor<T>>>();
    //public List<Processor<T>> Processors { get; private set; }
    public delegate void Processor<T>(ref T value);

    public Processed(T initialValue, ProcessValueMode processValueMode)
    {
        ProcessValueMode = processValueMode;
        BaseValue = initialValue;
        _value = initialValue;
        Processors.DistinctUntilChanged().Subscribe(_ => ProcessValue());
    }

    public Processed(T initialValue)
    {
        ProcessValueMode = ProcessValueMode.OnGet;
        BaseValue = initialValue;
        _value = initialValue;
        Processors.DistinctUntilChanged().Subscribe(_ => ProcessValue());
    }

    // public void AddProcessor(Processor<T> processor)
    // {
    //     if (Processors.Value.Contains(processor)) return;
    //     Processors.Value.Add(processor);
    //     ProcessValue();
    // }

    // public void RemoveProcessor(Processor<T> processor)
    // {
    //     Processors.Value.Remove(processor);
    //     ProcessValue();
    // }

    public void ProcessOnChange<P>(ReactiveProperty<P> value) => value.Subscribe(_ => ProcessValue()); //надо объединять процессоры и биндинги, к одному процессору может быть много биндингов
//Сделать шире. Надо как-то выделить всё после чего надо делать ProcessValue() и сохранять это в List
    //public bool ContainsProcessor(Processor<T> processor) => Processors.Contains(processor);

    private void ProcessValue()
    {
        _value = BaseValue;
        if (Processors.Value.Count == 0) return; //фикс
        foreach (var processor in Processors.Value)
            processor(ref _value);
    } 
}