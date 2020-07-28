using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class CompilationHook
    {
        [InitializeOnLoadMethod]
        private static void OnInitializeOnLoad()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
            OnPlayModeStateChanged(PlayModeStateChange.ExitingEditMode);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                CompilationHook.CheckWeaveAssemblies(WeaverProgram.Base);
                CompilationHook.CheckWeaveAssemblies(WeaverProgram.Client);
                CompilationHook.CheckWeaveAssemblies(WeaverProgram.Server);
            }
        }

        private static void CheckWeaveAssemblies(string module)
        {
            if (!SessionState.GetBool(module, false))
            {
                foreach (Assembly assembly in CompilationPipeline.GetAssemblies())
                {
                    if (File.Exists(assembly.outputPath) && assembly.outputPath.IndexOf(module) >= 0)
                    {
                        OnAssemblyCompilationFinished(assembly.outputPath, new CompilerMessage[0]);
                        break;
                    }
                }

                if (SessionState.GetBool(module, false))
                {
                    UnityEditorInternal.InternalEditorUtility.RequestScriptReload();
                }
            }
        }

        private static void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            if (EditorApplication.isPlaying || assemblyPath.IndexOf(".Editor") >= 0 || assemblyPath.IndexOf("-Editor") >= 0)
            {
                return;
            }

            string networkingRuntimeDLL = Helpers.FindNetworkingRuntime();
            string unityEngineCoreModuleRuntimeDLL = UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath();
            string baseModuleRuntimeDLL = Helpers.FindBaseRuntime();
            HashSet<string> dependencyPaths = Helpers.GetDependecyPaths(assemblyPath);

            bool result = WeaverProgram.WeaveAssemblies(unityEngineCoreModuleRuntimeDLL, networkingRuntimeDLL, baseModuleRuntimeDLL, assemblyPath, dependencyPaths.ToArray());
            string module = Path.GetFileName(assemblyPath);
            SessionState.SetBool(module, result);
        }
    }
}