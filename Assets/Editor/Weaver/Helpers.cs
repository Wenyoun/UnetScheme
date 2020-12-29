using System.IO;
using UnityEditor.Compilation;
using System.Collections.Generic;

namespace Zyq.Weaver
{
    public static class Helpers
    {
        //获取输入的程序集依赖的其他程序集路径
        public static HashSet<string> GetDependecyPaths(string assemblyPath)
        {
            HashSet<string> dependencyPaths = new HashSet<string>();
            dependencyPaths.Add(Path.GetDirectoryName(assemblyPath));
            foreach (Assembly unityAsm in CompilationPipeline.GetAssemblies())
            {
                if (unityAsm.outputPath != assemblyPath)
                {
                    continue;
                }
                foreach (string unityAsmRef in unityAsm.compiledAssemblyReferences)
                {
                    dependencyPaths.Add(Path.GetDirectoryName(unityAsmRef));
                }
            }
            return dependencyPaths;
        }

        //找到Base模块目录
        public static string FindBaseRuntime()
        {
            foreach (Assembly unityAsm in CompilationPipeline.GetAssemblies())
            {
                if (unityAsm.outputPath.EndsWith("Nice.Game.Base.dll"))
                {
                    return unityAsm.outputPath;
                }
            }
            throw new System.Exception("Not found Nice.Game.Base.dll");
        }

        public static string FindUnityEngineDLLDirectoryName()
        {
            string directoryName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            return directoryName?.Replace(@"file:\", "");
        }
    }
}