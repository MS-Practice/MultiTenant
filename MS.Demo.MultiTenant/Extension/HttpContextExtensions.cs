using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Demo.MultiTenant.Extension
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取当前租户
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T GetTenant<T>(this HttpContext context) where T:Tenant
        {
            if (!context.Items.ContainsKey(Constants.HttpContextTenantKey))
                return default;
            return context.Items[Constants.HttpContextTenantKey] as T;
        }
        /// <summary>
        /// 获取当前租户
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Tenant GetTenant(this HttpContext context)
        {
            return GetTenant<Tenant>(context);
        }
    }
}
