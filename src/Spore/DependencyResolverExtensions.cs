using System;
using System.Collections.Generic;
using Splat;

namespace Spore;

public static class DependencyResolverExtensions
{
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver resolver, string? contract = default)
    {
        return (T)resolver.GetRequiredService(typeof(T), contract);
    }

    public static object GetRequiredService(this IReadonlyDependencyResolver resolver, Type type, string? contract = default)
    {
        var service = resolver.GetService(type, contract);

        if (service is not null) return service;

        var message = $"Failed to resolve {type.Name}";

        if (!string.IsNullOrWhiteSpace(contract))
            message = $"{message} with contract {contract}";

        throw new InvalidOperationException(message);
    }

    public static IEnumerable<T> ResolveIEnumerable<T>(this IEnumerable<T> target)
    {
        // https://github.com/reactivemarbles/Splat.DI.SourceGenerator/issues/72
        if (target is not null)
            throw new InvalidOperationException("This issue has been resolved. Remove this method.");

        return Locator.Current.GetServices<T>();
    }

    public static Lazy<IEnumerable<T>> ResolveIEnumerable<T>(this Lazy<IEnumerable<T>>? target)
    {
        // https://github.com/reactivemarbles/Splat.DI.SourceGenerator/issues/72
        if (target is not null)
            throw new InvalidOperationException("This issue has been resolved. Remove this method.");

        return new Lazy<IEnumerable<T>>(() => Locator.Current.GetServices<T>());
    }
}
