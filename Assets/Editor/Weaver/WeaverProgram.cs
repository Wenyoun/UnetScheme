using System;
using System.IO;
using Mono.CecilX;
using UnityEngine;

namespace Zyq.Weaver
{
    public static class WeaverProgram
    {
        public const int SugBufferSize = 512;

        #region User Module
        public const string Base = "Nice.Game.Base.dll";
        public const string Server = "Nice.Game.Server.dll";
        public const string Client = "Nice.Game.Client.dll";
        #endregion

        #region AssemblyDefinition
        public static AssemblyDefinition UnityAssembly;
        public static AssemblyDefinition BaseAssembly;
        public static AssemblyDefinition CurrentAssembly;
        #endregion

        #region Attribute
        public static TypeReference SendType;
        public static TypeReference RecvType;
        public static TypeReference ProtocolType;
        public static TypeReference BroadcastType;
        public static TypeReference SyncClassType;
        public static TypeReference SyncFieldType;
        #endregion

        #region Net
        public static TypeReference ChannelMessageType;
        public static FieldReference ChannelMessageBufferField;
        public static TypeReference ByteBufferType;
        public static MethodReference ByteBufferAllocateMethod;
        public static TypeReference ByteUtilsType;
        public static MethodReference ByteUtilsReadMethod;
        public static MethodReference ByteUtilsWriteMethod;
        public static TypeReference ByteBufferUtilsType;
        public static MethodReference ByteBufferUtilsReadMethod;
        public static MethodReference ByteBufferUtilsWriteMethod;
        #endregion

        #region Nice.Game.Base.AbsCop
        public static TypeReference AbsCopType;
        public static MethodReference AbsCopOnRemoveMethod;
        public static MethodReference AbsCopGetEntityMethod;
        #endregion

        #region Nice.Game.Base.Connection
        public static TypeReference IConnectionType;
        public static MethodReference IConnectionRegisterHandlerMethod;
        public static MethodReference IConnectionUnregisterHandlerMethod;
        #endregion

        #region Nice.Game.Base
        public static TypeReference NetworkClientManagerType;
        public static MethodReference NetworkClientManagerSendMethod;
        #endregion

        #region Nice.Game.Server.Server
        public static TypeReference NetworkServerManagerType;
        public static MethodReference NetworkServerManagerSendMethod;
        public static MethodReference NetworkServerManagerBroadcastMethod;
        #endregion

        public static bool WeaveAssemblies(string unityEngineDLL, string baseModuleRuntimeDLL, string assemblyPath, string[] depAssemblyPaths)
        {
            using (UnityAssembly = AssemblyDefinition.ReadAssembly(unityEngineDLL))
            {
                try
                {
                    return Weave(unityEngineDLL, assemblyPath, depAssemblyPaths, baseModuleRuntimeDLL);
                }
                catch (Exception e)
                {
                    Debug.LogError("Weave出错啦: " + e);
                }
                finally
                {
                    if (UnityAssembly != null)
                    {
                        UnityAssembly.Dispose();
                        UnityAssembly = null;
                    }
                    if (BaseAssembly != null)
                    {
                        BaseAssembly.Dispose();
                        BaseAssembly = null;
                    }
                    if (CurrentAssembly != null)
                    {
                        CurrentAssembly.Dispose();
                        CurrentAssembly = null;
                    }
                }
                return false;
            }
        }

        private static bool Weave(string unityEngineDLL, string assemblyPath, string[] depAssemblyPaths, string baseModuleRuntimeDLL)
        {
            using (DefaultAssemblyResolver asmResolver = new DefaultAssemblyResolver())
            using (CurrentAssembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters {ReadWrite = true, ReadSymbols = true, AssemblyResolver = asmResolver}))
            {
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath));
                asmResolver.AddSearchDirectory(Helpers.FindUnityEngineDLLDirectoryName());
                asmResolver.AddSearchDirectory(Path.GetDirectoryName(unityEngineDLL));

                foreach (string path in depAssemblyPaths)
                {
                    asmResolver.AddSearchDirectory(path);
                }

