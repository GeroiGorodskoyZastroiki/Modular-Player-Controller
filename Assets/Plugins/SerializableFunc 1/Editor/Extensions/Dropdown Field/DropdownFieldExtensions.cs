#if UNITY_EDITOR

using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace Utilities.Extensions.UIToolkit
{
    public static class DropdownFieldExtensions
    {
        private static Type genericOSMenuType;
        private static Type iGenericMenuInterfaceType;

        private const string CreateMenuCallback_Property_Name = "createMenuCallback";
        private const string GenericOsMenu_Type_Name = "UnityEditor.UIElements.GenericOSMenu";
        private const string IGenericMenu_Type_Name = "IGenericMenu";

        private const string FormatSelectedValueCallback_Property_Name = "formatSelectedValueCallback";

        #region Generic Menu Assigning

        public static void AssignGenericMenu(this DropdownField dropdownField, 
            Func<GenericMenu> genericMenuBuildingFunc)
        {
            Func<object> genericOSMenuFunc = () =>
            {
                GenericMenu menu = genericMenuBuildingFunc();
                return GetGenericOSMenu(menu);
            };

            object boxedfunc = ConvertFuncToDesiredType(genericOSMenuFunc);

            FieldInfo createMenuCallbackFieldInfo = typeof(DropdownField)
                .GetField(CreateMenuCallback_Property_Name, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            createMenuCallbackFieldInfo.SetValue(dropdownField, boxedfunc);
        }

        #region Generic OS Menu Creation

        private static object GetGenericOSMenu(GenericMenu menu)
        {
            Type type = GetGenericOSMenuType();
            object[] args = new object[] { menu };
            return Activator.CreateInstance(type, args);
        }

        #endregion

        #region Type helpers

        private static Type GetGenericOSMenuType()
        {
            if (genericOSMenuType == null)
            {
                genericOSMenuType = typeof(UnityEditor.UIElements.ColorField).Assembly.GetType(GenericOsMenu_Type_Name, false, true);
            }

            return genericOSMenuType;
        }

        private static Type GetIGenericMenuInterfaceType()
        {
            if (iGenericMenuInterfaceType == null)
            {
                Type genericOSType = GetGenericOSMenuType();
                iGenericMenuInterfaceType = genericOSType.GetInterface(IGenericMenu_Type_Name);
            }

            return iGenericMenuInterfaceType;
        }

        #endregion

        #region Func Conversion

        private static Delegate ConvertFuncToDesiredType(Func<object> func)
        {
            Type interfaceType = GetIGenericMenuInterfaceType();
            Type resultType = typeof(Func<>).MakeGenericType(interfaceType);

            Expression<Func<object>> expressionFunc = FuncToExpression(func);

            InvocationExpression invokedExpression = Expression.Invoke(expressionFunc);
            UnaryExpression convertedReturnValue = Expression.Convert(invokedExpression, interfaceType);

            LambdaExpression lambda = Expression.Lambda(delegateType: resultType, body: convertedReturnValue);

            return lambda.Compile();
        }

        private static Expression<Func<T>> FuncToExpression<T>(Func<T> f)
        {
            return () => f();
        }

        #endregion

        #endregion

        #region Formatting Callback Assigning

        public static void AssignFormattingCallback(this DropdownField dropdownField, 
            Func<string, string> formattingCallback)
        {
#if UNITY_2022_2_OR_NEWER
            dropdownField.formatSelectedValueCallback = formattingCallback;
#else
            AssignFormattingCallbackViaReflection(dropdownField, formattingCallback);      
#endif
        }

        private static void AssignFormattingCallbackViaReflection(this DropdownField functionDropdown, 
            Func<string, string> formattingCallback)
        {
            PropertyInfo propertyInfo = typeof(DropdownField)
                .GetProperty(FormatSelectedValueCallback_Property_Name, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            object[] args = new object[] { formattingCallback };

            propertyInfo.SetMethod.Invoke(functionDropdown, args);
        }

        #endregion
    }
}

#endif