using HCI.EasyDapper.Values;
using PESALEXMapper.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace HCI.EasyDapper.Data
{
    public static class Mapper
    {
        private static string _systemNamespace => "System";
        private static string _collectionsNamespace => "System.Collections.Generic";
        private static ICollection<Type> _types;

        public static void AddIgnoreType(Type type)
        {
            _types = _types ?? new List<Type>();
            if (!_types.Contains(type))
                _types.Add(type);
        }
        private static PropertyInfo[] IgnoreChildren(Type source)
        {
            var defaultNamespace = _systemNamespace;
            string nameSpace = source.Namespace;
            string[] namespaceModel = source.Namespace.Equals(defaultNamespace)
               ? new string[] { _collectionsNamespace }
               : new string[] { nameSpace, _collectionsNamespace };
            var namespacePrefix = defaultNamespace != nameSpace.Split('.')[0] ? nameSpace.Split('.')[0] : string.Empty;
            var properties = source.GetProperties().Where(x => !namespaceModel.Contains(x.PropertyType.Namespace));
            if (!string.IsNullOrWhiteSpace(namespacePrefix))
                properties = properties.Where(x => !x.PropertyType.Namespace.Contains(namespacePrefix));
            return properties.ToArray();
        }

        internal static SQLRequest GetSQLRequest(object source)
        {
            var result = Activator.CreateInstance<SQLRequest>();
            try
            {
                var type = source.GetType();
                var specialProperties = new List<PropertyInfo>();
                var notMappedAttr = typeof(NotMappedAttribute).Name;
                var keyAttr = typeof(KeyAttribute).Name;
                if (_types != null)
                {
                    foreach (var item in _types)
                        if ((item.IsInterface && type.GetInterface(item.Name) != null) || type.IsSubclassOf(item))
                            specialProperties.AddRange(item.GetProperties()
                                .Where(p => p.CustomAttributes
                                    .Any(x =>
                                        x.AttributeType.Name.Equals(notMappedAttr)
                                        || x.AttributeType.Name.Equals(keyAttr))));
                }

                foreach (var property in IgnoreChildren(type))
                {
                    var prefix = string.Empty;
                    var propertyName = property.Name;
                    if (property.PropertyType.IsAutoClass)
                        continue;
                    foreach (var special in specialProperties)
                    {
                        if (special.Name.Equals(propertyName) && special.CustomAttributes.Any(c => c.AttributeType.Name.Equals(keyAttr)))
                            prefix = NamingPrefixValue.Key;
                        else if (special.Name.Equals(propertyName) && special.CustomAttributes.Any(c => c.AttributeType.Name.Equals(notMappedAttr)))
                            prefix = NamingPrefixValue.NotMapped;
                        if (!string.IsNullOrWhiteSpace(prefix))
                            break;
                    }
                    if (property.CustomAttributes != null)
                    {
                        foreach (var attr in property.CustomAttributes)
                        {
                            if (PropertyValidate(keyAttr, attr, propertyName, specialProperties.ToArray()))
                                prefix = NamingPrefixValue.Key;
                            else if (PropertyValidate(notMappedAttr, attr, propertyName, specialProperties.ToArray()))
                                prefix = NamingPrefixValue.NotMapped;
                            if (!string.IsNullOrWhiteSpace(prefix))
                                break;
                        }
                    }
                    if (prefix != NamingPrefixValue.NotMapped)
                    {
                        var value = property.GetValue(source);
                        result.Add($"{prefix}{propertyName}", value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return result;
        }

        private static bool PropertyValidate(string targetAttrName, CustomAttributeData attr, string propertyName, params PropertyInfo[] specialProperties)
        {
            return attr.AttributeType.Name == targetAttrName
                                            || (specialProperties != null && specialProperties.Any(x => x.Name.Equals(propertyName) && x.CustomAttributes.Any(c => c.AttributeType.Name.Equals(targetAttrName))));
        }

        internal static string GetTableName<T>()
        {
            var type = typeof(T);
            var tableAttr = typeof(TableAttribute).Name;
            var customAttr = type.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Name == tableAttr);
            if (customAttr != null)
            {
                return $"{customAttr.NamedArguments?[0].TypedValue.Value}].[{customAttr.ConstructorArguments?[0].Value}";
            }
            return type.Name;
        }

        internal static bool IsAutoIncrement<TEntity>(TEntity source)
            where TEntity : class
        {
            var result = false;
            var autoIncrementAttr = typeof(DatabaseGeneratedAttribute);

            foreach (var prop in IgnoreChildren(typeof(TEntity)))
            {
                var attr = prop.GetCustomAttribute(autoIncrementAttr);
                if (attr != null)
                {
                    result = ((DatabaseGeneratedAttribute)attr).DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
                    break;
                }
            }

            return result;
        }

        internal static void BindForeign<TEntity>(TEntity source)
            where TEntity : class
        {
            var foreignAttr = typeof(ForeignKeyAttribute);
            var keyAttr = typeof(KeyAttribute);
            foreach (var prop in typeof(TEntity).GetProperties())
            {
                var attr = prop.GetCustomAttribute(foreignAttr);
                if (attr == null) continue;
                var foreign = prop.GetValue(source);
                if (foreign != null)
                    foreach (var dependency in foreign.GetType().GetProperties())
                    {
                        if (dependency.GetCustomAttribute(keyAttr) != null)
                        {
                            var foreignKey = ((ForeignKeyAttribute)attr).Name;
                            MapperUtil.SetValue(source, foreignKey, dependency.GetValue(foreign));
                            break;
                        }
                    }
            }
        }

        internal static string GetKeyName<TEntity>(TEntity source) where TEntity : class
        {
            var keyAttr = typeof(KeyAttribute);
            ICollection<string> result = new List<string>();
            foreach (var prop in typeof(TEntity).GetProperties())
                if (prop.GetCustomAttribute(keyAttr) != null)
                    result.Add(prop.Name);
            return string.Join("-", result);
        }

        internal static void NavigationMapping<TEntity>(TEntity entity)
            where TEntity : class
        {
            var type = typeof(TEntity);
            var name = type.Name;
            foreach (var prop in type.GetProperties().Where(p => p.PropertyType.Namespace != _systemNamespace))
            {
                try
                {
                    if (ImplementationUtil.ContainsInterface(prop.PropertyType, nameof(IEnumerable)))
                    {
                        var array = prop.GetValue(entity);
                        if (array != null)
                        {
                            foreach (var obj in array as IEnumerable)
                            {
                                try
                                {
                                    if (obj != null && obj.GetType().GetProperty(name) != null)
                                        MapperUtil.SetValue(obj, name, entity);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                    }
                    else if (prop.PropertyType.IsClass)
                    {
                        var obj = prop.GetValue(entity);
                        if (obj != null && obj.GetType().GetProperty(name) != null)
                            MapperUtil.SetValue(obj, name, entity);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        internal static bool CheckIfIsNew<TEntity>(string propertyName, TEntity entity)
            where TEntity : class
        {
            var isNew = false;
            foreach (var keyName in propertyName.Split('-'))
            {
                var value = MapperUtil.GetValue(entity, keyName);
                if (string.IsNullOrEmpty(value.ToString()) || value.ToString() == "0")
                {
                    isNew = true;
                    break;
                }
            }

            return isNew;
        }

        internal static string GetSchema<T>() where T : class
        {
            var type = typeof(T);
            var tableAttr = typeof(TableAttribute).Name;
            var customAttr = type.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Name == tableAttr);
            if (customAttr != null)
                return $"[{customAttr.NamedArguments?[0].TypedValue.Value}].";
            return string.Empty;
        }
    }
}
