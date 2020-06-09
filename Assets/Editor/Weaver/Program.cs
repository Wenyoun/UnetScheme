namespace Zyq.Weaver {
    public static class Program {
        public static bool Process(string unityEngineDLL, string networkingDLL, string baseModuleRuntimeDLL, string assemblyPath, string[] depAssemblyPaths) {
            return WeaverProgram.WeaveAssemblies(unityEngineDLL, networkingDLL, baseModuleRuntimeDLL, assemblyPath, depAssemblyPaths);
        }
    }
}