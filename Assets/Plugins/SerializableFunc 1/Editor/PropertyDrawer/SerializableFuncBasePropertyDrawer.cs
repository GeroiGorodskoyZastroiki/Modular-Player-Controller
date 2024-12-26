#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Extensions.SystemExtensions;
using Utilities.Extensions.UIToolkit;
using UnityUtilities.SerializableDataHelpers;
using Object = UnityEngine.Object;

namespace UnityUtilities.SerializableDataHelpers.UnityEditorUtilities
{
    [CustomPropertyDrawer(typeof(SerializableFuncBase<>), true)]
    public class SerializableFuncBasePropertyDrawer : PropertyDrawer
    {
        private const string Target_Function_Label = "Target Function";
        private const string Target_Object_Label = "Target Object";
        private const string No_Function_Label = "No Function";

        private const string TargetObject_Property_Name = "targetObject";
        private const string MethodName_Property_Name = "methodName";

        private const string MixedValueContent_Property_Name = "mixedValueContent";

        private static BindingFlags SuitableMethodsFlags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;

        #region GUI Drawing

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            if (Event.current.type == EventType.Repaint)
            {
                ReorderableList.defaultBehaviours.DrawHeaderBackground(position);
            }

            Rect headerRect = position;

            headerRect.xMin += 6f;
            headerRect.xMax -= 6f;
            headerRect.height -= 2f;
            headerRect.y += 1f;

            DrawFuncHeader(ref headerRect, property, label.text);

            DrawPropertyView(ref position, ref headerRect, property);

            EditorGUI.EndProperty();
        }

        private void DrawFuncHeader(ref Rect position, SerializedProperty funcProperty, string labelText)
        {
            position.height = 18f;
            string text = GetHeaderText(funcProperty, labelText);
            GUI.Label(position, text);
        }

        private void DrawPropertyView(ref Rect position, ref Rect headerRect, SerializedProperty funcProperty)
        {
            Rect listRect = new Rect(position.x, headerRect.y + headerRect.height, position.width, 45f);

            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            if (Event.current.type == EventType.Repaint)
            {
                ReorderableList.defaultBehaviours.boxBackground.Draw(listRect, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);
            }

            listRect.yMin += 1f;
            listRect.yMax -= 4f;
            listRect.xMin += 1f;
            listRect.xMax -= 1f;

            SerializedProperty targetObjectProperty = GetTargetObjectSerializedProperty(funcProperty);
            SerializedProperty targetMethodProperty = GetMethodNameSerializedProperty(funcProperty);
            EditorGUI.BeginChangeCheck();

            Rect targetObjectLineRect = GetListViewSingleLineRect(ref position, ref listRect);
            Rect[] targetObjectRects = GetTwoRectsForLabelAndProperty(ref targetObjectLineRect);

            EditorGUI.LabelField(targetObjectRects[0], Target_Object_Label);
            EditorGUI.PropertyField(targetObjectRects[1], targetObjectProperty, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                targetMethodProperty.stringValue = string.Empty;
                targetMethodProperty.serializedObject.ApplyModifiedProperties();
            }

            DrawMethodNameProperty(ref targetObjectLineRect, funcProperty, targetObjectProperty, targetMethodProperty);

            EditorGUI.indentLevel = indentLevel;
        }

