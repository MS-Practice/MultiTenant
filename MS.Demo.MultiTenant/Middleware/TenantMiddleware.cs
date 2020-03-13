using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Demo.MultiTenant.Middleware
{
    class TenantMiddleware<T> where T:Tenant
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Items.ContainsKey(Constants.HttpContextTenantKey))
            {
                var tenantService = context.RequestServices.GetService(typeof(TenantAccessService<T>)) as TenantAccessService<T>;
                context.Items.Add(Constants.HttpContextTenantKey, await tenantService.GetTenantAsync());
            }

            if (_next != null)
                await _next(context);
        }
    }
}
