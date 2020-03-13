using Microsoft.AspNetCore.Mvc;
using MS.Demo.MultiTenant;
using MS.Demo.MultiTenant.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MultiTenantDemo.Controllers
{
    [ApiController]
    [Route("api/v1/{controller}")]
    public class MultiTenantController : ControllerBase
    {
        private readonly ITenantResolutionResolver _tenantResolutionResolver;
        private readonly ITenantStore<Tenant> _tenantStore;
        public MultiTenantController(
            ITenantResolutionResolver tenantResolutionResolver,
            ITenantStore<Tenant> tenantStore)
        {
            _tenantResolutionResolver = tenantResolutionResolver;
            _tenantStore = tenantStore;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            var identifier = await _tenantResolutionResolver.GetTenantIdentifierAsync();

            var tenant = await _tenantStore.GetTenantAsync(identifier);

            return identifier + " 租户：" + JsonSerializer.Serialize(tenant);
        }
    }


    [ApiController]
    [Route("api/v2/multitenant")]
    public class MultiTenantV2Controller : ControllerBase
    {
        private readonly TenantAccessService<Tenant> _tenantAccessService;
        public MultiTenantV2Controller(
            TenantAccessService<Tenant> tenantAccessService
            )
        {
            _tenantAccessService = tenantAccessService;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            var tenant = await _tenantAccessService.GetTenantAsync();

            var currentTenant = HttpContext.GetTenant();

            return "租户：" + JsonSerializer.Serialize(tenant) + Environment.NewLine + "当前租户："+JsonSerializer.Serialize(currentTenant);
        }
    }
}