        private Rect DrawMethodNameProperty(ref Rect previousRect,
            SerializedProperty funcProperty,
            SerializedProperty targetObjectSerializedProperty,
            SerializedProperty targetMethodSerializedProperty)
        {
            Rect targetMethodRect = GetNextSingleLineRect(ref previousRect);
            Rect[] targetMethodRects = GetTwoRectsForLabelAndProperty(ref targetMethodRect);

            EditorGUI.LabelField(targetMethodRects[0], Target_Function_Label);

            using (new EditorGUI.DisabledScope(targetObjectSerializedProperty.objectReferenceValue == null))
            {
                EditorGUI.BeginProperty(targetMethodRect, GUIContent.none, targetMethodSerializedProperty);

                GUIContent content;
                if (EditorGUI.showMixedValue)
                {
                    content = GetMixedValueContentGUIContent();
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    if (targetObjectSerializedProperty.objectReferenceValue == null
                        || string.IsNullOrEmpty(targetMethodSerializedProperty.stringValue))
                    {
                        stringBuilder.Append(No_Function_Label);
                    }
                    else if (!IsPersistantListenerValid(funcProperty, targetObjectSerializedProperty, targetMethodSerializedProperty))
                    {
                        string missingComponentString = GetMissingComponentMethodString(targetObjectSerializedProperty, targetMethodSerializedProperty);

                        stringBuilder.Append(missingComponentString);
                    }
                    else
                    {
                        stringBuilder.Append(targetObjectSerializedProperty.objectReferenceValue.GetType().Name);
                        if (!string.IsNullOrEmpty(targetMethodSerializedProperty.stringValue))
                        {
                            stringBuilder.Append(".");
                            string nicerName = NicifyGetterPropertyName(targetMethodSerializedProperty.stringValue);
                            stringBuilder.Append(nicerName);
                        }
                    }

                    content = new GUIContent(stringBuilder.ToString());
                }

                if (EditorGUI.DropdownButton(targetMethodRects[1], content, FocusType.Passive, EditorStyles.popup))
                {
                    CachedPropertiesAndObjectsData data = new CachedPropertiesAndObjectsData(funcProperty, targetObjectSerializedProperty, targetMethodSerializedProperty, null, null);

                    BuildGenericMenu(data).DropDown(previousRect);
                }

                EditorGUI.EndProperty();
            }

            return targetMethodRect;
        }

        private bool IsPersistantListenerValid(SerializedProperty funcProperty,
            SerializedProperty targetObjectProperty,
            SerializedProperty targetMethodProperty)
        {
            if (targetObjectProperty.objectReferenceValue == null
                || string.IsNullOrWhiteSpace(targetMethodProperty.stringValue)) return false;

            Type[] funcArguments = GetFuncTypeArguments(funcProperty);
            Type returnType = funcArguments.Last();

            MethodInfo targetMethod = targetObjectProperty.objectReferenceValue
                .GetType()
                .GetMethods(SuitableMethodsFlags)
                .Where(x => IsSuitableMethodInfo(x, funcArguments))
                .FirstOrDefault(x => string.Equals(x.Name, targetMethodProperty.stringValue));

            if (targetMethod != null) return true;
            if ((funcArguments.Length - 1) != 0) return false;

            MethodInfo targetProperty = targetObjectProperty.objectReferenceValue
                .GetType()
                .GetProperties(SuitableMethodsFlags)
                .Where(x => IsSuitableProperty(x, returnType))
                .Select(x => x.GetGetMethod())
                .FirstOrDefault(x => string.Equals(x.Name, targetMethodProperty.stringValue));

            return targetProperty != null;
        }

        private GUIContent GetMixedValueContentGUIContent()
        {
            MethodInfo info = typeof(EditorGUI)
                .GetMethod(MixedValueContent_Property_Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);

            return info.Invoke(null, null) as GUIContent;
        }

        #region Rect Utility

        private Rect GetListViewSingleLineRect(ref Rect position, ref Rect listRect)
        {
            Rect result = new Rect(position.x,
                listRect.y + EditorGUIUtility.standardVerticalSpacing,
                listRect.width,
                EditorGUIUtility.singleLineHeight);

            result.xMin += 8f;
            result.xMax -= 3f;

            return result;
        }

        private Rect GetNextSingleLineRect(ref Rect previousRect)
        {
            return new Rect(previousRect.x,
                previousRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                previousRect.width,
                EditorGUIUtility.singleLineHeight);
        }

        private Rect[] GetTwoRectsForLabelAndProperty(ref Rect lineRect)
        {
            Rect labelRect = new Rect(lineRect.position.x,
                lineRect.position.y,
                lineRect.width / 3 - 1f,
                lineRect.height);

            float propertyRectX = labelRect.xMax + 2f;
            float propertyRectWidth = lineRect.xMax - propertyRectX;

            Rect propertyRect = new Rect(
                propertyRectX,
                labelRect.y,
                propertyRectWidth,
                lineRect.height);

            return new Rect[] { labelRect, propertyRect };
        }

