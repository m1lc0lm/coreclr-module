using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using AltV.Net.Data;
using AltV.Net.Elements.Args;
using AltV.Net.Elements.Entities;
using AltV.Net.Shared;
using AltV.Net.Shared.Elements.Entities;

namespace AltV.Net.FunctionParser
{
    internal static class FunctionObjectParsers
    {
        public static object ParseObject(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            if (core.MValueFromObject(value, type, out var result))
            {
                return result;
            }

            if (!type.IsValueType || type == FunctionTypes.String) return value;
            var defaultValue = typeInfo?.DefaultValue;
            return defaultValue ?? Activator.CreateInstance(type);
        }

        public static object ParseFunction(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            if (value is MValueFunctionCallback function)
            {
                return (Function.Func) new FunctionWrapper(core, function).Call;
            }

            return null;
        }

        public static object ParseBool(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            switch (value)
            {
                case bool _:
                    return value;
                case string stringValue:
                    switch (stringValue)
                    {
                        case "true":
                            return true;
                        case "false":
                            return false;
                    }

                    break;
            }

            return null;
        }

        public static object ParseSByte(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (sbyte) longValue,
                    ulong ulongValue => (sbyte) ulongValue,
                    double doubleValue => (sbyte) doubleValue,
                    string stringValue when sbyte.TryParse(stringValue, out var sbyteValue) => sbyteValue,
                    bool boolValue => boolValue ? (sbyte) 1 : (sbyte) 0,
                    _ => default
                };
            }
        }

        public static object ParseShort(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (short) longValue,
                    ulong ulongValue => (short) ulongValue,
                    double doubleValue => (short) doubleValue,
                    string stringValue when short.TryParse(stringValue, out var shortValue) => shortValue,
                    bool boolValue => boolValue ? (short) 1 : (short) 0,
                    _ => default
                };
            }
        }

        public static object ParseInt(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (int) longValue,
                    ulong ulongValue => (int) ulongValue,
                    double doubleValue => (int) doubleValue,
                    string stringValue when int.TryParse(stringValue, out var intValue) => intValue,
                    bool boolValue => boolValue ? 1 : 0,
                    _ => default
                };
            }
        }

        public static object ParseLong(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => longValue,
                    ulong ulongValue => (long) ulongValue,
                    double doubleValue => (long) doubleValue,
                    string stringValue when long.TryParse(stringValue, out var longValue) => longValue,
                    bool boolValue => boolValue ? 1L : 0L,
                    _ => default
                };
            }
        }

        public static object ParseByte(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (byte) longValue,
                    ulong ulongValue => (byte) ulongValue,
                    double doubleValue => (byte) doubleValue,
                    string stringValue when byte.TryParse(stringValue, out var byteValue) => byteValue,
                    bool boolValue => boolValue ? (byte) 1 : (byte) 0,
                    _ => default
                };
            }
        }

        public static object ParseUShort(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (ushort) longValue,
                    ulong ulongValue => (ushort) ulongValue,
                    double doubleValue => (ushort) doubleValue,
                    string stringValue when ushort.TryParse(stringValue, out var ushortValue) => ushortValue,
                    bool boolValue => boolValue ? (ushort) 1 : (ushort) 0,
                    _ => default
                };
            }
        }

        public static object ParseUInt(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (uint) longValue,
                    ulong ulongValue => (uint) ulongValue,
                    double doubleValue => (uint) doubleValue,
                    string stringValue when uint.TryParse(stringValue, out var uintValue) => uintValue,
                    bool boolValue => boolValue ? 1U : 0U,
                    _ => default
                };
            }
        }

        public static object ParseULong(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (ulong) longValue,
                    ulong ulongValue => ulongValue,
                    double doubleValue => (ulong) doubleValue,
                    string stringValue when ulong.TryParse(stringValue, out var ulongValue) => ulongValue,
                    bool boolValue => boolValue ? 1UL : 0UL,
                    _ => default
                };
            }
        }

        public static object ParseFloat(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (float) longValue,
                    ulong ulongValue => (float) ulongValue,
                    double doubleValue => (float) doubleValue,
                    string stringValue when float.TryParse(stringValue, out var floatValue) => floatValue,
                    bool boolValue => boolValue ? 1.0f : 0.0f,
                    _ => default
                };
            }
        }

        public static object ParseDouble(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            unchecked
            {
                return value switch
                {
                    long longValue => (double) longValue,
                    ulong ulongValue => (double) ulongValue,
                    double doubleValue => doubleValue,
                    string stringValue when double.TryParse(stringValue, out var doubleValue) => doubleValue,
                    bool boolValue => boolValue ? 1.0 : 0.0,
                    _ => default
                };
            }
        }

        public static object ParseString(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            return value switch
            {
                long longValue => longValue.ToString(CultureInfo.InvariantCulture),
                ulong ulongValue => ulongValue.ToString(CultureInfo.InvariantCulture),
                double doubleValue =>doubleValue.ToString(CultureInfo.InvariantCulture),
                string stringValue => stringValue,
                bool boolValue => boolValue ? "true" : "false",
                _ => default
            };
        }

        public static object ParseEntity(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            if (value is not ISharedEntity entity) return null;
            return type == FunctionTypes.Obj || value.GetType().IsAssignableTo(type) ? entity : null;
        }
        
        public static object ParseArray(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            if (!(value is object[] objects)) return null;
            var length = objects.Length;
            var elementType = typeInfo?.ElementType ?? type.GetElementType();
            if (elementType == FunctionTypes.Obj) return objects;

            if (elementType == FunctionTypes.String)
            {
                var stringArray = new string[length];
                for (var i = 0; i < length; i++)
                {
                    var obj = objects[i];
                    switch (obj)
                    {
                        case null:
                            stringArray[i] = null;
                            break;
                        case string stringValue:
                            stringArray[i] = stringValue;
                            break;
                    }
                }

                return stringArray;
            }

            //TODO: optimize like in MValueParsers for default arrays,
            //TODO: and add IVehicle, IPlayer, IBlip and ICheckpoint types as well for optimization in both parsers

            object defaultValue = null;
            var defaultValueSet = false;
            var nullableDefaultValue = typeInfo?.Element?.DefaultValue;
            if (nullableDefaultValue != null)
            {
                defaultValue = nullableDefaultValue;
                defaultValueSet = true;
            }

            var typedArray = Array.CreateInstance(elementType, length);

            for (var i = 0; i < length; i++)
            {
                var curr = objects[i];
                if (curr == null)
                {
                    if (defaultValueSet)
                    {
                        typedArray.SetValue(defaultValue, i);
                    }
                    else
                    {
                        if (type.IsValueType && type != FunctionTypes.String)
                        {
                            defaultValue = Activator.CreateInstance(type);
                        }

                        defaultValueSet = true;
                        typedArray.SetValue(defaultValue, i);
                    }
                }
                else
                {
                    if ((typeInfo?.Element?.IsMValueConvertible == true || typeInfo?.Element == null) &&
                        core.MValueFromObject(curr, elementType, out var result))
                    {
                        typedArray.SetValue(result, i);
                    }
                    else
                    {
                        typedArray.SetValue(curr is IConvertible ? Convert.ChangeType(curr, elementType) : curr, i);
                    }
                }
            }

            return typedArray;
        }

        public static object ParseDictionary(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            if (!(value is Dictionary<string, object> dictionary)) return null;
            var args = typeInfo?.GenericArguments ?? type.GetGenericArguments();
            if (args.Length != 2) return null;
            var keyType = args[0];
            if (keyType != FunctionTypes.String) return null;
            var valueType = args[1];
            IDictionary typedDictionary;
            if (typeInfo != null)
            {
                typedDictionary = typeInfo.CreateDictionary();
            }
            else
            {
                var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                typedDictionary = (IDictionary) Activator.CreateInstance(dictType);
            }

            object defaultValue = null;
            var defaultValueSet = false;
            var nullableDefaultValue = typeInfo?.DictionaryValue?.DefaultValue;
            if (nullableDefaultValue != null)
            {
                defaultValue = nullableDefaultValue;
                defaultValueSet = true;
            }

            foreach (var (key, obj) in dictionary)
            {
                if (obj == null)
                {
                    if (defaultValueSet)
                    {
                        typedDictionary[key] = defaultValue;
                    }
                    else
                    {
                        if (type.IsValueType && type != FunctionTypes.String)
                        {
                            defaultValue = Activator.CreateInstance(type);
                        }

                        defaultValueSet = true;
                        typedDictionary[key] = defaultValue;
                    }
                }
                else
                {
                    if ((typeInfo?.IsMValueConvertible == true || typeInfo == null) &&
                        core.MValueFromObject(obj, valueType, out var result))
                    {
                        typedDictionary[key] = result;
                    }
                    else
                    {
                        if (obj is IConvertible)
                        {
                            typedDictionary[key] = Convert.ChangeType(obj, valueType);
                        }
                        else
                        {
                            typedDictionary[key] = obj;
                        }
                    }
                }
            }

            return typedDictionary;
        }

        public static object ParseConvertible(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            if (!(value is IDictionary dictionary)) return null;
            core.CreateMValue(out var mValue, dictionary);
            if (!core.FromMValue(in mValue, type, out var obj))
            {
                mValue.Dispose();
                return null;
            }

            mValue.Dispose();
            return obj;
        }

        public static object ParseEnum(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            return !Enum.TryParse(type, value.ToString(), true, out var enumObject) ? null : enumObject;
        }
        
        public static object ParsePosition(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            return value is Position position ? position : default;
        }
        
        public static object ParseRotation(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            return value is Position position ? (Rotation) position : default;
        }
        
        public static object ParseVector3(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            return value is Position position ? (Vector3) position : default;
        }
        
        public static object ParseRgba(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            return value is Rgba rgba ? rgba : default;
        }
        
        public static object ParseByteArray(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo)
        {
            return value is byte[] bytes ? bytes : default;
        }
    }

    internal delegate object FunctionObjectParser(ISharedCore core, object value, Type type, FunctionTypeInfo typeInfo);
}