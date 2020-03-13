using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Demo.MultiTenant
{
    /// <summary>
    /// 基于 Host 协议解析租户信息
    /// </summary>
    public class HostResolutionResolver : ITenantResolutionResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HostResolutionResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 获取上下文标识符
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTenantIdentifierAsync()
        {
            return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Host.Host);
        }
    }
}
