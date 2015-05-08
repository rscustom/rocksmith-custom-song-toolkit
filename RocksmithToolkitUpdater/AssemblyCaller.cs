using System;
using System.Linq;
using System.Reflection;

namespace RocksmithToolkitUpdater
{
    public class AssemblyCaller : MarshalByRefObject
    {
        private const string UPDATER_DOMAIN = "UpdaterDomain";

        public static object CallStatic(string assemblyPath, string typeName, string method, params object[] methodParams) {
            return Call(assemblyPath, typeName, method, null, true, methodParams);
        }

        public static object Call(string assemblyPath, string typeName, string method, Type[] paramTypes, params object[] methodParams) {
            return Call(assemblyPath, typeName, method, paramTypes, false, methodParams);
        }

        // EXECUTE METHOD IN LIBS WITHOUT LOCK FILE
        private static object Call(string assemblyPath, string typeName, string method, Type[] paramTypes, bool createInstance, params object[] methodParams) {
            AppDomain domain = AppDomain.CreateDomain(UPDATER_DOMAIN);
            AssemblyCaller assemblyCaller = new AssemblyCaller();
            assemblyCaller = (AssemblyCaller)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(AssemblyCaller).FullName);
            object result = assemblyCaller.PrivateCall(assemblyPath, typeName, method, paramTypes, createInstance, methodParams);
            AppDomain.Unload(domain);
            return result;
        }

        private object PrivateCall(string assemblyPath, string typeName, string method, Type[] paramTypes, bool createInstance, params object[] methodParams)
        {
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            Type compiledType = assembly.GetType(typeName);
            var istance = Activator.CreateInstance(compiledType);
            var bindingFlags = createInstance ? (BindingFlags.Public | BindingFlags.Instance) : (BindingFlags.Public | BindingFlags.Static);
            var methodInfo = (paramTypes == null) ? compiledType.GetMethod(method, bindingFlags) : compiledType.GetMethod(method, paramTypes);
            return methodInfo.Invoke(istance, methodParams ?? new object[] {
                new object()
            });
        }
    }
}