using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Processed<>))]
[CustomPropertyDrawer(typeof(ReactiveProcessed<>))]
public class ProcessedDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Создаем foldout для Processed<>
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        if (property.isExpanded)
        {
            // Добавляем отступ для вложенных полей
            EditorGUI.indentLevel++;

            // Определяем высоту строки
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 2;
            Rect baseValueRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
            Rect valueRect = new Rect(position.x, position.y + (lineHeight + spacing) * 2, position.width, lineHeight);

            // Находим поля BaseValue и _value
            var baseValueProperty = property.FindPropertyRelative("BaseValue");
            var valueField = property.FindPropertyRelative("_value");

            // Отрисовываем BaseValue как редактируемое поле
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(baseValueRect, baseValueProperty, new GUIContent("Base Value"));
            if (EditorGUI.EndChangeCheck())
            {
                // Устанавливаем _value равным BaseValue при изменении BaseValue
                CopyValue(baseValueProperty, valueField);
                property.serializedObject.ApplyModifiedProperties();
            }

            // Отрисовываем _value как ReadOnly поле
            GUI.enabled = false;
            EditorGUI.PropertyField(valueRect, valueField, new GUIContent("Value"));
            GUI.enabled = true;

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Высота заголовка (одна строка) и двух строк для BaseValue и Value, если раскрыто
        if (property.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 4;
        }
        return EditorGUIUtility.singleLineHeight;
    }

    private void CopyValue(SerializedProperty source, SerializedProperty target)
    {
        switch (source.propertyType)
        {
            case SerializedPropertyType.Integer:
                target.intValue = source.intValue;
                break;
            case SerializedPropertyType.Float:
                target.floatValue = source.floatValue;
                break;
            case SerializedPropertyType.String:
                target.stringValue = source.stringValue;
                break;
            case SerializedPropertyType.Boolean:
                target.boolValue = source.boolValue;
                break;
            case SerializedPropertyType.ObjectReference:
                target.objectReferenceValue = source.objectReferenceValue;
                break;
            case SerializedPropertyType.Color:
                target.colorValue = source.colorValue;
                break;
            case SerializedPropertyType.Vector2:
                target.vector2Value = source.vector2Value;
                break;
            case SerializedPropertyType.Vector3:
                target.vector3Value = source.vector3Value;
                break;
            case SerializedPropertyType.Vector4:
                target.vector4Value = source.vector4Value;
                break;
            case SerializedPropertyType.Rect:
                target.rectValue = source.rectValue;
                break;
            case SerializedPropertyType.Bounds:
                target.boundsValue = source.boundsValue;
                break;
            case SerializedPropertyType.Quaternion:
                target.quaternionValue = source.quaternionValue;
                break;
            case SerializedPropertyType.AnimationCurve:
                target.animationCurveValue = source.animationCurveValue;
                break;
            case SerializedPropertyType.Gradient:
                target.gradientValue = source.gradientValue;
                break;
            case SerializedPropertyType.ExposedReference:
                target.exposedReferenceValue = source.exposedReferenceValue;
                break;
            case SerializedPropertyType.ManagedReference:
                target.managedReferenceValue = source.managedReferenceValue;
                break;
            case SerializedPropertyType.RectInt:
                target.rectIntValue = source.rectIntValue;
                break;
            case SerializedPropertyType.BoundsInt:
                target.boundsIntValue = source.boundsIntValue;
                break;
            case SerializedPropertyType.Hash128:
                target.hash128Value = source.hash128Value;
                break;
            default:
                Debug.LogWarning($"Unsupported SerializedPropertyType: {source.propertyType}");
                break;
        }
    }
}


// using UnityEditor;
// using UnityEngine;
// using System.Reflection;
// using System;
// using System.Collections.Generic;

// [CustomPropertyDrawer(typeof(Processed<>))]
// [CustomPropertyDrawer(typeof(ReactiveProcessed<>))]
// public class ProcessedDrawer<T> : PropertyDrawer
// {
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         var target = property.serializedObject.context;
//         EditorGUI.BeginProperty(position, label, property);

//         // Создаем foldout для Processed<>
//         property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

//         if (property.isExpanded)
//         {
//             // Добавляем отступ для вложенных полей
//             EditorGUI.indentLevel++;

//             // Определяем высоту строки
//             float lineHeight = EditorGUIUtility.singleLineHeight;
//             float spacing = 2;
//             Rect baseValueRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
//             Rect valueRect = new Rect(position.x, position.y + (lineHeight + spacing) * 2, position.width, lineHeight);

//             // Находим поля BaseValue и _value
//             var baseValueProperty = property.FindPropertyRelative("BaseValue");
//             var valueField = property.FindPropertyRelative("_value");

