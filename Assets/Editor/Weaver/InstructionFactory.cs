using System;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine.Networking;

namespace Zyq.Weaver {
    public static class InstructionFactory {
        public static Instruction CreateReadTypeInstruction(ModuleDefinition module, ILProcessor processor, string type) {
            if (type == "System.Int16") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadInt16", Type.EmptyTypes)));
            } else if (type == "System.Int32") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadInt32", Type.EmptyTypes)));
            } else if (type == "System.Int64") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadInt64", Type.EmptyTypes)));
            } else if (type == "System.UInt16") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadUInt16", Type.EmptyTypes)));
            } else if (type == "System.UInt32") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadUInt32", Type.EmptyTypes)));
            } else if (type == "System.UInt64") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadUInt64", Type.EmptyTypes)));
            } else if (type == "System.Single") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadSingle", Type.EmptyTypes)));
            } else if (type == "System.Double") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadDouble", Type.EmptyTypes)));
            } else if (type == "System.String") {
                return processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(NetworkReader).GetMethod("ReadString", Type.EmptyTypes)));
            }
            throw new Exception("无法确定的类型:" + type);
        }
    }
}