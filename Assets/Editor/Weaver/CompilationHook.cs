using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;

namespace Zyq.Weaver
{
    public class CompilationHook
    {
        [InitializeOnLoadMethod]
        private static void OnInitializeOnLoad()
        {
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
            if (!SessionState.GetBool("MIRROR_WEAVED", false))
            {
                SessionState.SetBool("MIRROR_WEAVED", true);
                SessionState.SetBool("MIRROR_WEAVE_SUCCESS", true);
                WeaveExistingAssemblies();
            }
        }

        [MenuItem("Test/Execute")]
        public static void WeaveExistingAssemblies()
        {
            foreach (Assembly assembly in CompilationPipeline.GetAssemblies())
            {
                if (File.Exists(assembly.outputPath))
                {
                    OnAssemblyCompilationFinished(assembly.outputPath, new CompilerMessage[0]);
                }
            }
            UnityEditorInternal.InternalEditorUtility.RequestScriptReload();
        }

        private static void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            if (EditorApplication.isPlaying ||
                assemblyPath.IndexOf(".Editor") >= 0 ||
                assemblyPath.IndexOf("-Editor") >= 0 ||
                assemblyPath.IndexOf("Zyq.Game.Base") >= 0)
            {
                return;
            }

            if (assemblyPath.IndexOf("Zyq.Game") == -1)
            {
                return;
            }

            Debug.Log(assemblyPath);

            string networkingRuntimeDLL = Helpers.FindNetworkingRuntime();

            string unityEngineCoreModuleRuntimeDLL = UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath();

            string baseModuleRuntimeDLL = Helpers.FindBaseRuntime();

            HashSet<string> dependencyPaths = Helpers.GetDependecyPaths(assemblyPath);

            if (!Program.Process(unityEngineCoreModuleRuntimeDLL, networkingRuntimeDLL, baseModuleRuntimeDLL, assemblyPath, dependencyPaths.ToArray()))
            {
                SessionState.SetBool("MIRROR_WEAVE_SUCCESS", false);
            }
        }
    }
}