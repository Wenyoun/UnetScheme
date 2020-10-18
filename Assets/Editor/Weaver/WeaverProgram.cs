using System;
using System.IO;
using Mono.CecilX;
using UnityEngine;

namespace Zyq.Weaver
{
    public static class WeaverProgram
    {
        #region User Module
        public const string Base = "Zyq.Game.Base.dll";
        public const string Server = "Zyq.Game.Server.dll";
        public const string Client = "Zyq.Game.Client.dll";
        #endregion

        #region AssemblyDefinition
        public static AssemblyDefinition UnityAssembly;
        public static AssemblyDefinition BaseAssembly;
        public static AssemblyDefinition NetworkingAssembly;
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
                    if (NetworkingAssembly != null)
                    {
                        NetworkingAssembly.Dispose();
                        NetworkingAssembly = null;
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
            using (CurrentAssembly = AssemblyDefinition.ReadAssembly(assemblyPath,
                                                                     new ReaderParameters { ReadWrite = true, ReadSymbols = true, AssemblyResolver = asmResolver }))
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
                        CurrentAssembly.Write(new WriterParameters { WriteSymbols = true });
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
            if (module.Name.EndsWith(Base))
            {
                BaseAssembly = CurrentAssembly;
                SetupBaseModuleTypes();
                return BaseWeaver.Weave(module);
            }
            else if (module.Name.EndsWith(Client))
            {
                BaseAssembly = AssemblyDefinition.ReadAssembly(baseModuleRuntimeDLL);
                SetupBaseModuleTypes();
                SetupClientModuleTypes();
                return ClientWeaver.Weave(module);
            }
            else if (module.Name.EndsWith(Server))
            {
                BaseAssembly = AssemblyDefinition.ReadAssembly(baseModuleRuntimeDLL);
                SetupBaseModuleTypes();
                SetupServerModuleTypes();
                return ServerWeaver.Weave(module);
            }
            return false;
        }
        
        private static void SetupBaseModuleTypes()
        {
            ChannelMessageType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.ChannelMessage");
            ChannelMessageBufferField = ResolveHelper.ResolveField(ChannelMessageType, "Buffer");
            ByteBufferType = BaseAssembly.MainModule.GetType("Zyq.Game.Base.ByteBuffer");
            ByteBufferAllocateMethod = ResolveHelper.ResolveMethod(ByteBufferType, "Allocate");
                
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