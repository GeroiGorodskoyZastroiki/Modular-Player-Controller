using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Extensions.SystemExtensions
{
    public static class TypeExtensions
    {
        private static Dictionary<Type, string> PrimitiveTypesLookup = new Dictionary<Type, string>
        {
            { typeof(bool), "Bool" },
            { typeof(byte), "Byte" },
            { typeof(char), "Char" },
            { typeof(decimal), "Decimal" },
            { typeof(double), "Double" },
            { typeof(float), "Float" },
            { typeof(int), "Int" },
            { typeof(long), "Long" },
            { typeof(sbyte), "Sbyte" },
            { typeof(short), "Short" },
            { typeof(string), "String" },
            { typeof(uint), "Uint" },
            { typeof(ulong), "Ulong" },
            { typeof(ushort), "Ushort" },
        };

        public static string NicifyTypeName(this Type type)
        {
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                string elementTypeString = NicifyTypeName(elementType);
                return $"{elementTypeString}[]";
            }

            if (!type.IsGenericType)
            {
                return GetSingleTypeName(type);
            }

            if (type.IsNested && type.DeclaringType.IsGenericType) return type.Name;

            string genericTypeString = $"{type.Name[..type.Name.IndexOf('`')]}";
            
            IEnumerable<string> genericArgumentNames = type.GetGenericArguments()
                .Select(x => NicifyTypeName(x));

            string genericArgumentsString = string.Join(", ", genericArgumentNames);

            return $"{genericTypeString}<{genericArgumentsString}>";
        }

        private static string GetSingleTypeName(Type type)
        {
            if (PrimitiveTypesLookup.TryGetValue(type, out string result)) return result;

            return type.Name;
        }
    }
}