//             // Отрисовываем BaseValue как редактируемое поле
//             EditorGUI.BeginChangeCheck();
//             EditorGUI.PropertyField(baseValueRect, baseValueProperty, new GUIContent("Base Value"));
//             if (EditorGUI.EndChangeCheck())
//             {
//                 // Устанавливаем _value равным BaseValue при изменении BaseValue
//                 CopyValue(baseValueProperty, valueField);
//                 property.serializedObject.ApplyModifiedProperties();
//             }

//             // Отрисовываем _value как ReadOnly поле
//             GUI.enabled = false;
//             EditorGUI.PropertyField(valueRect, valueField, new GUIContent("Value"));
//             GUI.enabled = true;

//             var processorsProperty = property.FindPropertyRelative("Processors");
//             if (processorsProperty != null)
//             {
//                 Rect processorRect = new Rect(position.x, position.y + (lineHeight + spacing) * 3, position.width, lineHeight);

//                 processorsProperty.
//                 foreach (var element in processorsProperty)
//                 {
//                     // Обрабатываем SortedList<int, List<Processor<T>>> Processors
//                     var processorList = element.FindPropertyRelative("Value");
//                     if (processorList != null)
//                     {
//                         foreach (SerializedProperty processor in processorList)
//                         {
//                             // Получаем делегат и его имя метода
//                             var delegateValue = processor.objectReferenceValue;
//                             if (delegateValue != null)
//                             {
//                                 var methodInfo = delegateValue.GetType().GetMethod("Invoke");
//                                 if (methodInfo != null)
//                                 {
//                                     string methodName = methodInfo.Name;
//                                     // Отображаем приоритет (ключ) и название метода
//                                     EditorGUI.LabelField(processorRect, $"Priority: {element.FindPropertyRelative("Key").intValue} - Method: {methodName}");
//                                     processorRect.y += lineHeight + spacing; // Увеличиваем позицию для следующего элемента
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }

//             EditorGUI.indentLevel--;
//         }

//         EditorGUI.EndProperty();
//     }

//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         // Высота заголовка (одна строка), двух строк для BaseValue и Value и строк для методов делегатов
//         float height = EditorGUIUtility.singleLineHeight * 3 + 4;

//         var processorsProperty = property.FindPropertyRelative("Processors");
//         if (processorsProperty != null)
//         {
//             foreach (var element in processorsProperty)
//             {
//                 var processorList = element.FindPropertyRelative("Value");
//                 if (processorList != null)
//                 {
//                     height += processorList.arraySize * (EditorGUIUtility.singleLineHeight + 2);
//                 }
//             }
//         }

//         return height;
//     }

//     private void CopyValue(SerializedProperty source, SerializedProperty target)
//     {
//         switch (source.propertyType)
//         {
//             case SerializedPropertyType.Integer:
//                 target.intValue = source.intValue;
//                 break;
//             case SerializedPropertyType.Float:
//                 target.floatValue = source.floatValue;
//                 break;
//             case SerializedPropertyType.String:
//                 target.stringValue = source.stringValue;
//                 break;
//             case SerializedPropertyType.Boolean:
//                 target.boolValue = source.boolValue;
//                 break;
//             case SerializedPropertyType.ObjectReference:
//                 target.objectReferenceValue = source.objectReferenceValue;
//                 break;
//             case SerializedPropertyType.Color:
//                 target.colorValue = source.colorValue;
//                 break;
//             case SerializedPropertyType.Vector2:
//                 target.vector2Value = source.vector2Value;
//                 break;
//             case SerializedPropertyType.Vector3:
//                 target.vector3Value = source.vector3Value;
//                 break;
//             case SerializedPropertyType.Vector4:
//                 target.vector4Value = source.vector4Value;
//                 break;
//             case SerializedPropertyType.Rect:
//                 target.rectValue = source.rectValue;
//                 break;
//             case SerializedPropertyType.Bounds:
//                 target.boundsValue = source.boundsValue;
//                 break;
//             case SerializedPropertyType.Quaternion:
//                 target.quaternionValue = source.quaternionValue;
//                 break;
//             case SerializedPropertyType.AnimationCurve:
//                 target.animationCurveValue = source.animationCurveValue;
//                 break;
//             case SerializedPropertyType.Gradient:
//                 target.gradientValue = source.gradientValue;
//                 break;
//             case SerializedPropertyType.ExposedReference:
//                 target.exposedReferenceValue = source.exposedReferenceValue;
//                 break;
//             case SerializedPropertyType.ManagedReference:
//                 target.managedReferenceValue = source.managedReferenceValue;
//                 break;
//             case SerializedPropertyType.RectInt:
//                 target.rectIntValue = source.rectIntValue;
//                 break;
//             case SerializedPropertyType.BoundsInt:
//                 target.boundsIntValue = source.boundsIntValue;
//                 break;
//             case SerializedPropertyType.Hash128:
//                 target.hash128Value = source.hash128Value;
//                 break;
//             default:
//                 Debug.LogWarning($"Unsupported SerializedPropertyType: {source.propertyType}");
//                 break;
//         }
//     }
// }
