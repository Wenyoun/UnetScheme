using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using UnityEngine;

namespace Zyq.Weaver {
    public static class WeaverProgram {
        public static ModuleDefinition CorlibModule;
        public static AssemblyDefinition UnityAssembly;
        public static AssemblyDefinition BaseAssembly;
        public static AssemblyDefinition NetworkingAssembly;
        public static AssemblyDefinition CurrentAssembly;

        #region  networking
        public static TypeReference NetworkWriterType;
        public static TypeReference NetworkReaderType;
        #endregion

        #region  attribute
        public static TypeReference SendType;
        public static TypeReference RecvType;
        public static TypeReference ProtocolType;
        public static TypeReference BroadcastType;
        #endregion

        #region 基本类型
        public static TypeReference singleType;
        public static TypeReference voidType;
        public static TypeReference doubleType;
        public static TypeReference boolType;
        public static TypeReference int64Type;
        public static TypeReference uint64Type;
        public static TypeReference int32Type;
        public static TypeReference uint32Type;
        public static TypeReference objectType;
        public static TypeReference typeType;
        #endregion

        public static bool WeaveAssemblies(string unityEngineDLL, string networkingDLL, string baseModuleRuntimeDLL, string assemblyPath, string[] depAssemblyPaths) {
            using(UnityAssembly = AssemblyDefinition.ReadAssembly(unityEngineDLL))
            using(NetworkingAssembly = AssemblyDefinition.ReadAssembly(networkingDLL))
            using(BaseAssembly = AssemblyDefinition.ReadAssembly(baseModuleRuntimeDLL)) {
                SetupUnityTypes();
                try {
                    if (!Weave(unityEngineDLL, networkingDLL, assemblyPath, depAssemblyPaths)) {
                        return false;
                    }
                } catch (Exception e) {
                    Debug.LogError("Exception:" + e);
                }
            }
            return true;
        }

        private static bool Weave(string unityEngineDLL, string networkingDLL, string assemblyPath, string[] depAssemblyPaths) {
            using(DefaultAssemblyResolver asmResolver = new DefaultAssemblyResolver())
            using(CurrentAssembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { ReadWrite = true, ReadSymbols = true, AssemblyResolver = asmResolver })) {
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
                asmResolver.AddSearchDirectory(Helpers.FindUnityEngineDLLDirectoryName());
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(unityEngineDLL));
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(networkingDLL));

                foreach (string path in depAssemblyPaths) {
                    asmResolver.AddSearchDirectory(path);
                }

                SetupCorlib();
                SetupTargetTypes();

                bool result = WeaveModule(CurrentAssembly.MainModule);
                if (result) {
                    WriterParameters writeParams = new WriterParameters { WriteSymbols = true };
                    CurrentAssembly.Write(writeParams);
                }
            }
            return true;
        }

        private static bool WeaveModule(ModuleDefinition module) {
            if (module.Name.EndsWith("Zyq.Game.Client.dll")) {
                return ClientWeaver.Weave(module);
            } else if (module.Name.EndsWith("Zyq.Game.Server.dll")) {
                return ServerWeaver.Weave(module);
            }
            return false;
        }

        private static void SetupUnityTypes() {
            NetworkWriterType = NetworkingAssembly.MainModule.GetType("UnityEngine.Networking.NetworkWriter");
            NetworkReaderType = NetworkingAssembly.MainModule.GetType("UnityEngine.Networking.NetworkReader");
        }

        private static void SetupCorlib() {
            AssemblyNameReference name = AssemblyNameReference.Parse("mscorlib");
            ReaderParameters parameters = new ReaderParameters {
                AssemblyResolver = CurrentAssembly.MainModule.AssemblyResolver
            };
            CorlibModule = CurrentAssembly.MainModule.AssemblyResolver.Resolve(name, parameters).MainModule;
        }

        private static void SetupTargetTypes() {
            voidType = ImportCorlibType("System.Void");
            singleType = ImportCorlibType("System.Single");
            doubleType = ImportCorlibType("System.Double");
            boolType = ImportCorlibType("System.Boolean");
            int64Type = ImportCorlibType("System.Int64");
            uint64Type = ImportCorlibType("System.UInt64");
            int32Type = ImportCorlibType("System.Int32");
            uint32Type = ImportCorlibType("System.UInt32");
            objectType = ImportCorlibType("System.Object");
            typeType = ImportCorlibType("System.Type");

            SendType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.SendAttribute");
            RecvType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.RecvAttribute");
            ProtocolType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.ProtocolAttribute");
            BroadcastType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.BroadcastAttribute");
        }

        private static TypeReference ImportCorlibType(string fullName) {
            TypeDefinition type = CorlibModule.GetType(fullName) ?? CorlibModule.ExportedTypes.First(t => t.FullName == fullName).Resolve();
            if (type != null) {
                return CurrentAssembly.MainModule.ImportReference(type);
            }
            return null;
        }
    }
}