using Maple.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System.Data.Common
{
    public static class DbProviderFactories
    {
        private const string Instance = "Instance";
        private static Dictionary<string, string> _providers = new Dictionary<string, string>();
        private static object _sync = new object();
        //private static ReaderWriterLockSlim _providerTableLock = new ReaderWriterLockSlim();
        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            // no need for locking on the table, as the row can only be from a DataTable that's a copy of the one contained in this class.
            string assemblyQualifiedName = "";
            if (_providers.TryGetValue(providerInvariantName, out assemblyQualifiedName))
            {
                if (!string.IsNullOrWhiteSpace(assemblyQualifiedName))
                {
                    Type providerType = Type.GetType(assemblyQualifiedName);
                    if (null != providerType)
                    {
                        System.Reflection.FieldInfo providerInstance = providerType.GetField(Instance, System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (null != providerInstance)
                        {
                            if (providerInstance.FieldType.IsSubclassOf(typeof(DbProviderFactory)))
                            {
                                object factory = providerInstance.GetValue(null);
                                if (null != factory)
                                {
                                    return (DbProviderFactory)factory;
                                }
                            }
                        }
                    }
                    throw new MapleException($"The registered .Net Framework Data Provider's DbProviderFactory implementation type '{assemblyQualifiedName}' couldn't be loaded.");
                }
            }
            throw new MapleException("The missing .Net Framework Data Provider's assembly qualified name is required.");
        }

        public static void SetFactory<TFactory>(string name = "")
            where TFactory : DbProviderFactory
        {
            RegisterFactoryInTable(name,  typeof(TFactory));
        }

        private static void RegisterFactoryInTable(string name, Type factoryType)
        {
            object sync = _sync;
            lock (sync)
            {
                string assemblyQualifiedNamea = factoryType.AssemblyQualifiedName;
                if (_providers.ContainsKey(name))
                    _providers[name] = assemblyQualifiedNamea;
                else
                    _providers.Add(name, assemblyQualifiedNamea);
            }

            //try
            //{
            //    _providerTableLock.EnterWriteLock();
            //    string assemblyQualifiedNamea = factoryType.AssemblyQualifiedName;

            //    if (_providers.ContainsKey(name))
            //        _providers[name] = assemblyQualifiedNamea;
            //    else
            //        _providers.Add(name, assemblyQualifiedNamea);

            //}
            //finally
            //{
            //    _providerTableLock.ExitWriteLock();
            //}
        }
    }
}
