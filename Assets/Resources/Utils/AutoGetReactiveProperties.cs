    // public void AddSubscriptionsToReactivePropertiesFromProcessor(Processor<T> processor, object instance)
    // {
    //     var methodInfo = processor.Method;
    //     var assemblyPath = methodInfo.DeclaringType.Assembly.Location;
    //     var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);
    //     var typeDefinition = assemblyDefinition.MainModule.GetType(methodInfo.DeclaringType.FullName);
    //     var methodDefinition = typeDefinition.Methods.FirstOrDefault<MethodDefinition>(m => m.Name == methodInfo.Name);

    //     if (methodDefinition == null)
    //     {
    //         Debug.LogError("Method is not found.");
    //         return;
    //     }

    //     List<object> reactiveProperties = new List<object>();

    //     foreach (var instruction in methodDefinition.Body.Instructions)
    //     {
    //         if (instruction.OpCode.Code == Mono.Cecil.Cil.Code.Ldfld)
    //         {
    //             if (instruction.Operand is FieldReference fieldReference)
    //             {
    //                 //Debug.Log($"- Поле: {fieldReference.Name} (тип: {fieldReference.FieldType}, класс: {fieldReference.DeclaringType})");

    //                 var fieldInfo = instance.GetType().GetField(fieldReference.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //                 if (fieldInfo != null)
    //                 {
    //                     var fieldValue = fieldInfo.GetValue(instance);
    //                     if (IsTypeReactiveProperty(fieldValue.GetType())) reactiveProperties.Add(fieldValue);
    //                 }
    //             }
    //         }
    //     }

    //     AddSubscriptionsToReactiveProperties(processor, reactiveProperties);
    // }


    // public void AddSubscriptionsToReactivePropertiesFromProcessor(Processor<T> processor, object instance)
    // {
    //     var methodInfo = processor.Method;
    //     var assemblyPath = methodInfo.DeclaringType.Assembly.Location;
    //     var assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);
    //     var typeDefinition = assemblyDefinition.MainModule.GetType(methodInfo.DeclaringType.FullName);
    //     var methodDefinition = typeDefinition.Methods.FirstOrDefault<MethodDefinition>(m => m.Name == methodInfo.Name);

    //     if (methodDefinition == null)
    //     {
    //         Debug.LogError("Method is not found.");
    //         return;
    //     }

    //     List<object> reactiveProperties = new List<object>();

    //     foreach (var instruction in methodDefinition.Body.Instructions)
    //     {
    //         if (instruction.OpCode.Code == Mono.Cecil.Cil.Code.Ldfld)
    //         {
    //             // Обработка поля
    //             if (instruction.Operand is FieldReference fieldReference)
    //             {
    //                 HandleFieldReference(methodDefinition, instruction, instance, reactiveProperties);
    //             }
    //         }
    //         else if (instruction.OpCode.Code == Mono.Cecil.Cil.Code.Call || instruction.OpCode.Code == Mono.Cecil.Cil.Code.Callvirt)
    //         {
    //             // Обработка свойства
    //             if (instruction.Operand is MethodReference methodReference && methodReference.Name.StartsWith("get_"))
    //             {
    //                 HandlePropertyReference(methodDefinition, instruction, instance, reactiveProperties);
    //             }
    //         }
    //     }

    //     AddSubscriptionsToReactiveProperties(processor, reactiveProperties);
    // }

    // private void HandleFieldReference(MethodDefinition methodDefinition, Mono.Cecil.Cil.Instruction instruction, object instance, List<object> reactiveProperties)
    // {
    //     var fieldReference = (FieldReference)instruction.Operand;
    //     var instanceReference = FindInstanceBeforeLdfld(methodDefinition, instruction, instance);

    //     if (instanceReference != null)
    //     {
    //         var fieldInfo = instanceReference.GetType().GetField(fieldReference.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //         if (fieldInfo != null)
    //         {
    //             var fieldValue = fieldInfo.GetValue(instanceReference);
    //             if (IsTypeReactiveProperty(fieldValue.GetType()) && !reactiveProperties.Contains(fieldValue))
    //             {
    //                 reactiveProperties.Add(fieldValue);
    //             }
    //         }
    //     }
    // }

    // private void HandlePropertyReference(MethodDefinition methodDefinition, Mono.Cecil.Cil.Instruction instruction, object instance, List<object> reactiveProperties)
    // {
    //     var methodReference = (MethodReference)instruction.Operand;
    //     var declaringType = Type.GetType(methodReference.DeclaringType.FullName);
    //     if (declaringType == null) return;

    //     var propertyName = methodReference.Name.Substring(4); // Убираем "get_"
    //     var propertyInfo = declaringType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //     if (propertyInfo == null) return;

    //     var instanceReference = FindInstanceBeforeLdfld(methodDefinition, instruction, instance);

    //     if (instanceReference != null)
    //     {
    //         var propertyValue = propertyInfo.GetValue(instanceReference);
    //         if (propertyValue != null)
    //         {
    //             if (IsTypeReactiveProperty(propertyValue.GetType()) && !reactiveProperties.Contains(propertyValue))
    //             {
    //                 reactiveProperties.Add(propertyValue);
    //             }
    //             else if (propertyValue.GetType().IsClass)
    //             {
    //                 // Рекурсивно обрабатываем поле, возвращаемое свойством
    //                 ProcessFieldFromProperty(propertyValue, reactiveProperties);
    //             }
    //         }
    //     }
    // }

    // private void ProcessFieldFromProperty(object propertyValue, List<object> reactiveProperties)
    // {
    //     var type = propertyValue.GetType();
    //     var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    //     foreach (var field in fields)
    //     {
    //         var fieldValue = field.GetValue(propertyValue);
    //         if (fieldValue != null && IsTypeReactiveProperty(fieldValue.GetType()) && !reactiveProperties.Contains(fieldValue))
    //         {
    //             reactiveProperties.Add(fieldValue);
    //         }
    //     }
    // }

    // private object FindInstanceBeforeLdfld(MethodDefinition methodDefinition, Mono.Cecil.Cil.Instruction ldfldInstruction, object rootInstance)
    // {
    //     var instructions = methodDefinition.Body.Instructions;
    //     var currentIndex = instructions.IndexOf(ldfldInstruction);

    //     for (int i = currentIndex - 1; i >= 0; i--)
    //     {
    //         var instruction = instructions[i];

    //         if (instruction.OpCode.Code == Mono.Cecil.Cil.Code.Ldarg_0) // this
    //         {
    //             return rootInstance;
    //         }

    //         if (instruction.OpCode.Code == Mono.Cecil.Cil.Code.Ldloc || instruction.OpCode.Code == Mono.Cecil.Cil.Code.Ldloc_S)
    //         {
    //             Debug.LogWarning("Обработка локальных переменных не реализована.");
    //             return null;
    //         }

    //         if (instruction.OpCode.Code == Mono.Cecil.Cil.Code.Ldsfld)
    //         {
    //             var fieldReference = instruction.Operand as FieldReference;
    //             if (fieldReference != null)
    //             {
    //                 var declaringType = Type.GetType(fieldReference.DeclaringType.FullName);
    //                 var fieldInfo = declaringType?.GetField(fieldReference.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    //                 return fieldInfo?.GetValue(null);
    //             }
    //         }

    //         if (instruction.OpCode.Code == Mono.Cecil.Cil.Code.Ldfld)
    //         {
    //             // Рекурсивный вызов для обработки цепочек
    //             var previousInstruction = FindInstanceBeforeLdfld(methodDefinition, instructions[i], rootInstance);
    //             if (previousInstruction != null)
    //             {
    //                 var fieldReference = instruction.Operand as FieldReference;
    //                 if (fieldReference != null)
    //                 {
    //                     var fieldInfo = previousInstruction.GetType().GetField(fieldReference.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //                     return fieldInfo?.GetValue(previousInstruction);
    //                 }
    //             }
    //         }
    //     }

    //     return null;
    // }
