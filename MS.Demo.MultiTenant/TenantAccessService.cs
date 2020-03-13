using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Demo.MultiTenant
{
    /// <summary>
    /// 租户访问服务
    /// 这样在应用端（控制器）就不用直接注入租户解析器了，简化访问流程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TenantAccessService<T> where T : Tenant
    {
        private readonly ITenantResolutionResolver _tenantResolutionResolver;
        private readonly ITenantStore<T> _tenantStore;

        public TenantAccessService(
            ITenantResolutionResolver tenantResolutionResolver,
            ITenantStore<T> tenantStore)
        {
            _tenantResolutionResolver = tenantResolutionResolver;
            _tenantStore = tenantStore;
        }

        public async Task<T> GetTenantAsync()
        {
            var tenantIdentifier = await _tenantResolutionResolver.GetTenantIdentifierAsync();
            return await _tenantStore.GetTenantAsync(tenantIdentifier);
        }
    }
}
