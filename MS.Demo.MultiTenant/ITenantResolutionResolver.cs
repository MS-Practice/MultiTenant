using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Demo.MultiTenant
{
    /// <summary>
    /// 租户决议解析，解析 Http 请求在哪个租户上下文运行
    /// </summary>
    public interface ITenantResolutionResolver
    {
        Task<string> GetTenantIdentifierAsync();
    }
}
