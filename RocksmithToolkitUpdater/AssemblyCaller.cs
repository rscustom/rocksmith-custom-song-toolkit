using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;

namespace RocksmithToolkitUpdater
{
    public class AssemblyCaller : MarshalByRefObject
    {
        private const string UPDATER_DOMAIN = "UpdaterDomain";

        public static object CallStatic(string assemblyPath, string typeName, string method, params object[] methodParams)
        {
            return Call(assemblyPath, typeName, method, null, true, methodParams);
        }

        public static object Call(string assemblyPath, string typeName, string method, Type[] paramTypes, params object[] methodParams)
        {
            return Call(assemblyPath, typeName, method, paramTypes, false, methodParams);
        }

        // EXECUTE METHOD IN LIBS WITHOUT LOCK FILE
        private static object Call(string assemblyPath, string typeName, string method, Type[] paramTypes, bool createInstance, params object[] methodParams)
        {
            var assemblyName = Assembly.GetExecutingAssembly().FullName;
            AppDomain domain = AppDomain.CreateDomain(UPDATER_DOMAIN);
            AssemblyCaller assemblyCaller = new AssemblyCaller();
            assemblyCaller = (AssemblyCaller)domain.CreateInstanceAndUnwrap(assemblyName, typeof(AssemblyCaller).FullName);
            object result = assemblyCaller.PrivateCall(assemblyPath, typeName, method, paramTypes, createInstance, methodParams);
            AppDomain.Unload(domain);
            return result;
        }

        private object PrivateCall(string assemblyPath, string typeName, string method, Type[] paramTypes, bool createInstance, params object[] methodParams)
        {
            if (!File.Exists(assemblyPath))
            {
                // try to find RocksmithToolkitGUI.exe using enumeration
                var toolkitPath = Directory.EnumerateFiles(Path.GetPathRoot(assemblyPath), "RocksmithToolkitGUI.exe", SearchOption.AllDirectories).FirstOrDefault();
                if (String.IsNullOrEmpty(assemblyPath))
                {
                    MessageBox.Show("<ERROR> PrivateCall file does not exist ..." + Environment.NewLine +
                                    "assemblyPath = " + assemblyPath, "DPDM");
                    return null;
                }
                else
                    assemblyPath = toolkitPath;
            }

            // a f'n nightmare to debug this shit ... here's another bad boy
            // Assembly assembly = Assembly.LoadFile(assemblyPath);
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Type compiledType = assembly.GetType(typeName);
            try
            {
                var instance = Activator.CreateInstance(compiledType);
                var bindingFlags = createInstance ? (BindingFlags.Public | BindingFlags.Instance) : (BindingFlags.Public | BindingFlags.Static);
                var methodInfo = (paramTypes == null) ? compiledType.GetMethod(method, bindingFlags) : compiledType.GetMethod(method, paramTypes);
                var ret = methodInfo.Invoke(instance, methodParams);
                return ret;
            }
            catch (Exception ex)
            {
                MessageBox.Show("<ERROR> PrivateCall instance failed ..." + Environment.NewLine +
                                "assemblyPath = " + assemblyPath + Environment.NewLine +
                                 ex.InnerException.Message, "DPDM");
            }

            return null;
        }

    }
}