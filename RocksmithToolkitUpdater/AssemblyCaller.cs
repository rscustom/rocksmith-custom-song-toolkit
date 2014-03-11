using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RocksmithToolkitUpdater
{
    public class AssemblyCaller : MarshalByRefObject
    {
        private const string UPDATER_DOMAIN = "UpdaterDomain";

        public static object Call(string assemblyPath, string typeName, string method, params object[] methodParams) {
            AppDomain domain = AppDomain.CreateDomain(UPDATER_DOMAIN);
            AssemblyCaller assemblyCaller = (AssemblyCaller)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(AssemblyCaller).FullName);
            object result = assemblyCaller.PrivateCall(assemblyPath, typeName, method, methodParams);
            AppDomain.Unload(domain);
            return result;
        }

        private object PrivateCall(string assemblyPath, string typeName, string method, params object[] methodParams) {
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            Type compiledType = assembly.GetType(typeName);
            var istance = Activator.CreateInstance(compiledType);
            return compiledType.GetMethod(method).Invoke(istance, methodParams == null ? new object[] { new object() } : methodParams);
        }
    }
}