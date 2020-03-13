using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Demo.MultiTenant
{
    public class TenantBuilder<T> where T : Tenant
    {
        private readonly IServiceCollection _services;

        public TenantBuilder(IServiceCollection services)
        {
            services.AddTransient<TenantAccessService<T>>();
            _services = services;
        }

        /// <summary>
        /// 注册租户解析器的具体实现
        /// </summary>
        /// <typeparam name="TImp"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithResolutionResolver<TImp>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            _services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _services.Add(ServiceDescriptor.Describe(typeof(ITenantResolutionResolver), typeof(TImp), lifetime));
            return this;
        }

        /// <summary>
        /// 注册租户存储的具体实现
        /// </summary>
        /// <typeparam name="TImp"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder<T> WithStore<TImp>(ServiceLifetime lifetime = ServiceLifetime.Transient) where TImp : class, ITenantStore<T>
        {
            _services.Add(ServiceDescriptor.Describe(typeof(ITenantStore<T>), typeof(TImp), lifetime));
            return this;
        }
    }
}
