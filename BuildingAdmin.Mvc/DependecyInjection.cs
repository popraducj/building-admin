using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using BuildingAdmin.DataLayer.Repository;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc
{
    public static class DependecyInjection {
        
        public static IServiceCollection AddDependencies(this IServiceCollection services){

            services.AddSingleton<MongoDbGenericRepository.IBaseMongoRepository, WebsiteRepository>();
            GetTypes("BuildingAdmin.Services.Implementations", "BuildingAdmin.Services").ForEach(p =>
            {
                services.AddTransient(p.GetInterfaces().Last().UnderlyingSystemType, p.UnderlyingSystemType);
            });
            return services;
        }

        private static List<Type> GetTypes(string path, string assemblyName = "")
        {
           var assembly = string.IsNullOrEmpty(assemblyName) ? Assembly.GetExecutingAssembly() : Assembly.Load(assemblyName);
           return (from t in assembly.GetTypes()
                where t.IsClass && !t.IsNested && !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith(path)
                select t).ToList();
        }
    }
}