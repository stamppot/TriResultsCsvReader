using System;
using Optional;

namespace UrlResultsFetcher
{
    public static class OptionalExtensions
    {
        public static void IfPresent<T>(this Option<T> option, Action<T> action)
        {
            if (option.HasValue)
            {
                action.Invoke(option.ValueOr(default(T)));
            }
        }

        public static U IfPresentWithDefault<T,U>(this Option<T> option, Func<T, U> func, U defaultValue)
        {
            return option.HasValue ? func.Invoke(option.ValueOr(default(T))) : defaultValue;
        }
    }
}
