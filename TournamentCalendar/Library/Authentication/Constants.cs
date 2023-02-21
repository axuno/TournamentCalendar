using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TournamentCalendar.Library.Authentication
{
    public class Constants
    {
        /// <summary>
        /// Constants which are used as authentication roles.
        /// </summary>
        public class RoleName
        {
            public const string Editor = nameof(Editor);
            public const string Admin = nameof(Admin);

            /// <summary>
            /// Get the values of all constants in this class.
            /// </summary>
            /// <returns>Returns the values of all constants in this class.</returns>
            public static IEnumerable<T?> GetAllValues<T>()
            {
                return Constants.GetAllValues<T>(typeof(RoleName));
            }

            /// <summary>
            /// Get the names of all constants in this class.
            /// </summary>
            /// <returns>Returns the names of all constants in this class.</returns>
            public static IEnumerable<string> GetAllNames()
            {
                return Constants.GetAllNames(typeof(RoleName));
            }
        }

        /// <summary>
        /// Get the values of type T of all constants in this class.
        /// </summary>
        /// <returns>Returns the values of type T of all constants in this class.</returns>
        private static IEnumerable<T?> GetAllValues<T>(IReflect type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T?)x.GetRawConstantValue())
                .ToList();
        }

        /// <summary>
        /// Get the names of all constants in this class.
        /// </summary>
        /// <returns>Returns the names of all constants in this class.</returns>
        private static IEnumerable<string> GetAllNames(IReflect type)
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public;

            var fields = type.GetFields(flags).Where(f => f.IsLiteral);
            foreach (var fieldInfo in fields)
            {
                var val = fieldInfo.GetValue(null)?.ToString();
                if (val != null) yield return val;
            }
        }
    }
}
