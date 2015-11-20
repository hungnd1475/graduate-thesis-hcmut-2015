using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;
using IronPython.Compiler;
using IronPython.Runtime;
using System.Text;
using System.IO;

namespace emr_corefsol_service.Utilities
{
    public class PythonExcuter
    {
        private ScriptEngine pyEngine = null;
        private ScriptScope scope = null;
        private PythonCompilerOptions pco = null;
        private ScriptSource script = null;
        public PythonExcuter(string python_lib_path, string pyFile, string[] options)
        {
            pyEngine = Python.CreateEngine();
            pyEngine.GetSysModule().SetVariable("argv", options);
            scope = pyEngine.Runtime.CreateScope();

            script = pyEngine.CreateScriptSourceFromFile(pyFile, Encoding.UTF8, SourceCodeKind.File);

            pco = (PythonCompilerOptions)pyEngine.GetCompilerOptions(scope);
            pco.ModuleName = "__main__";
            pco.Module |= ModuleOptions.Initialize;

            var paths = pyEngine.GetSearchPaths();
            paths.Add(python_lib_path);
            var file_directory = new FileInfo(pyFile).Directory.FullName;
            paths.Add(file_directory);
            pyEngine.SetSearchPaths(paths);
        }
        
        public void Excute()
        {
            script.Compile(pco).Execute(scope);
        }

        public dynamic GetVariable(string name)
        {
            return scope.GetVariable(name);
        }
    }
}