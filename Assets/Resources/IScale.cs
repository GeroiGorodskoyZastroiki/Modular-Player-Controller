public interface IScale<T>
{
    T MinValue { get; }
    T Value { get; }
    T MaxValue { get; }
}

[System.Serializable]
public class Scaled<T>
{
    public T MinValue;
    public T Value;
    public T MaxValue;

    public Scaled(T min, T current, T max)
    {
        MinValue = min;
        Value = current;
        MaxValue = max;
    }

    public Scaled()
    {
        MinValue = default;
        Value = default;
        MaxValue = default;
    }
}
