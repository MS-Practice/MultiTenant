using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MS.Demo.MultiTenant;
using MS.Demo.MultiTenant.Extension;

namespace MultiTenantDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddMultiTenancy()
                .WithResolutionResolver<HostResolutionResolver>()
                .WithStore<InMemoryTenantStore>();

            services.AddControllers();

            return services.UseMultiTenantServiceProvider<Tenant>((t, c) =>
            {
                c.RegisterInstance(new OperationIdService()).SingleInstance();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // ʹ�ö��⻧�м�������⻧��Ϣ���� HttpContext.Items
            // �������������߾ͱ���Ҫ��ʱ��ȡ���� Items ��Ӧ��ֵ
            // �������ǻ��ǵý���һЩ�������򣬷����ȡ��ǰ�⻧��Ϣ��
            //app.UseMultiTenancy();

            app.UseMultiTenancy()
                .UseMultiTenantContainer<Tenant>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static void ConfigureMultiTenantServices(Tenant t, ContainerBuilder c)
        {
            c.RegisterInstance(new OperationIdService()).SingleInstance();
        }
    }
}
