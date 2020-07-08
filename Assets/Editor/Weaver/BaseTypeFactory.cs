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

        private static Dictionary<string, TypeWrapper> Mappers = new Dictionary<string, TypeWrapper>();

        static BaseTypeFactory()
        {
            Mappers.Add("System.Byte", new TypeWrapper(typeof(byte), "ReadByte", "Write"));
            Mappers.Add("System.Boolean", new TypeWrapper(typeof(bool), "ReadBoolean", "Write"));
            Mappers.Add("System.Int16", new TypeWrapper(typeof(short), "ReadInt16", "Write"));
            Mappers.Add("System.Int32", new TypeWrapper(typeof(int), "ReadInt32", "Write"));
            Mappers.Add("System.Int64", new TypeWrapper(typeof(long), "ReadInt64", "Write"));
            Mappers.Add("System.UInt16", new TypeWrapper(typeof(ushort), "ReadUInt16", "Write"));
            Mappers.Add("System.UInt32", new TypeWrapper(typeof(uint), "ReadUInt32", "Write"));
            Mappers.Add("System.UInt64", new TypeWrapper(typeof(ulong), "ReadUInt64", "Write"));
            Mappers.Add("System.Single", new TypeWrapper(typeof(float), "ReadSingle", "Write"));
            Mappers.Add("System.Double", new TypeWrapper(typeof(double), "ReadDouble", "Write"));
            Mappers.Add("System.String", new TypeWrapper(typeof(string), "ReadString", "Write"));
            Mappers.Add("UnityEngine.Vector2", new TypeWrapper(typeof(Vector2), "ReadVector2", "Write"));
            Mappers.Add("UnityEngine.Vector3", new TypeWrapper(typeof(Vector3), "ReadVector3", "Write"));
            Mappers.Add("UnityEngine.Vector4", new TypeWrapper(typeof(Vector4), "ReadVector4", "Write"));
            Mappers.Add("UnityEngine.Quaternion", new TypeWrapper(typeof(Quaternion), "ReadQuaternion", "Write"));
        }

        public static bool IsBaseType(string type)
        {
            return Mappers.ContainsKey(type);
        }

        public static Instruction CreateReadInstruction(ModuleDefinition module, ILProcessor processor, string type)
        {
            TypeWrapper wrapper = null;

            if (Mappers.TryGetValue(type, out wrapper))
            {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod(wrapper.readMethod, Type.EmptyTypes)));
            }

            return null;
        }

        public static Instruction CreateWriteInstruction(ModuleDefinition module, ILProcessor processor, string type)
        {
            TypeWrapper wrapper = null;

            if (Mappers.TryGetValue(type, out wrapper))
            {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkWriter).GetMethod(wrapper.writeMethod, new Type[] { wrapper.type })));
            }

            return null;
        }
    }
}