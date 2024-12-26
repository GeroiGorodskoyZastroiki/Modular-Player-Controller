namespace UnityUtilities.SerializableDataHelpers
{
    using System;

    [System.Serializable]
    public class SerializableFunc<TReturn> : SerializableFuncBase<Func<TReturn>>
    {
        public TReturn Invoke()
        {
            Func<TReturn> func = GetReturnedFunc();
            return func();
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TReturn> : SerializableFuncBase<Func<TArg0, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0)
        {
            Func<TArg0, TReturn> func = GetReturnedFunc();
            return func(arg0);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1)
        {
            Func<TArg0, TArg1, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            Func<TArg0, TArg1, TArg2, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn> : SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn> :
        SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn> :
        SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn> :
        SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }
    }

    [System.Serializable]
    public class SerializableFunc<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn> :
        SerializableFuncBase<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn>>
    {
        public TReturn Invoke(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15)
        {
            Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TReturn> func = GetReturnedFunc();
            return func(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }
    }
}