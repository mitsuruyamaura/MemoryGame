using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// FirstOfDefault の拡張メソッド(拡張メソッドは static クラスに定義する必要がある)
/// </summary>
public static class EnumerableExtensions {
    public static bool TryGetFirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T result) where T : class {
        result = source.FirstOrDefault(predicate);
        return result != null;
    }
}