using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

[CustomPropertyDrawer(typeof(Processed<>))]
[CustomPropertyDrawer(typeof(ReactiveProcessed<>))]
public class ProcessedDrawer : PropertyDrawer
{
    private bool isProcessorsFoldoutOpen = false; // Для управления состоянием foldout для Processors

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Создаем foldout для Processed<>
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        float currentY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.isExpanded)
        {
            // Добавляем отступ для вложенных полей
            EditorGUI.indentLevel++;

            // Определяем высоту строки
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect baseValueRect = new Rect(position.x, currentY, position.width, lineHeight);
            currentY += lineHeight + spacing;
            Rect valueRect = new Rect(position.x, currentY, position.width, lineHeight);
            currentY += lineHeight + spacing;

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

            // Отрисовываем foldout для Processors
            Rect processorsFoldoutRect = new Rect(position.x, currentY, position.width, lineHeight);
            isProcessorsFoldoutOpen = EditorGUI.Foldout(processorsFoldoutRect, isProcessorsFoldoutOpen, "Processors");
            currentY += lineHeight + spacing;

            if (isProcessorsFoldoutOpen)
            {
                // Добавляем отступ для Processors
                EditorGUI.indentLevel++;

                // Рисуем список процессов
                object targetObject = property.serializedObject.targetObject;
                if (targetObject != null)
                {
                    // Поиск поля Processors в объекте
                    var fieldInfo = targetObject.GetType().GetField(property.name,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    if (fieldInfo != null)
                    {
                        // Получаем экземпляр Processed<T>
                        object processedInstance = fieldInfo.GetValue(targetObject);
                        if (processedInstance != null)
                        {
                            // Определяем тип T
                            Type processedType = fieldInfo.FieldType;
                            Type genericTypeT = processedType.GetGenericArguments()[0];

                            // Ищем поле Processors
                            FieldInfo processorsField = processedType.GetField("Processors",
                                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                            if (processorsField != null)
                            {
                                object processorsValue = processorsField.GetValue(processedInstance);
                                if (processorsValue is System.Collections.IDictionary processorsDictionary)
                                {
                                    foreach (DictionaryEntry entry in processorsDictionary)
                                    {
                                        int key = (int)entry.Key; // Ключ — int
                                        var processorList = entry.Value as System.Collections.IEnumerable;

                                        foreach (var processor in processorList)
                                        {
                                            if (processor is Delegate del)
                                            {
                                                // Получаем имя метода
                                                string methodName = del.Method.Name;

                                                // Получаем имя класса (если есть объект)
                                                string className = del.Target != null
                                                    ? del.Target.GetType().Name
                                                    : del.Method.DeclaringType.Name;

                                                string processorName = $"{key} {className}:{methodName}";

                                                // Рисуем имя процессора
                                                EditorGUI.LabelField(
                                                    new Rect(position.x + EditorGUI.indentLevel * 15, currentY, position.width - 15, lineHeight),
                                                    processorName
                                                );
                                                currentY += lineHeight + spacing;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        if (!property.isExpanded)
            return baseHeight;

        // Высота для BaseValue и _value
        float expandedHeight = baseHeight * 4 + spacing * 2;

        // Высота для Processors
        if (isProcessorsFoldoutOpen)
        {
            object targetObject = property.serializedObject.targetObject;
            if (targetObject != null)
            {
                var fieldInfo = targetObject.GetType().GetField(property.name,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (fieldInfo != null)
                {
                    object processedInstance = fieldInfo.GetValue(targetObject);
                    if (processedInstance != null)
                    {
                        FieldInfo processorsField = fieldInfo.FieldType.GetField("Processors",
                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                        if (processorsField != null)
                        {
                            object processorsValue = processorsField.GetValue(processedInstance);
                            if (processorsValue is System.Collections.IDictionary processorsDictionary)
                            {
                                int itemCount = 0;
                                foreach (DictionaryEntry entry in processorsDictionary)
                                {
                                    var processorList = entry.Value as System.Collections.IEnumerable;
                                    if (processorList != null)
                                    {
                                        foreach (var _ in processorList)
                                        {
                                            itemCount++;
                                        }
                                    }
                                }

                                expandedHeight += (baseHeight + spacing) * itemCount;
                            }
                        }
                    }
                }
            }
        }

        return expandedHeight;
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