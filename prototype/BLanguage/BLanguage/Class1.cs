using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLanguage
{
    static class Class1
    {
        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue addValue, Func<TValue, TValue> updateValueFactory)
=> dict[key] = dict.TryGetValue(key, out TValue? result) ? updateValueFactory(result) : addValue;

    }
}
