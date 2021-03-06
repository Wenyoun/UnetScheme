﻿using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using System.Collections.Generic;
using UnityEngine;

namespace Zyq.Weaver
{
    public static class CompilationHook
    {
        [InitializeOnLoadMethod]
        private static void OnInitializeOnLoad()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
            //OnPlayModeStateChanged(PlayModeStateChange.ExitingEditMode);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                CheckWeaveAssemblies(WeaverProgram.Client);
                CheckWeaveAssemblies(WeaverProgram.Server);
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
            if (EditorApplication.isPlaying || assemblyPath.IndexOf(".Editor") >= 0 || assemblyPath.IndexOf("-Editor") >= 0 || assemblyPath.IndexOf("Assembly-CSharp.dll") >= 0)
            {
                return;
            }

            for (int i = 0; i < messages.Length; ++i)
            {
                if (messages[i].type == CompilerMessageType.Error)
                {
                    return;
                }
            }
            
            if (assemblyPath.IndexOf(WeaverProgram.Client) == -1 && assemblyPath.IndexOf(WeaverProgram.Server) == -1)
            {
                return;
            }

            Debug.Log(assemblyPath);

            string unityEngineCoreModuleRuntimeDLL = UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath();
            string baseModuleRuntimeDLL = Helpers.FindBaseRuntime();
            HashSet<string> dependencyPaths = Helpers.GetDependecyPaths(assemblyPath);

            bool result = WeaverProgram.WeaveAssemblies(unityEngineCoreModuleRuntimeDLL, baseModuleRuntimeDLL, assemblyPath, dependencyPaths.ToArray());
            string module = Path.GetFileName(assemblyPath);
            SessionState.SetBool(module, result);

            UnityEditorInternal.InternalEditorUtility.RequestScriptReload();
        }
    }
}