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
        public static AssemblyDefinition UnityAssembly;
        public static AssemblyDefinition BaseAssembly;
        public static AssemblyDefinition NetworkingAssembly;
        public static AssemblyDefinition CurrentAssembly;

        #region  networking
        public static TypeReference NetowrkMessageType;
        public static FieldReference NetworkMessageReaderField;
        public static MethodReference NetowrkMessageCtorMethod;
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

        #region Zyq.Game.Base.IEntity
        public static TypeReference IEntityType;
        public static MethodReference IEntityGetFetureMethod;
        #endregion

        #region Zyq.Game.Base.AbsCop
        public static TypeReference AbsCopType;
        public static MethodReference AbsCopOnRemoveMethod;
        public static MethodReference AbsCopGetEntityMethod;
        #endregion

        #region Zyq.Game.Base.Connection
        public static TypeReference ConnectionType;
        public static MethodReference ConnectionSendMethod;
        public static MethodReference ConnectionRegisterHandlerMethod;
        public static MethodReference ConnectionUnregisterHandlerMethod;
        #endregion

        #region Zyq.Game.Base.ConnectionFeture
        public static TypeReference ConnectionFetureType;
        public static MethodReference ConnectionFetureRegisterHandlerMethod;
        public static MethodReference ConnectionFetureUnregisterHandlerMethod;
        #endregion

        #region Zyq.Game.Client.Client
        public static TypeReference ClientType;
        public static FieldReference ClientInsField;
        public static MethodReference ClientSendMethod;
        #endregion

        #region Zyq.Game.Server.Server
        public static TypeReference ServerType;
        public static FieldReference ServerInsField;
        public static MethodReference ServerSendMethod;
        public static MethodReference ServerBroadcastMethod;
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

                SetupBaseModuleTypes();

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
                SetupClientModuleTypes();
                return ClientWeaver.Weave(module);
            }
            else if (module.Name.EndsWith("Zyq.Game.Server.dll"))
            {
                SetupServerModuleTypes();
                return ServerWeaver.Weave(module);
            }
            return false;
        }

        private static void SetupUnityTypes()
        {
            NetowrkMessageType = NetworkingAssembly.MainModule.GetType("UnityEngine.Networking.NetworkMessage");
            NetworkMessageReaderField = ResolveHelper.ResolveField(NetowrkMessageType, "reader");

            NetworkWriterType = NetworkingAssembly.MainModule.GetType("UnityEngine.Networking.NetworkWriter");

            NetworkWriterCtorMethod = NetworkWriterType.Resolve().Methods.Single(m => m.FullName.IndexOf(".ctor()") >= 0);
            NetworkWriterStartMessageMethod = ResolveHelper.ResolveMethod(NetworkWriterType, "StartMessage");
            NetworkWriterFinishMessageMethod = ResolveHelper.ResolveMethod(NetworkWriterType, "FinishMessage");

            NetworkReaderType = NetworkingAssembly.MainModule.GetType("UnityEngine.Networking.NetworkReader");
        }

        private static void SetupBaseModuleTypes()
        {
            SendType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.SendAttribute");
            RecvType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.RecvAttribute");
            ProtocolType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.ProtocolAttribute");
            BroadcastType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.BroadcastAttribute");
            SyncClassType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.SyncClassAttribute");
            SyncFieldType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.SyncFieldAttribute");

            IEntityType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.IEntity");
            IEntityGetFetureMethod = ResolveHelper.ResolveMethod(IEntityType, "GetFeture");

            AbsCopType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.AbsCop");
            AbsCopOnRemoveMethod = ResolveHelper.ResolveMethod(AbsCopType, "OnRemove");
            AbsCopGetEntityMethod = ResolveHelper.ResolveMethod(AbsCopType, "get_Entity");

            ConnectionType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.Connection");
            ConnectionSendMethod = ResolveHelper.ResolveMethod(ConnectionType, "Send");
            ConnectionRegisterHandlerMethod = ResolveHelper.ResolveMethod(ConnectionType, "RegisterHandler");
            ConnectionUnregisterHandlerMethod = ResolveHelper.ResolveMethod(ConnectionType, "UnregisterHandler");

            ConnectionFetureType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.ConnectionFeture");
            ConnectionFetureRegisterHandlerMethod = ResolveHelper.ResolveMethod(ConnectionFetureType, "RegisterHandler");
            ConnectionFetureUnregisterHandlerMethod = ResolveHelper.ResolveMethod(ConnectionFetureType, "UnregisterHandler");
        }

        private static void SetupClientModuleTypes()
        {
            ClientType = CurrentAssembly.MainModule.GetType("Zyq.Game.Client.Client");
            ClientInsField = ResolveHelper.ResolveField(ClientType, "Ins");
            ClientSendMethod = ResolveHelper.ResolveMethod(ClientType, "Send");
        }

        private static void SetupServerModuleTypes()
        {
            ServerType = CurrentAssembly.MainModule.GetType("Zyq.Game.Server.Server");
            ServerInsField = ResolveHelper.ResolveField(ServerType, "Ins");
            ServerSendMethod = ResolveHelper.ResolveMethod(ServerType, "Send");
            ServerBroadcastMethod = ResolveHelper.ResolveMethod(ServerType, "Broadcast");
        }
    }
}