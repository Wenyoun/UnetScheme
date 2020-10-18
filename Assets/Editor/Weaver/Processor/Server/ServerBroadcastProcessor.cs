using Mono.CecilX;
using Mono.CecilX.Cil;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class ServerBroadcastProcessor
    {
        public static void Weave(ModuleDefinition module, Dictionary<ushort, MethodDefinition> methods)
        {
            foreach (ushort msgId in methods.Keys)
            {
                MethodDefinition method = methods[msgId];

                if (!CheckHelper.CheckMethodParams(WeaverProgram.Server, method))
                {
                    continue;
                }

                ILProcessor processor = method.Body.GetILProcessor();
                method.Body.Variables.Clear();
                method.Body.Instructions.Clear();

                method.Body.Variables.Add(new VariableDefinition(module.ImportReference(WeaverProgram.ByteBufferType)));

                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ldc_I4, 1400));
                processor.Append(processor.Create(OpCodes.Call, module.ImportReference(WeaverProgram.ByteBufferAllocateMethod)));
                processor.Append(processor.Create(OpCodes.Stloc_0));

                Collection<ParameterDefinition> parms = method.Parameters;
                for (int i = 0; i < parms.Count; ++i)
                {
                    ParameterDefinition parm = parms[i];
                    TypeDefinition parmType = parm.ParameterType.Resolve();
                    byte index = (byte)(method.IsStatic ? i : i + 1);

                    if (parm.ParameterType.IsArray)
                    {
                        ArrayWriteFactory.CreateMethodParamWriteInstruction(module, method, processor, parmType, index);
                    }
                    else if (BaseTypeFactory.IsBaseType(parmType))
                    {
                        processor.Append(processor.Create(OpCodes.Ldloc_0));
                        processor.Append(processor.Create(OpCodes.Ldarg_S, index));
                        processor.Append(BaseTypeFactory.CreateWriteInstruction(module, processor, parmType));
                    }
                    else if (parmType.IsValueType)
                    {
                        processor.Append(processor.Create(OpCodes.Ldarga_S, index));
                        processor.Append(processor.Create(OpCodes.Ldloc_0));
                        processor.Append(processor.Create(OpCodes.Call, StructMethodFactory.FindSerialize(module, parmType)));
                    }
                }

                processor.Append(processor.Create(OpCodes.Ldsfld, module.ImportReference(WeaverProgram.ServerInsField)));
                processor.Append(processor.Create(OpCodes.Ldc_I4, msgId));
                processor.Append(processor.Create(OpCodes.Ldloc_0));
                processor.Append(processor.Create(OpCodes.Callvirt, module.ImportReference(WeaverProgram.ServerSendMethod)));
                processor.Append(processor.Create(OpCodes.Nop));
                processor.Append(processor.Create(OpCodes.Ret));
            }
        }
    }
}