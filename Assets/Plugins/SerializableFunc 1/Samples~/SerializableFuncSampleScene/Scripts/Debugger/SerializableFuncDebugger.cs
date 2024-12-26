#if UNITY_EDITOR

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityUtilities.SerializableDataHelpers;

public class SerializableFuncDebugger : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI displayedText;

    [Header("Debug Controls")]
    [SerializeField] private KeyCode boolFuncInvocationKeyCode = KeyCode.Q;
    [SerializeField] private KeyCode stringFuncInvocationKeyCode = KeyCode.W;
    [SerializeField] private KeyCode vector3FuncFuncInvocationKeyCode = KeyCode.E;
    [SerializeField] private KeyCode quaternionFuncFuncInvocationKeyCode = KeyCode.R;
    [SerializeField] private KeyCode floatFuncInvocationKeyCode = KeyCode.T;
    [SerializeField] private KeyCode listFuncInvocationKeyCode = KeyCode.Y;

    [Header("Test Funcs")]
    [SerializeField] private SerializableFunc<bool> boolFunc;
    [SerializeField] private SerializableFunc<string> stringFunc;
    [SerializeField] private SerializableFunc<Vector3> vector3Func;
    [SerializeField] private SerializableFunc<Quaternion> quaternionFunc;
    [SerializeField] private SerializableFunc<float> floatFunc;
    [SerializeField] private SerializableFunc<List<string>> listFunc;
    [SerializeField] private SerializableFunc<float[]> arrayFunc;
    [SerializeField] private SerializableFunc<float[][]> array2DFunc;
    [SerializeField] private SerializableFunc<InternalClass> internalClassFunc;
    [SerializeField] private SerializableFunc<GenericInternalClass<bool>> genericInternalClassFunc;
    [SerializeField] private SerializableFunc<GenericInternalClass<Dictionary<string, byte>>> genericInternalClassFunc2;

    [Header("Test Objects")]
    [SerializeField] private List<string> stringsList;

    private void Update()
    {
        TryInvokeBoolFunc();
        TryInvokeStringFunc();
        TryInvokeVector3Func();
        TryInvokeQuaternionFunc();
        TryInvokeFloatFunc();
        TryInvokeListFunc();
    }

    #region Public Calls

    public void CallBoolFunc()
    {
        bool result = boolFunc.Invoke();
        DisplayCalledFuncResult(result);
    }

    public void CallStringFunc()
    {
        string result = stringFunc.Invoke();
        DisplayCalledFuncResult(result);
    }

    public void CallVector3Func()
    {
        Vector3 result = vector3Func.Invoke();
        DisplayCalledFuncResult(result);
    }

    public void CallQuaternionFunc()
    {
        Quaternion result = quaternionFunc.Invoke();
        DisplayCalledFuncResult(result);
    }

    public void CallFloatFunc()
    {
        float result = floatFunc.Invoke();
        DisplayCalledFuncResult(result);
    }

    public void CallListFunc()
    {
        List<string> result = listFunc.Invoke();
        PrintListDataToConsole(result);
    }

    #endregion

    #region Public Data Returns

    public List<string> GetSampleList()
    {
        return stringsList;
    }

    #endregion

    #region Private Calls

    private void TryInvokeBoolFunc()
    {
        if (!Input.GetKeyDown(boolFuncInvocationKeyCode)) return;
        CallBoolFunc();
    }

    private void TryInvokeStringFunc()
    {
        if (!Input.GetKeyDown(stringFuncInvocationKeyCode)) return;
        CallStringFunc();
    }

    private void TryInvokeVector3Func()
    {
        if (!Input.GetKeyDown(vector3FuncFuncInvocationKeyCode)) return;
        CallVector3Func();
    }

    private void TryInvokeQuaternionFunc()
    {
        if (!Input.GetKeyDown(quaternionFuncFuncInvocationKeyCode)) return;
        CallQuaternionFunc();
    }

    private void TryInvokeFloatFunc()
    {
        if (!Input.GetKeyDown(floatFuncInvocationKeyCode)) return;
        CallFloatFunc();
    }

    private void TryInvokeListFunc()
    {
        if (!Input.GetKeyDown(listFuncInvocationKeyCode)) return;
        CallListFunc();
    }

    #endregion

    private void DisplayCalledFuncResult(object result)
    {
        string resultString = $"Res: {result}";

        if (displayedText != null)
        {
            displayedText.text = resultString;
        }
        else
        {
            Debug.Log(resultString);
        }
    }

    private void PrintListDataToConsole(List<string> list)
    {
        if (list == null)
        {
            Debug.Log("The list is null");
        }
        else
        {
            Debug.Log($"The list has {list.Count} elements");
            foreach (string item in list)
            {
                Debug.Log(item);
            }
        }
    }

    private class InternalClass
    {
    }

    private class GenericInternalClass<T>
    {
    }
}

#endif