using System;
using System.Collections.Generic;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine;
using UnityEngine.Networking;

namespace Zyq.Weaver
{
    public static class BaseTypeFactory
    {
        private class TypeWrapper
        {
            public Type type;
            public string readMethod;
            public string writeMethod;

            public TypeWrapper(Type _type, string _readMethod, string _writeMethod)
            {
                type = _type;
                readMethod = _readMethod;
                writeMethod = _writeMethod;
            }
        }

        private static Dictionary<string, TypeWrapper> ReadMappers = new Dictionary<string, TypeWrapper>();

        static BaseTypeFactory()
        {
            ReadMappers.Add("System.Byte", new TypeWrapper(typeof(byte), "ReadByte", "Write"));
            ReadMappers.Add("System.Boolean", new TypeWrapper(typeof(bool), "ReadBoolean", "Write"));
            ReadMappers.Add("System.Int16", new TypeWrapper(typeof(short), "ReadInt16", "Write"));
            ReadMappers.Add("System.Int32", new TypeWrapper(typeof(int), "ReadInt32", "Write"));
            ReadMappers.Add("System.Int64", new TypeWrapper(typeof(long), "ReadInt64", "Write"));
            ReadMappers.Add("System.UInt16", new TypeWrapper(typeof(ushort), "ReadUInt16", "Write"));
            ReadMappers.Add("System.UInt32", new TypeWrapper(typeof(uint), "ReadUInt32", "Write"));
            ReadMappers.Add("System.UInt64", new TypeWrapper(typeof(ulong), "ReadUInt64", "Write"));
            ReadMappers.Add("System.Single", new TypeWrapper(typeof(float), "ReadSingle", "Write"));
            ReadMappers.Add("System.Double", new TypeWrapper(typeof(double), "ReadDouble", "Write"));
            ReadMappers.Add("System.String", new TypeWrapper(typeof(string), "ReadString", "Write"));
            ReadMappers.Add("UnityEngine.Vector2", new TypeWrapper(typeof(Vector2), "ReadVector2", "Write"));
            ReadMappers.Add("UnityEngine.Vector3", new TypeWrapper(typeof(Vector3), "ReadVector3", "Write"));
            ReadMappers.Add("UnityEngine.Vector4", new TypeWrapper(typeof(Vector4), "ReadVector4", "Write"));
            ReadMappers.Add("UnityEngine.Quaternion", new TypeWrapper(typeof(Quaternion), "ReadQuaternion", "Write"));
        }

        public static bool IsBaseType(string type)
        {
            return ReadMappers.ContainsKey(type);
        }

        public static Instruction CreateReadTypeInstruction(ModuleDefinition module, ILProcessor processor, string type)
        {
            TypeWrapper wrapper = null;

            if (ReadMappers.TryGetValue(type, out wrapper))
            {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod(wrapper.readMethod, Type.EmptyTypes)));
            }

            return null;
        }

        public static Instruction CreateWriteTypeInstruction(ModuleDefinition module, ILProcessor processor, string type)
        {
            TypeWrapper wrapper = null;

            if (ReadMappers.TryGetValue(type, out wrapper))
            {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkWriter).GetMethod(wrapper.writeMethod, new Type[] { wrapper.type })));
            }

            return null;
        }
    }
}