using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine;
using UnityEngine.Networking;

namespace Zyq.Weaver
{
    public static class WeaverProgram
    {
        public static ModuleDefinition CorlibModule;
        public static AssemblyDefinition UnityAssembly;
        public static AssemblyDefinition BaseAssembly;
        public static AssemblyDefinition NetworkingAssembly;
        public static AssemblyDefinition CurrentAssembly;

        #region  networking
        public static TypeReference NetworkWriterType;
        public static MethodReference NetworkWriterCtorMethod;
        public static MethodReference NetworkWriterStartMessageMethod;
        public static MethodReference NetworkWriterFinishMessageMethod;
        public static TypeReference NetworkReaderType;
        #endregion

        #region  attribute
        public static TypeReference SendType;
        public static TypeReference RecvType;
        public static TypeReference ProtocolType;
        public static TypeReference BroadcastType;
        public static TypeReference SyncClassType;
        public static TypeReference SyncFieldType;
        #endregion

        public static bool WeaveAssemblies(string unityEngineDLL, string networkingDLL, string baseModuleRuntimeDLL, string assemblyPath, string[] depAssemblyPaths)
        {
            using (UnityAssembly = AssemblyDefinition.ReadAssembly(unityEngineDLL))
            using (NetworkingAssembly = AssemblyDefinition.ReadAssembly(networkingDLL))
            using (BaseAssembly = AssemblyDefinition.ReadAssembly(baseModuleRuntimeDLL))
            {
                SetupUnityTypes();
                try
                {
                    if (!Weave(unityEngineDLL, networkingDLL, assemblyPath, depAssemblyPaths))
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception:" + e);
                }
            }
            return true;
        }

        private static bool Weave(string unityEngineDLL, string networkingDLL, string assemblyPath, string[] depAssemblyPaths)
        {
            using (DefaultAssemblyResolver asmResolver = new DefaultAssemblyResolver())
            using (CurrentAssembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { ReadWrite = true, ReadSymbols = true, AssemblyResolver = asmResolver }))
            {
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
                asmResolver.AddSearchDirectory(Helpers.FindUnityEngineDLLDirectoryName());
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(unityEngineDLL));
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(networkingDLL));

                foreach (string path in depAssemblyPaths)
                {
                    asmResolver.AddSearchDirectory(path);
                }

                SetupCorlib();
                SetupTargetTypes();

                bool result = WeaveModule(CurrentAssembly.MainModule);
                if (result)
                {
                    WriterParameters writeParams = new WriterParameters { WriteSymbols = true };
                    CurrentAssembly.Write(writeParams);
                }
            }
            return true;
        }

        private static bool WeaveModule(ModuleDefinition module)
        {
            if (module.Name.EndsWith("Zyq.Game.Client.dll"))
            {
                return ClientWeaver.Weave(module);
            }
            else if (module.Name.EndsWith("Zyq.Game.Server.dll"))
            {
                return ServerWeaver.Weave(module);
            }
            return false;
        }

        private static void SetupUnityTypes()
        {
            NetworkWriterType = NetworkingAssembly.MainModule.GetType("UnityEngine.Networking.NetworkWriter");

            NetworkWriterCtorMethod = NetworkWriterType.Resolve().Methods.Single(m => m.FullName.IndexOf(".ctor()") >= 0);
            NetworkWriterStartMessageMethod = NetworkWriterType.Resolve().Methods.Single(m => m.FullName.IndexOf("StartMessage") >= 0);
            NetworkWriterFinishMessageMethod = NetworkWriterType.Resolve().Methods.Single(m => m.FullName.IndexOf("FinishMessage") >= 0);

            NetworkReaderType = NetworkingAssembly.MainModule.GetType("UnityEngine.Networking.NetworkReader");
        }

        private static void SetupCorlib()
        {
            AssemblyNameReference name = AssemblyNameReference.Parse("mscorlib");
            ReaderParameters parameters = new ReaderParameters
            {
                AssemblyResolver = CurrentAssembly.MainModule.AssemblyResolver
            };
            CorlibModule = CurrentAssembly.MainModule.AssemblyResolver.Resolve(name, parameters).MainModule;
        }

        private static void SetupTargetTypes()
        {
            SendType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.SendAttribute");
            RecvType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.RecvAttribute");
            ProtocolType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.ProtocolAttribute");
            BroadcastType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.BroadcastAttribute");
            SyncClassType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.SyncClassAttribute");
            SyncFieldType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.SyncFieldAttribute");
        }

        private static TypeReference ImportCorlibType(string fullName)
        {
            TypeDefinition type = CorlibModule.GetType(fullName) ?? CorlibModule.ExportedTypes.First(t => t.FullName == fullName).Resolve();
            if (type != null)
            {
                return CurrentAssembly.MainModule.ImportReference(type);
            }
            return null;
        }
    }
}