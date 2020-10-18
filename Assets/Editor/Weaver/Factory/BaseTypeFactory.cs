using System;
using UnityEngine;
using Mono.CecilX;
using Mono.CecilX.Cil;
using System.Collections.Generic;
using Zyq.Game.Base;

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
            Mappers.Add("System.Int16", new TypeWrapper(typeof(short), "ReadShort", "Write"));
            Mappers.Add("System.Int32", new TypeWrapper(typeof(int), "ReadInt", "Write"));
            Mappers.Add("System.Int64", new TypeWrapper(typeof(long), "ReadLong", "Write"));
            Mappers.Add("System.UInt16", new TypeWrapper(typeof(ushort), "ReadUShort", "Write"));
            Mappers.Add("System.UInt32", new TypeWrapper(typeof(uint), "ReadUInt", "Write"));
            Mappers.Add("System.UInt64", new TypeWrapper(typeof(ulong), "ReadULong", "Write"));
            Mappers.Add("System.Single", new TypeWrapper(typeof(float), "ReadFloat", "Write"));
            Mappers.Add("System.Double", new TypeWrapper(typeof(double), "ReadDouble", "Write"));
            Mappers.Add("System.String", new TypeWrapper(typeof(string), "ReadString", "Write"));
            Mappers.Add("UnityEngine.Vector2", new TypeWrapper(typeof(Vector2), "ReadVector2", "Write"));
            Mappers.Add("UnityEngine.Vector3", new TypeWrapper(typeof(Vector3), "ReadVector3", "Write"));
            Mappers.Add("UnityEngine.Vector4", new TypeWrapper(typeof(Vector4), "ReadVector4", "Write"));
            Mappers.Add("UnityEngine.Quaternion", new TypeWrapper(typeof(Quaternion), "ReadQuaternion", "Write"));
        }

        public static bool IsBaseType(TypeDefinition type)
        {
            return type.IsEnum || Mappers.ContainsKey(type.ToString());
        }

        public static bool IsSystemBaseType(TypeDefinition type)
        {
            return type.IsEnum || (Mappers.ContainsKey(type.FullName) && type.FullName.IndexOf("System.") >= 0);
        }

        public static Instruction CreateReadInstruction(ModuleDefinition module, ILProcessor processor, TypeDefinition type)
        {
            return CreateReadInstruction(module, processor, type.IsEnum ? typeof(int).ToString() : type.FullName);
        }

        public static Instruction CreateReadInstruction(ModuleDefinition module, ILProcessor processor, string type)
        {
            TypeWrapper wrapper = null;

            if (Mappers.TryGetValue(type, out wrapper))
            {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(ByteBuffer).GetMethod(wrapper.readMethod, Type.EmptyTypes)));
            }

            return null;
        }

        public static Instruction CreateWriteInstruction(ModuleDefinition module, ILProcessor processor, TypeDefinition type)
        {
            return CreateWriteInstruction(module, processor, type.IsEnum ? typeof(int).FullName : type.FullName);
        }

        public static Instruction CreateWriteInstruction(ModuleDefinition module, ILProcessor processor, string type)
        {
            TypeWrapper wrapper = null;

            if (Mappers.TryGetValue(type, out wrapper))
            {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(ByteBuffer).GetMethod(wrapper.writeMethod, new Type[] { wrapper.type })));
            }

            return null;
        }
    }
}