using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Demo.MultiTenant
{
    /// <summary>
    /// 租户存储，加载具体指定的租户信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITenantStore<T> where T : Tenant
    {
        Task<T> GetTenantAsync(string identifier);
    }
}
