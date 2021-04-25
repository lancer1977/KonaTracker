using System; 
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PolyhydraGames.Extensions;

namespace PolyhydraWebsite.Bootstrapper
{
    public static class IOCHelper
    {

        public static void RegisterScopedService<T, T2, T3>(this IServiceCollection collection) where T : class, T2, T3
    where T2 : class
    where T3 : class
        {
            collection.AddScoped<T>();

            collection.AddScoped<T2>(x => x.GetRequiredService<T>());
            collection.AddScoped<T3>(x => x.GetRequiredService<T>());

        }

        public static void RegisterService<T, T2, T3>(this IServiceCollection collection) where T : class, T2, T3
            where T2 : class
            where T3 : class
        {
            collection.AddSingleton<T>();
            collection.AddSingleton<T2>(x => x.GetRequiredService<T>());
            collection.AddSingleton<T3>(x => x.GetRequiredService<T>());

        }

        public static void RegisterTypesEndingWith(this IServiceCollection collection, string name, params Assembly[] assembly)
        {
            var types = assembly.GetTypesEndingWith(name);
            foreach (var item in types)
            {
                collection.AddSingleton(item); 
            }
        }

        public static void RegisterTypesAsInterfacesEndingWith(this IServiceCollection collection, string name, params Assembly[] assembly)
        {
            var types = assembly.GetTypesEndingWith(name);
            foreach (var item in types)
            {
                try
                {

                    collection.AddScoped(item);
                    var interfaces = item.GetInterfaces();
                    foreach (var @interface in interfaces)
                    {
                        collection.AddScoped(@interface, x => x.GetRequiredService(item));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        public static void RegisterTypesAsScopedAndInterfaces<T>(this IServiceCollection collection)
        {

            try
            {
                var type = typeof(T);
                collection.AddScoped(type);
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    collection.AddScoped(@interface, x => x.GetRequiredService(type));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
 
    }
}