        #endregion

        #endregion

        #region UI Toolkit Drawing

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement visualElement = new VisualElement();
            visualElement.AddToClassList("unity-event__container");

            Label label = new Label();
            label.text = GetHeaderText(property, property.displayName);
            label.tooltip = property.tooltip;
            label.AddToClassList("unity-list-view__header");
            label.style.overflow = new StyleEnum<Overflow>(Overflow.Hidden);

            VisualElement assignmentVE = CreateFuncAssignmentVisualElement(property);

            visualElement.Add(label);
            visualElement.Add(assignmentVE);

            return visualElement;
        }

        #region Visible Container Drawing

        private VisualElement CreateFuncAssignmentVisualElement(SerializedProperty funcProperty)
        {
            VisualElement visualElement = CreateContainerVisualElement();

            SerializedProperty objectProperty = GetTargetObjectSerializedProperty(funcProperty);
            string objectPropertyLabel = ObjectNames.NicifyVariableName(TargetObject_Property_Name);

            ObjectField targetObjectField = GetTargetObjectField(visualElement, objectProperty, objectPropertyLabel);

            SerializedProperty methodNameProperty = GetMethodNameSerializedProperty(funcProperty);
            DropdownField functionDropdown = CreateChosenMethodDropdownField(visualElement, methodNameProperty);

            RegisterObjectFieldCallback(targetObjectField, functionDropdown);

            CachedPropertiesAndObjectsData data = new CachedPropertiesAndObjectsData(funcProperty, objectProperty, methodNameProperty, targetObjectField, functionDropdown);

            AssignGenericOSMenuValueToDropdownField(data);

            AssignFormattingCallbackToDropdownField(data);

            return visualElement;
        }

        private VisualElement CreateContainerVisualElement()
        {
            VisualElement visualElement = new VisualElement();

            visualElement.AddToClassList("unity-scroll-view");
            visualElement.AddToClassList("unity-collection-view__scroll-view");
            visualElement.AddToClassList("unity-collection-view--with-border");
            visualElement.AddToClassList("unity-list-view__scroll-view--with-footer");
            visualElement.AddToClassList("unity-event__list-view-scroll-view");

            visualElement.style.paddingBottom = 5f;
            visualElement.style.paddingLeft = 3f;
            visualElement.style.paddingRight = 3f;
            visualElement.style.marginBottom = 5f;

            return visualElement;
        }

        private ObjectField GetTargetObjectField(VisualElement parentContainer,
            SerializedProperty objectProperty,
            string objectPropertyLabel)
        {
            ObjectField targetObjectField = new ObjectField(objectPropertyLabel);
            parentContainer.Add(targetObjectField);
            targetObjectField.BindProperty(objectProperty);
            targetObjectField.style.marginRight = 3f;
            targetObjectField.labelElement.style.paddingTop = 0f;
            return targetObjectField;
        }

        private DropdownField CreateChosenMethodDropdownField(VisualElement parentContainer,
            SerializedProperty methodNameProperty)
        {
            DropdownField functionDropdown = new DropdownField(Target_Function_Label);
            parentContainer.Add(functionDropdown);
            functionDropdown.BindProperty(methodNameProperty);
            functionDropdown.style.marginRight = 3f;
            functionDropdown.labelElement.style.paddingTop = 0f;
            return functionDropdown;
        }

        private void RegisterObjectFieldCallback(ObjectField targetObjectField, DropdownField functionDropdown)
        {
            targetObjectField.RegisterValueChangedCallback(changeEvent =>
            {
                // hadPreviousValue has to be checked because this event will be raised after you bind a property and the object field is created

                bool hasNewValue = changeEvent.newValue != null;
                bool hadPreviousValue = changeEvent.previousValue != null;
                if (!hasNewValue
                    ||
                    (hasNewValue && hadPreviousValue && changeEvent.previousValue != changeEvent.newValue))
                {
                    functionDropdown.value = null;
                }

                functionDropdown.SetEnabled(hasNewValue);
            });
        }

        private void AssignGenericOSMenuValueToDropdownField(CachedPropertiesAndObjectsData data)
        {
            data.DropdownField.AssignGenericMenu(() =>
                BuildGenericMenu(data));
        }

        private void AssignFormattingCallbackToDropdownField(CachedPropertiesAndObjectsData data)
        {
            data.DropdownField.AssignFormattingCallback(
                _ => FormatFunctionValueSelected(data));
        }

        private string FormatFunctionValueSelected(CachedPropertiesAndObjectsData data)
        {
            Object obj = data.ObjectField.value;
            string methodNameValue = data.TargetMethodProperty.stringValue;

            if (obj == null || string.IsNullOrWhiteSpace(methodNameValue)) return No_Function_Label;

            if (!IsPersistantListenerValid(data.FuncProperty, data.TargetObjectProperty, data.TargetMethodProperty))
            {
                return GetMissingComponentMethodString(data.TargetObjectProperty, data.TargetMethodProperty);
            }

            methodNameValue = NicifyGetterPropertyName(methodNameValue);
            return $"{obj.GetType().Name}.{methodNameValue}";
        }

        #endregion

        #endregion

        #region Common Utility

        private string GetHeaderText(SerializedProperty property, string labelText)
        {
            return $"{(string.IsNullOrEmpty(labelText) ? "Func" : labelText)} {GetEventParamsString(property)}";
        }

        private string GetEventParamsString(SerializedProperty property)
        {
            IEnumerable<string> typeNames = GetFuncTypeArguments(property)
                .Select(x => x.NicifyTypeName());

            string funcReturnValue = string.Join(", ", typeNames);
            return $"({funcReturnValue})";
        }

        private Type[] GetFuncTypeArguments(SerializedProperty funcProperty)
        {
            return funcProperty.GetBoxedValue().
                GetType().
                GetGenericArguments();
        }

        private string GetMissingComponentMethodString(SerializedProperty objectProperty,
            SerializedProperty methodProperty)
        {
            string arg = "UnknownComponent";
            Object objectValue = objectProperty.objectReferenceValue;
            if (objectValue != null)
            {
                arg = objectValue.GetType().Name;
            }

            return $"<Missing {arg}.{methodProperty.stringValue}>";
        }

        #endregion

        #region Generic Menu Building

        private GenericMenu BuildGenericMenu(CachedPropertiesAndObjectsData data)
        {
            Object target = data.TargetObjectProperty.objectReferenceValue;

            if (target is Component targetComponent)
            {
                target = targetComponent.gameObject;
            }

            SerializedProperty methodNameProperty = data.FuncProperty.FindPropertyRelative(MethodName_Property_Name);

            GenericMenu genericMenu = new GenericMenu();

            genericMenu.AddItem(new GUIContent(No_Function_Label), string.IsNullOrEmpty(methodNameProperty.stringValue), ClearMethodNameProperty, methodNameProperty);

            if (target == null) return genericMenu;

            genericMenu.AddSeparator("");

            Type[] funcParameters = GetFuncTypeArguments(data.FuncProperty);
            Component[] components = ((GameObject)target).GetComponents<Component>();

            DrawMenuForComponent(genericMenu, data, (GameObject)target, funcParameters);

            foreach (Component component in components)
            {
                DrawMenuForComponent(genericMenu, data, component, funcParameters);
            }

            return genericMenu;
        }

        private void DrawMenuForComponent(GenericMenu genericMenu,
            CachedPropertiesAndObjectsData data,
            Object component,
            Type[] funcParameters)
        {
            Type componentType = component.GetType();
            string componentName = componentType.Name;

            MethodInfo[] suitableProperties = GetSuitablePropertiesFromType(componentType, funcParameters);

            if (suitableProperties != null && suitableProperties.Any())
            {
                GUIContent propertiesContent = new GUIContent($"{componentName}/Property Getters");
                genericMenu.AddDisabledItem(propertiesContent);

                foreach (MethodInfo propertyGetter in suitableProperties)
                {
                    string propertyName = NicifyGetterPropertyName(propertyGetter);
                    GUIContent methodContent = new GUIContent($"{componentName}/{propertyName}");

                    SelectedMethodInfo info = new SelectedMethodInfo(component, propertyGetter);

                    bool isOn = IsMethodChosen(data.TargetObjectProperty, data.TargetMethodProperty, component, propertyGetter);

                    var menuSelectedCallback = GetMenuSelectedAction(data);
                    genericMenu.AddItem(methodContent, isOn, menuSelectedCallback, info);
                }

                genericMenu.AddSeparator($"{componentName}/");
            }

            MethodInfo[] suitableMethods = GetSuitableMethodsFromType(componentType, funcParameters);

            if (suitableMethods.Any())
            {
                GUIContent methodsContent = new GUIContent($"{componentName}/Invokable Methods");
                genericMenu.AddDisabledItem(methodsContent);

                foreach (MethodInfo method in suitableMethods)
                {
                    GUIContent methodContent = new GUIContent($"{componentName}/{method.Name}");

                    SelectedMethodInfo info = new SelectedMethodInfo(component, method);

                    bool isOn = IsMethodChosen(data.TargetObjectProperty, data.TargetMethodProperty, component, method);

                    var menuSelectedCallback = GetMenuSelectedAction(data);
                    genericMenu.AddItem(methodContent, isOn, menuSelectedCallback, info);
                }
            }
        }

        private bool IsMethodChosen(SerializedProperty targetObjectProperty,
            SerializedProperty targetMethodProperty,
            Object component,
            MethodInfo method)
        {
            if (string.IsNullOrWhiteSpace(targetMethodProperty.stringValue)) return false;

            return targetObjectProperty.objectReferenceValue == component
                && string.Equals(targetMethodProperty.stringValue, method.Name);
        }

        private GenericMenu.MenuFunction2 GetMenuSelectedAction(CachedPropertiesAndObjectsData data)
        {
            return obj => HandleNewMethodAssigned(obj, data);
        }

        #region Method List Getters

        private MethodInfo[] GetSuitableMethodsFromType(Type componentType, Type[] funcParameters)
        {
            IEnumerable<MethodInfo> suitableMethods = componentType
                .GetMethods(SuitableMethodsFlags)
                .Where(x => IsSuitableMethodInfo(x, funcParameters))
                .OrderBy(x => x.Name);

            return suitableMethods.ToArray();
        }

        private MethodInfo[] GetSuitablePropertiesFromType(Type componentType, Type[] funcParameters)
        {
            if (funcParameters.Length != 1) return null;
            Type returnType = funcParameters.Last();

            IEnumerable<PropertyInfo> suitableProperties = componentType
                .GetProperties(SuitableMethodsFlags)
                .Where(x => IsSuitableProperty(x, returnType));

            IEnumerable<MethodInfo> propertyGetters = suitableProperties
                .Select(x => x.GetGetMethod())
                .OrderBy(x => x.Name);

            return propertyGetters.ToArray();
        }

        private bool IsSuitableMethodInfo(MethodInfo info, Type[] funcParameters)
        {
            if (info.IsSpecialName) return false;

            Type returnType = funcParameters.Last();
            if (info.ReturnType != returnType) return false;

            ParameterInfo[] parameters = info.GetParameters();
            if (parameters.Length != (funcParameters.Length - 1)) return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                Type funcParameterType = funcParameters[i];

                if (parameter.ParameterType != funcParameterType) return false;
            }

            return true;
        }

        private bool IsSuitableProperty(PropertyInfo propertyInfo, Type returnType)
        {
            if (propertyInfo.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: true).Length != 0) return false;

            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null) return false;

            return getMethod.ReturnType == returnType;
        }

        #endregion

        #region Event Handlers

        private void ClearMethodNameProperty(object methodNameProperty)
        {
            SerializedProperty nameProperty = (SerializedProperty)methodNameProperty;

            nameProperty.stringValue = string.Empty;
            nameProperty.serializedObject.ApplyModifiedProperties();
        }

        private void HandleNewMethodAssigned(object selectedInfo, CachedPropertiesAndObjectsData data)
        {
            SelectedMethodInfo info = (SelectedMethodInfo)selectedInfo;

            // If we're drawing with UI Toolkit, the update will be a bit stoopid when you bind a property
            // Assigning a value without notify ensures that we don't accidentally set the function dropdown value to a null value
            // This has to work this way because when we assign the object to get the method from, we may have to change the object. Problem being, if we try to change the DropdownField's value or property within the object change event, the assert will fail => the SerializedProperty bound to the Dropdown Field will have a different value than the dropdown field itself for some reason
            // Unity Event has the same bug when drawn with UI Toolkit, so this is a workaround :>
            ObjectField targetObjectField = data.ObjectField;
            targetObjectField?.SetValueWithoutNotify(info.TargetObject);

            data.TargetObjectProperty.objectReferenceValue = info.TargetObject;
#if !UNITY_2023_1_OR_NEWER
            // Has to happen before we reassign the value, so an additional annoying block of ifs here
            bool isSameString = string.Equals(data.TargetMethodProperty.stringValue, info.TargetMethod.Name);
#endif

            data.TargetMethodProperty.stringValue = info.TargetMethod.Name;

#if !UNITY_2023_1_OR_NEWER
            // Older Unity version doesn't automatically use the formatting callback if the old value is the same as the new value
            // This is a problem because properties or methods can have the same name on different objects (gameobject.name and transform.name, for example)
            // The easiest way to call for formatting to happen is to just reassign the callback
            // Only relevant to my fav boi UI toolkit
            DropdownField dropdownField = data.DropdownField;
            if (isSameString && dropdownField != null)
            {
                AssignFormattingCallbackToDropdownField(data);
            }
#endif
            data.TargetMethodProperty.serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Utility

        #region Strings

        private string NicifyGetterPropertyName(MethodInfo methodInfo)
        {
            return NicifyGetterPropertyName(methodInfo.Name);
        }

        private string NicifyGetterPropertyName(string methodName)
        {
            return $"{methodName.Replace("get_", string.Empty)}";
        }

        #endregion

        #endregion

        #endregion

        #region Serialized Property Getters

        private SerializedProperty GetTargetObjectSerializedProperty(SerializedProperty funcProperty)
        {
            SerializedProperty objectProperty = funcProperty.FindPropertyRelative(TargetObject_Property_Name);
            return objectProperty;
        }

        private SerializedProperty GetMethodNameSerializedProperty(SerializedProperty funcProperty)
        {
            SerializedProperty methodNameProperty = funcProperty.FindPropertyRelative(MethodName_Property_Name);
            return methodNameProperty;
        }

        #endregion

        #region GUI Overrides

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 65f;
        }

        #endregion

        #region Helper Classes

        private struct SelectedMethodInfo
        {
            public Object TargetObject;
            public MethodInfo TargetMethod;

            public SelectedMethodInfo(Object obj, MethodInfo method)
            {
                TargetObject = obj;
                TargetMethod = method;
            }
        }

        private struct CachedPropertiesAndObjectsData
        {
            public SerializedProperty FuncProperty;
            public SerializedProperty TargetObjectProperty;
            public SerializedProperty TargetMethodProperty;

            public ObjectField ObjectField;
            public DropdownField DropdownField;

            public CachedPropertiesAndObjectsData(SerializedProperty funcProperty,
                SerializedProperty targetObjectProperty,
                SerializedProperty targetMethodProperty,
                ObjectField objectField,
                DropdownField dropdownField)
            {
                FuncProperty = funcProperty;
                TargetObjectProperty = targetObjectProperty;
                TargetMethodProperty = targetMethodProperty;

                ObjectField = objectField;
                DropdownField = dropdownField;
            }
        }

        #endregion
    }
}

#endif