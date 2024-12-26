using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityUtilities.SerializableDataHelpers
{
    [System.Serializable]
    public abstract class SerializableFuncBase<TFuncType>
        where TFuncType : Delegate
    {
        [SerializeField] protected Object targetObject;
        [SerializeField] protected string methodName;

        private TFuncType func;

        private static BindingFlags SuitableMethodsFlags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;

        protected TFuncType GetReturnedFunc()
        {
            if (func == null)
            {
                if (targetObject == null)
                {
                    throw new ArgumentNullException("Target Object is null!");
                }

                if (string.IsNullOrWhiteSpace(methodName))
                {
                    throw new ArgumentNullException("Target Method is null!");
                }

                Type funcType = typeof(TFuncType);

                MethodInfo info = targetObject
                    .GetType()
                    .GetMethods(SuitableMethodsFlags)
                    .FirstOrDefault(x => IsTargetMethodInfo(x, funcType));

                if (info == null)
                {
                    throw new MissingMethodException($"Object \"{targetObject.name}\" is missing target method: {methodName}");
                }

                func = (TFuncType)Delegate.CreateDelegate(funcType, targetObject, methodName);
            }

            return func;
        }

        #region Utility Functions

        private bool IsTargetMethodInfo(MethodInfo methodInfo, Type funcType)
        {
            if (!string.Equals(methodInfo.Name, methodName, StringComparison.InvariantCulture)) return false;

            Type[] typeArguments = funcType.GetGenericArguments();

            if (methodInfo.ReturnType != typeArguments.Last()) return false;

            ParameterInfo[] parameters = methodInfo.GetParameters();

            if (parameters.Length != (typeArguments.Length - 1)) return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                Type argType = typeArguments[i];
                ParameterInfo parameterInfo = parameters[i];
                if (argType != parameterInfo.ParameterType) return false;
            }

            return true;
        }

        #endregion

        public static implicit operator TFuncType(SerializableFuncBase<TFuncType> func)
        {
            if (func == null) return null;

            TFuncType result = func.GetReturnedFunc();
            return result;
        }
    }
}