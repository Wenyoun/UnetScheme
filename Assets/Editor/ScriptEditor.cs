using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.CecilX;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;

public class ScriptEditor : MonoBehaviour {
    private static string AssemblyPath = "Library/ScriptAssemblies/Assembly-CSharp.dll";

    [MenuItem("Gen/Script")]
    private static void OnGenerator() {
        using(DefaultAssemblyResolver resolver = new DefaultAssemblyResolver())
        using(AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(AssemblyPath, new ReaderParameters() { ReadSymbols = true, ReadWrite = true, AssemblyResolver = resolver })) {
            resolver.AddSearchDirectory(Path.GetDirectoryName(AssemblyPath));
            resolver.AddSearchDirectory(Path.GetDirectoryName(UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath()));

            HashSet<string> deps = GetDependecyPaths(AssemblyPath);
            foreach (string dep in deps) {
                resolver.AddSearchDirectory(dep);
            }

            ModuleDefinition module = assembly.MainModule;
        }
    }

    private static HashSet<string> GetDependecyPaths(string assemblyPath) {
        HashSet<string> dependencyPaths = new HashSet<string>();
        dependencyPaths.Add(Path.GetDirectoryName(assemblyPath));
        foreach (Assembly unityAsm in CompilationPipeline.GetAssemblies()) {
            if (unityAsm.outputPath != assemblyPath) {
                continue;
            }
            foreach (string unityAsmRef in unityAsm.compiledAssemblyReferences) {
                dependencyPaths.Add(Path.GetDirectoryName(unityAsmRef));
            }
        }
        return dependencyPaths;
    }
}