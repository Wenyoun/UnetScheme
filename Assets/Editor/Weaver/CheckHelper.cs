﻿using UnityEngine;
using Mono.CecilX;
using Mono.Collections.Generic;
using Nice.Game.Base;

namespace Zyq.Weaver
{
    public static class CheckHelper
    {
        public static bool CheckStructFiled(string module, TypeDefinition type, TypeDefinition fieldType)
        {
            if (fieldType.FullName == typeof(byte[]).FullName)
            {
                return true;
            }

            if (fieldType.FullName == typeof(ByteBuffer).FullName)
            {
                return true;
            }

            if (!BaseTypeFactory.IsBaseType(fieldType) && !fieldType.IsValueType)
            {
                Debug.LogError(module + ": [" + type.FullName + "]中的字段只能为[基本类型，字符串，枚举，结构体，基本类型数组，字符串数组，枚举数组，结构体数组]而不能为[" + fieldType.FullName + "]");
                return false;
            }

            return true;
        }

        public static bool CheckSyncFiled(string module, TypeDefinition type, TypeDefinition fieldType)
        {
            if (!BaseTypeFactory.IsBaseType(fieldType))
            {
                Debug.LogError(module + ": [" + type.FullName + "]中的字段只能为[基本类型，字符串，枚举]而不能为[" + fieldType.FullName + "]");
                return false;
            }
            return true;
        }

        public static bool CheckMethodParams(string module, MethodDefinition method)
        {
            Collection<ParameterDefinition> parms = method.Parameters;
            for (int i = 0; i < parms.Count; ++i)
            {
                ParameterDefinition parm = parms[i];
                TypeDefinition parmType = parm.ParameterType.Resolve();

                if (i == 0 && parmType.FullName == typeof(IConnection).FullName)
                {
                    Debug.LogError(module + ": 方法[" + method.FullName + "]中第一个参数不能为[" + parmType.FullName + "]");
                    return false;
                }

                if (parmType.FullName == typeof(byte[]).FullName)
                {
                    continue;
                }

                if (parmType.FullName == typeof(ByteBuffer).FullName)
                {
                    continue;
                }

                if (!BaseTypeFactory.IsBaseType(parmType) && !parmType.IsValueType)
                {
                    Debug.LogError(module + ": 方法[" + method.FullName + "]中的参数只能为[基本类型，字符串，枚举，结构体，基本类型数组，字符串数组，枚举数组，结构体数组]而不能为[" + parmType.FullName + "]");
                    return false;
                }
            }
            return true;
        }

        public static bool CheckMethodFirstParams(string module, MethodDefinition method)
        {
            Collection<ParameterDefinition> parms = method.Parameters;
            for (int i = 0; i < parms.Count; ++i)
            {
                ParameterDefinition parm = parms[i];
                TypeDefinition parmType = parm.ParameterType.Resolve();
                if (i == 0)
                {
                    if (parmType.FullName != typeof(IConnection).FullName)
                    {
                        Debug.LogError(module + ": 方法[" + method.FullName + "]中第一个参数只能为[" + typeof(IConnection).FullName + "]");
                        return false;
                    }
                }
                else if (!BaseTypeFactory.IsBaseType(parmType) && !parmType.IsValueType)
                {
                    if (parmType.FullName == typeof(byte[]).FullName)
                    {
                        continue;
                    }

                    if (parmType.FullName == typeof(ByteBuffer).FullName)
                    {
                        continue;
                    }

                    Debug.LogError(module + ": 方法[" + method.FullName + "]中的参数只能为[基本类型，字符串，枚举，结构体，基本类型数组，字符串数组，枚举数组，结构体数组]而不能为[" + parmType.FullName + "]");
                    return false;
                }
            }
            return true;
        }
    }
}