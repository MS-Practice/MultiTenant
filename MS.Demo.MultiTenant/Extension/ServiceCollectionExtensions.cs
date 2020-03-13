using Microsoft.Extensions.DependencyInjection;
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
    }
}
