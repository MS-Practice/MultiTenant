using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MS.Demo.MultiTenant.TenantManage
{
    internal class MultiTenantContainer<T> : IContainer where T : Tenant
    {
        /// <summary>
        /// 基本应用容器
        /// </summary>
        private readonly IContainer _applicationContainer;
        /// <summary>
        /// 操作配置构建器
        /// </summary>
        private readonly Action<T, ContainerBuilder> _tenantContainerConfiguration;

        /// <summary>
        /// 这个字典能追踪我们创建的所有的租户生命周期
        /// </summary>
        private readonly Dictionary<string, ILifetimeScope> _tenantLifetimeScopes = new Dictionary<string, ILifetimeScope>();

        private static readonly object _lock = new object();
        private const string _multiTenantTag = "multitenantcontainer";

        public MultiTenantContainer(IContainer applicationContainer, Action<T,ContainerBuilder> tenantContainerConfiguration)
        {
            _tenantContainerConfiguration = tenantContainerConfiguration;
            _applicationContainer = applicationContainer;
        }

        /// <summary>
        /// 从应用程序容器取当前租户
        /// </summary>
        /// <returns></returns>
        private T GetCurrentTenant() => _applicationContainer.Resolve<TenantAccessService<T>>().GetTenantAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public ILifetimeScope GetCurrentTenantScope() => GetTenantScope(GetCurrentTenant()?.Id);

        public ILifetimeScope GetTenantScope(string tenantId)
        {
            if (tenantId == null)
                return _applicationContainer;

            if (_tenantLifetimeScopes.ContainsKey(tenantId))
                return _tenantLifetimeScopes[tenantId];

            lock (_lock)
            {
                if (_tenantLifetimeScopes.ContainsKey(tenantId))
                {
                    return _tenantLifetimeScopes[tenantId];
                }
                else
                {
                    _tenantLifetimeScopes.Add(tenantId, _applicationContainer.BeginLifetimeScope(_multiTenantTag, a => _tenantContainerConfiguration(GetCurrentTenant(), a)));
                    return _tenantLifetimeScopes[tenantId];
                }
            }
        }

        public IDisposer Disposer => throw new NotImplementedException();

        public object Tag => throw new NotImplementedException();

        public IComponentRegistry ComponentRegistry => throw new NotImplementedException();

        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;
        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding;
        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning;

        public ILifetimeScope BeginLifetimeScope()
        {
            throw new NotImplementedException();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            throw new NotImplementedException();
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            throw new NotImplementedException();
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var scope in _tenantLifetimeScopes)
                {
                    scope.Value.Dispose();
                }
                _applicationContainer.Dispose();
            }
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public object ResolveComponent(ResolveRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
