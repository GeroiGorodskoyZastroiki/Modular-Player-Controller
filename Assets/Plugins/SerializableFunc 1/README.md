### Serializable Func
A UnityEvent for function calls with a return value. 
Allows you to assign Func\<T\> via the Inspector.
Looks and acts like a UnityEvent.
Supports both GUI and UI Toolkit. Tested with Unity 2020.3, 2021.3, 2022.2, 2023.1.
Tested in standalone Windows and Android builds, both with Mono and IL2CPP.
Version 1.1 supports generic parameters.
If there are no generic parameters for the func, you'll also be able to assign getter properties.

GUI Representation
![unity_inspector](https://i.imgur.com/pR4uo7H.png)

UI Toolkit Representation
![unity_inspector](https://i.imgur.com/tGmKW1m.png)

### Example Usage
```csharp
public class ExampleClass : MonoBehaviour 
{
    [Header("My Bool Func")]
    [SerializeField] private SerializableFunc<bool> boolFunc;
    [SerializeField] private SerializableFunc<int, string> stringFunc;

    private void Start()
    {
        bool result = boolFunc.Invoke();
        Debug.Log(result);

        string stringResult = stringFunc.Invoke(69);
        Debug.Log(stringResult);
    }
}
```


