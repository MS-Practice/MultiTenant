using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Demo.MultiTenant
{
    public class InMemoryTenantStore : ITenantStore<Tenant>
    {
        public async Task<Tenant> GetTenantAsync(string identifier)
        {
            var tenant = new[]
            {
                // 这里采取的是硬编码形式加载多租户
                // 应该用配置文件（Option）模式动态加载指定租户
                new Tenant{ Id = "80fdb3c0-5888-4295-bf40-ebee0e3cd8f3",Identifier = "localhost"}
            }.SingleOrDefault(t => t.Identifier == identifier);

            return await Task.FromResult(tenant);
        }
    }
}
