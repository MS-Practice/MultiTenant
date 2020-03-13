using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Demo.MultiTenant
{
    /// <summary>
    /// 这是模仿 IHttpContextAccessor 访问模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITenantAccessor<T> where T : Tenant
    {
        T Tenant { get; }
    }
}
