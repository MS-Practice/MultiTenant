using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MS.Demo.MultiTenant.TenantManage;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Demo.MultiTenant.Extension
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加租户服务，用户自定义租户
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static TenantBuilder<T> AddMultiTenancy<T>(this IServiceCollection services) where T : Tenant
            => new TenantBuilder<T>(services);

        /// <summary>
        /// 添加租户服务，默认租户类
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static TenantBuilder<Tenant> AddMultiTenancy(this IServiceCollection services) => new TenantBuilder<Tenant>(services);


        public static IServiceProvider UseMultiTenantServiceProvider<T>(this IServiceCollection services, Action<T, ContainerBuilder> registerServicesForTenant) where T : Tenant
        {
            var containerBuilder = new ContainerBuilder();

            MultiTenantContainer<T> container = null;
            Func<MultiTenantContainer<T>> containerAccessor = () => container;

            services.AddSingleton(containerAccessor);

            containerBuilder.Populate(services);

            // 创建并赋值多租户容器
            container = new MultiTenantContainer<T>(containerBuilder.Build(), registerServicesForTenant);
            // 返回新的 IServiceProvider 来代替内置标准的服务提供者
            return new AutofacServiceProvider(containerAccessor());
        }
    }
}
