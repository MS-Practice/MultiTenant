using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using MS.Demo.MultiTenant.TenantManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Demo.MultiTenant.Middleware
{
    /// <summary>
    /// 这个新建的当前租户中间件。由于要在程序启动时就要确定租户信息
    /// 所以这个中间件一定要再所有的中间件的前面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class MultiTenantContainerMiddleware<T> where T : Tenant
    {
        private readonly RequestDelegate next;
        public MultiTenantContainerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, Func<MultiTenantContainer<T>> multiTenantContainerAccessor)
        {
            // 设置当前租户容器 Container
            // 给每一个请求的标准生命周期域开始一个新的域
            context.RequestServices = new AutofacServiceProvider(
                multiTenantContainerAccessor().GetCurrentTenantScope().BeginLifetimeScope()
                );

            await next(context);
        }
    }
}
