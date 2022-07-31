using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Nuwa.Blob
{
    public static class Utility
    {
        public static string ToReadableFullName([NotNull] this Type type)
        {
            return type.IsGenericType ? Regex.Replace(type.ToString(), @"(\w+)`\d+\[(.*)\]", "$1<$2>") : type.ToString();
        }

        public static string ToReadableName([NotNull] this Type type)
        {
            if (!type.IsGenericType) return type.Name;
            var name = type.Name.Remove(type.Name.LastIndexOf('`'));
            name += "<";
            name += string.Join(",", type.GenericTypeArguments.Select(t => t.ToReadableName()));
            name += ">";
            return name;
        }
    }
}