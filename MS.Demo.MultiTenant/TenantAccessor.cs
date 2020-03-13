using Microsoft.AspNetCore.Http;
using MS.Demo.MultiTenant.Extension;

namespace MS.Demo.MultiTenant
{
    public class TenantAccessor<T> : ITenantAccessor<T> where T : Tenant
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenantAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public T Tenant => _httpContextAccessor.HttpContext.GetTenant<T>();
    }
}
