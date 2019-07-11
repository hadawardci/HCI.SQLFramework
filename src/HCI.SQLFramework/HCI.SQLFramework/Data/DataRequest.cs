using HCI.SQLFramework.Values;
using System.Collections.Generic;
using System.Linq;

namespace HCI.SQLFramework.Data
{
    internal class SQLRequest : Dictionary<string, object>
    {
        /// <summary>
        /// Cláusula de valores por parâmetro
        /// </summary>
        public string ValuesClause => $"@{string.Join(", @", this.Keys.Where(x => !x.StartsWith(NamingPrefixValue.Key)))}";

        /// <summary>
        /// Cláusula de valores por parâmetro com chave primária
        /// </summary>
        public string ValuesClauseWithKeys => $"@{string.Join(", @", this.Keys.Select(x => NamingPrefixValue.WithoutKey(x)))}";

        /// <summary>
        /// Cláusula de nomes dos parâmetros
        /// </summary>
        public string ParametersNames => $"[{string.Join("], [", this.Keys.Where(x => !x.StartsWith(NamingPrefixValue.Key)))}]";

        /// <summary>
        /// Cláusula de nomes dos parâmetros com chave primária
        /// </summary>
        public string ParametersNamesWithKeys => $"[{string.Join("], [", this.Keys.Select(x => NamingPrefixValue.WithoutKey(x)))}]";
        /// <summary>
        /// Cláusula de atribuições por parâmetro
        /// </summary>
        public string SetClause
        {
            get
            {
                var result = new List<string>();
                foreach (var item in this.Where(x => !x.Key.StartsWith(NamingPrefixValue.Key)))
                    result.Add($"[{ item.Key }] = @{ item.Key }");
                return string.Join(", ", result);
            }
        }
        /// <summary>
        /// Cláusula de condição da alteração por parâmetro
        /// </summary>
        public string WhereClause
        {
            get
            {
                var result = new List<string>();
                foreach (var item in this.Where(x => x.Key.StartsWith(NamingPrefixValue.Key)))
                    result.Add($"[{ NamingPrefixValue.WithoutKey(item.Key) }] = @{NamingPrefixValue.WithoutKey(item.Key) }");
                if (!result.Any())
                    return SetClause;
                return result.Any() ? $"WHERE {string.Join(" AND ", result)}" : string.Empty;
            }
        }
        /// <summary>
        /// Obter objeto sem chaves primárias
        /// </summary>
        /// <returns></returns>
        public object ParametersWithoutKeys()
        {
            return this.Where(x => !x.Key.StartsWith(NamingPrefixValue.Key));
        }
        /// <summary>
        /// Obter objeto com chaves primárias
        /// </summary>
        /// <returns></returns>
        public object ParametersWithKeys()
        {
            var result = new Dictionary<string, object>();
            foreach (var item in this.ToList())
                result.Add(NamingPrefixValue.WithoutKey(item.Key), item.Value);
            return result;
        }

        /// <summary>
        /// Obter objeto de chaves primárias
        /// </summary>
        /// <returns></returns>
        public object ParametersKeys()
        {
            var result = new Dictionary<string, object>();
            foreach (var item in this.Where(x => x.Key.StartsWith(NamingPrefixValue.Key)))
                result.Add(NamingPrefixValue.WithoutKey(item.Key), item.Value);
            return result;
        }

    }
}