                if (WeaveModule(CurrentAssembly.MainModule, baseModuleRuntimeDLL))
                {
                    try
                    {
                        CurrentAssembly.Write(new WriterParameters {WriteSymbols = true});
                    }
                    finally
                    {
                        asmResolver.Dispose();
                    }
                    return true;
                }
            }

            return false;
        }

        private static bool WeaveModule(ModuleDefinition module, string baseModuleRuntimeDLL)
        {
            if (module.Name.EndsWith(Client))
            {
                BaseAssembly = AssemblyDefinition.ReadAssembly(baseModuleRuntimeDLL);
                SetupBaseModuleTypes();
                return ClientWeaver.Weave(module);
            }

            if (module.Name.EndsWith(Server))
            {
                BaseAssembly = AssemblyDefinition.ReadAssembly(baseModuleRuntimeDLL);
                SetupBaseModuleTypes();
                return ServerWeaver.Weave(module);
            }

            return false;
        }

        private static void SetupBaseModuleTypes()
        {
            ChannelMessageType = BaseAssembly.MainModule.GetType("Nice.Game.Base.ChannelMessage");
            ChannelMessageBufferField = ResolveHelper.ResolveField(ChannelMessageType, "Buffer");
            ByteBufferType = BaseAssembly.MainModule.GetType("Nice.Game.Base.ByteBuffer");
            ByteBufferAllocateMethod = ResolveHelper.ResolveMethod(ByteBufferType, "Allocate");
            ByteUtilsType = BaseAssembly.MainModule.GetType("Nice.Game.Base.ByteUtils");
            ByteUtilsReadMethod = ResolveHelper.ResolveMethod(ByteUtilsType, "Read");
            ByteUtilsWriteMethod = ResolveHelper.ResolveMethod(ByteUtilsType, "Write");
            ByteBufferUtilsType = BaseAssembly.MainModule.GetType("Nice.Game.Base.ByteBufferUtils");
            ByteBufferUtilsReadMethod = ResolveHelper.ResolveMethod(ByteBufferUtilsType, "Read");
            ByteBufferUtilsWriteMethod = ResolveHelper.ResolveMethod(ByteBufferUtilsType, "Write");

            SendType = BaseAssembly.MainModule.GetType("Nice.Game.Base.SendAttribute");
            RecvType = BaseAssembly.MainModule.GetType("Nice.Game.Base.RecvAttribute");
            ProtocolType = BaseAssembly.MainModule.GetType("Nice.Game.Base.AutoProtocolAttribute");
            BroadcastType = BaseAssembly.MainModule.GetType("Nice.Game.Base.BroadcastAttribute");
            SyncClassType = BaseAssembly.MainModule.GetType("Nice.Game.Base.SyncClassAttribute");
            SyncFieldType = BaseAssembly.MainModule.GetType("Nice.Game.Base.SyncFieldAttribute");

            AbsCopType = BaseAssembly.MainModule.GetType("Nice.Game.Base.AbsCop");
            AbsCopOnRemoveMethod = ResolveHelper.ResolveMethod(AbsCopType, "OnRemove");
            AbsCopGetEntityMethod = ResolveHelper.ResolveMethod(AbsCopType, "get_Entity");

            IConnectionType = BaseAssembly.MainModule.GetType("Nice.Game.Base.IConnection");
            IConnectionRegisterHandlerMethod = ResolveHelper.ResolveMethod(IConnectionType, "RegisterHandler");
            IConnectionUnregisterHandlerMethod = ResolveHelper.ResolveMethod(IConnectionType, "UnRegisterHandler");
            
            NetworkClientManagerType = BaseAssembly.MainModule.GetType("Nice.Game.Base.NetworkClientManager");
            NetworkClientManagerSendMethod = ResolveHelper.ResolveMethod(NetworkClientManagerType, "Send");
            
            NetworkServerManagerType = BaseAssembly.MainModule.GetType("Nice.Game.Base.NetworkServerManager");
            NetworkServerManagerSendMethod = ResolveHelper.ResolveMethod(NetworkServerManagerType, "Send");
            NetworkServerManagerBroadcastMethod = ResolveHelper.ResolveMethod(NetworkServerManagerType, "Broadcast");
        }
    }
}