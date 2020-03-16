using Microsoft.AspNetCore.Builder;
using MS.Demo.MultiTenant.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Demo.MultiTenant.Extension
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 使用租户中间件处理请求
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMultiTenancy<T>(this IApplicationBuilder builder) where T : Tenant
        => builder.UseMiddleware<TenantMiddleware<T>>();

        public static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder builder) => builder.UseMiddleware<TenantMiddleware<Tenant>>();

        public static IApplicationBuilder UseMultiTenantContainer<T>(this IApplicationBuilder builder) where T : Tenant => builder.UseMiddleware<MultiTenantContainerMiddleware<T>>();
    }
}
