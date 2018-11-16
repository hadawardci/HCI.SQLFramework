using HCI.SQLFramework.Values;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace HCI.SQLFramework.Data
{
    public static class Mapper
    {
        private static ICollection<Type> _types;

        public static void AddIgnoreType(Type type)
        {
            _types = _types ?? new List<Type>();
            if (!_types.Contains(type))
                _types.Add(type);
        }

        private static PropertyInfo[] IgnoreChildren(Type source)
        {
            var defaultNamespace = "System";
            string nameSpace = source.Namespace;
            string[] namespaceModel = source.Namespace.Equals(defaultNamespace)
               ? new string[] { "System.Collections.Generic" }
               : new string[] { nameSpace, "System.Collections.Generic" };
            var namespacePrefix = defaultNamespace != nameSpace.Split('.')[0] ? nameSpace.Split('.')[0] : string.Empty;
            var properties = source.GetProperties().Where(x => !namespaceModel.Contains(x.PropertyType.Namespace));
            if (!string.IsNullOrWhiteSpace(namespacePrefix))
                properties = properties.Where(x => !x.PropertyType.Namespace.Contains(namespacePrefix));
            return properties.ToArray();
        }

        public static SQLRequest GetSQLRequest(object source)
        {
            var result = Activator.CreateInstance<SQLRequest>();
            try
            {
                var type = source.GetType();
                var specialProperties = new List<PropertyInfo>();
                var notMappedAttributeName = typeof(NotMappedAttribute).Name;
                var keyAttributeName = typeof(KeyAttribute).Name;
                if (_types != null)
                {
                    foreach (var item in _types)
                        if ((item.IsInterface && type.GetInterface(item.Name) != null) || type.IsSubclassOf(item))
                            specialProperties.AddRange(item.GetProperties()
                                .Where(p => p.CustomAttributes
                                    .Any(x =>
                                        x.AttributeType.Name.Equals(notMappedAttributeName)
                                        || x.AttributeType.Name.Equals(keyAttributeName))));
                }

                foreach (var property in IgnoreChildren(type))
                {
                    var prefix = string.Empty;
                    var key = property.Name;
                    if (property.PropertyType.IsAutoClass)
                        continue;
                    foreach (var special in specialProperties)
                    {
                        if (special.Name.Equals(key) && special.CustomAttributes.Any(c => c.AttributeType.Name.Equals(keyAttributeName)))
                            prefix = NamingPrefixValue.Key;
                        else if (special.Name.Equals(key) && special.CustomAttributes.Any(c => c.AttributeType.Name.Equals(notMappedAttributeName)))
                            prefix = NamingPrefixValue.NotMapped;
                        if (!string.IsNullOrWhiteSpace(prefix))
                            break;
                    }
                    if (property.CustomAttributes != null)
                    {
                        foreach (var attr in property.CustomAttributes)
                        {
                            if (attr.AttributeType.Name == keyAttributeName
                                || specialProperties.Any(x => x.Name.Equals(key) && x.CustomAttributes.Any(c => c.AttributeType.Name.Equals(keyAttributeName))))
                                prefix = NamingPrefixValue.Key;
                            else if (attr.AttributeType.Name == notMappedAttributeName
                                || specialProperties.Any(x => x.Name.Equals(key) && x.CustomAttributes.Any(c => c.AttributeType.Name.Equals(notMappedAttributeName))))
                                prefix = NamingPrefixValue.NotMapped;
                            if (!string.IsNullOrWhiteSpace(prefix))
                                break;
                        }
                    }
                    if (prefix != NamingPrefixValue.NotMapped)
                    {
                        var value = property.GetValue(source);
                        result.Add($"{prefix}{key}", value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return result;
        }
    }
}
