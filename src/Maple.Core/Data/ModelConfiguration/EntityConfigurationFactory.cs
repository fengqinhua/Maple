using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Data.ModelConfiguration
{
    public static class EntityConfigurationFactory
    {
        private const string Instance = "Instance";
        private static Dictionary<string, string> _providers = new Dictionary<string, string>();
        private static object _sync = new object();

        public static IEntityConfiguration GetEntityConfiguration(Type entityType)
        {
            string key = entityType.AssemblyQualifiedName;
            string assemblyQualifiedName = "";
            if (_providers.TryGetValue(key, out assemblyQualifiedName))
            {
                if (!string.IsNullOrWhiteSpace(assemblyQualifiedName))
                {
                    Type providerType = Type.GetType(assemblyQualifiedName);
                    if (null != providerType)
                    {
                        return (IEntityConfiguration)Activator.CreateInstance(providerType);
                    }
                    return null;
                }
            }

            return null;
        }

        public static void SetConfiguration(Type entityType,Type entityConfigurationType)
        {
            string key = entityType.AssemblyQualifiedName;
            string value = entityConfigurationType.AssemblyQualifiedName;

            object sync = _sync;
            lock (sync)
            {
                if (_providers.ContainsKey(key))
                    _providers[key] = value;
                else
                    _providers.Add(key, value);
            }
        }

    }
}
