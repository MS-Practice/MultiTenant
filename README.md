# 如何自行实现一个多租户系统

注意：前情概要描述的文字比较多，说的是我的思考过程，不感兴趣的可以直接到跳到 “解析租户信息” 一节。

现如今框架满天飞的环境下，好像很少机会需要自己来实现一个模块。毕竟这样能节省很多的开发时间，提高效率。

这就是框架的好处，也是我们使用框架的直接原因。

情况总有例外，假设刚好我们公司没有用到框架，用的就是 .netcore 平台新建项目，直接开干一把唆。由于前期工作没有考虑周全，现在发现公司新建的平台项目的业务数据越来越大，提供给用户的数量越来越多。但是这些不同的用户的数据肯定不能互相干扰。

举个例子说明，例如我举个跟我公司接近的一种情况，公司再搭建数据平台，来给不同学校提供资料。并且我们的数据平台要记录合作学校对应的学生和老师。前面有假设提到公司前期考虑不周，我们把所有的学校放在 school 表中，所有学生放在 student 表中。所有老师放在 teacher 表中。这样当公司系统在给他们（用户）提供数据的时候，是不是每次都要判断当前用户在哪个学校，然后再把对应的学校资料推送给他们。不仅如此，对数据敏感的增删改操作对这种混在一起数据要各位小心。一不小心，可能就会发生误删其他学校的信息。

为了有效的解决这个问题，我们第一要做的就是要将数据分开管理，彼此互不干扰。这是我们要实现的最终目的。

想要的效果有了，现在的问题是能不能实现，该如何实现，怎么实现才算好。

我们做事情的目的就是解决问题，在前面我们分析了我们要把数据在一个系统中隔离。那么我们自然能想到的就是以学校为领域划分为不同的库。这样我们在系统运行的时候就能做到在用户选择对应的学校登陆系统时，就只能访问这个学校的所有信息了。

到这里，我们就很清晰了，如果我们平时多看多听到别人谈论新的知识点或框架时，我们就会知道对于这种情况，“多租户”就是为了这种情况而诞生的。

既然要做 “多租户” 系统，并且团队之间没有使用市面上的多租户框架。那么我们就得自己实现一个了。那么要做的第一件就是要了解 “多租户” 的概念。正所谓知己知彼，方能战无不胜。

# 什么是多租户

我们来看下维基百科对多租户的定义是什么（以下是概述）

多租户软件架构就是在同一个系统实例上运行不同用户，能做到应用程序共享，服务自治，并且还能做到数据互相隔离的软件架构思想。一个租户就相当于一组用户（比如针对学校来说，一个学校就是一个租户，这个租户下有学生，老师作为用户（一组用户））。

现在我们总结一下我们要做什么？

我们要实现：

1. 相同的应用程序 app 下
2. 解析出登陆系统的（当前用户）是属于哪一个租户（对应到例子就是学校）。
3. 根据解析出来的租户信息，来访问对应的数据库信息。

现在我们就来实现上面说的步骤。第一步不用想，肯定要得一个 app 下。

# 解析租户信息

现在我们要设计如何才能让系统检测到当前用户的租户信息。

现阶段我们能想到的解析方式有三种：

1. 域名：例如 tenant1.example.com，tenant2.example.com
2. URL：例如 www.example.com/tenant1/，www.example.com/tenant2
3. header：例如 [x-header: 'tenant1']，[x-header: 'tenant2']

一下子有这么多解决方式，是不是自信心起来了，有木有。

具体如何用代码实现呢？首先要定义一个 “租户” 的信息体，为了方便表述我这里用的是类（当然也可以用接口）

```c#
public class Tenant {
	public string Identifier { get; set;}
  public string Id { get; set;}
}
```

只要继承了这个租户类，就表示拥有了这个租户信息。有了租户之后，我们紧接着要做的就是解析了。因为前面有讨论我们解析方式有三种，这里我主要讨论第一种的实现方案。正是因为有多种可能，解析方式对于架构来说是不稳定的，所以我们要封装变化来抽象画。我们先定一个解析租户接口类，然后提供一个实现类具体以域名方式解析，这样封装就达到对修改封闭，新增开放（OCP）的目的了。例如用户可以自行继承接口用 URL 方式解析租户信息。

```c#
public interface ITenantResolver {
	Task<string> GetTenantIdentifierAsync();
}

public class DomainTenantResolver : ITenantResolver {
  private readonly IHttpContextAccessor _accessor;
  public DomainTenantResolver(IHttpContextAccessor accessor) {
    _accessor = accessor;
  }
  // 这里就解析道了具体的域名了，从而就能得知当前租户
  public async Task<string> GetTenantIdentifierAsync() {
    return await Task.FromResult(_accessor.HttpContext.Request.Host.Host);
  }
}
```

接着我们拿到租户标识符，要干嘛呢？自然是要存起来的，好让系统很方便的获取当前用户的租户信息。

# 存储租户信息

关于存储功能，同样我们选择抽象出来一个 `ITenantStore` 接口。为什么要抽象出来，作为一个基础功能架构设计。我们就应该考虑这个功能的解决方案是否是稳定的。明显，对于存储来说，方式太多了。所以作为系统，要提供一个基本实现的同时还要供开发者方便选择其他方式。

```c#
public interface ITenantStore {
	Task<T> GetTenantAsync(string identifier);
}
```

关于存储，其实我们可以选择将租户信息放入内存中，也可以选择放入配置文件，当然你选择将租户信息放入数据库也是没问题的。

> 现在的最佳实践是将一些敏感信息，比如每个租户对应的链接字符串都是以 Option 配置文件方式存储的。利用 .netcore 内置 DI 做到即拿即用。

这里为了简便，我选择用硬编码的方式存储租户信息

```c#
public class InMemoryTenantStore: ITenantStore {
  private Tenant[] tenantSource = new[] {
			new Tenant{ Id = "4da254ff-2c02-488d-b860-cb3b6363c19a", Identifier = "localhost" }
	};
	public async Task<T> GetTenantAsync(string identifier) {
    var tenant = tenantSource.FirstOrDefault(p => p.Identifier == identifier);
		return await Task.FromResult(tenant);
	}
}
```

好了，现在我们租户信息有了，解析器也提供了，存储服务也决定了。那么接下来就只剩下什么了？

# 进入管道捕获源头

剩下的就是找到请求的源头，很显然，.netcore 优良的设计，我们可以很方便的将上述我们准备的服务安排至管道中。那就是注册服务（AddXXXService）和中间件（UseXXX）。

所以我们这一步要做的就是

1. 注册解析租户信息服务
2. 注册中间件，好让每一次请求发起时截获信息将用户的租户信息存至这个请求（HttpContext）里面，好让系统随时访问当前用户租户信息。

## 注册服务类

这个太简单了，.netcore 的源代码给了我们很好的范例

```c#
public static class ServiceCollectionExtensions {
	public static AddMultiTenancy<T>(this IServiceColletion services, Action<IServiceCollection> registerAction) where T : Tenant {
		service.TryAddSingleton<IHttpContextAccessor,HttpContextAccessotr>();  // 这一步很重要
		registerAction?.Invoke(services);
	}
}
```

调用：

```c#
// Startup.cs ConfigureServices

services.AddMultiTenancy<Tenant>(s => {
	// 注册解析类
	s.AddScoped(typeof(ITenantResolver), typeof(DomainTenantResolver));
	// 注册存储
	s.AddScoped(typeof(ITenantStore), typeof(InMemoryStore));
})
```

这样我们就能在系统中比如控制器，注入这两个类来完成对当前租户信息的访问。

这样我们就实现了整个请求对当前租户操作过程了。所以本文就结束了。

不好意思，开个玩笑。还没结束，其实上面是我第一版的写法。不知道大家有没有发现，我这样写其实是有 “问题” 的。大毛病没有，就是对开发者不友好。

首先，在 ConfigureServices 方法里的注册操作，我的 AddMultiTenancy 方法不纯粹。这是我当时写这个 demo 时候感觉特别明显的。因为起初我的方法签名是不带回调函数 action 的。

```c#
public static IServiceCollection AddMultiTenancy<T>(this IServiceColletion services) where T : Tenant {
  services.TryAddSingleton<IHttpContextAccessor,HttpContextAccessotr>();  // 这一步很重要
  services.Add(typeof(ITenantResolver), typeof(ImlITenantResolver), LifetimeScope);
  services.Add(typeof(ITenantStore), typeof(ImlITenantStore), LifetimeScope);
  return services;
}
```

注册服务解决了，然后是中间件

## 注册中间件

中间件所干的事，很简单，就是捕获进来管道的请求上下文，然后解析得出租户信息，然后把对应的租户信息放入请求上下文中。

```c#
class MultiTenantMiddleware<T> where T : Tenant {
	private readonly RequestDelegate _next;

  public TenantMiddleware(RequestDelegate next)
  {
      _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
      if (!context.Items.ContainsKey("localhost"))
      {
          var tenantService = context.RequestServices.GetService(typeof(TenantAppService<T>)) as TenantAppService<T>;
          // 这里也可以放到其他地方，比如 context.User.Cliams 中
          context.Items.Add("localhost", await tenantService.GetTenantAsync());
      }

      if (_next != null)
          await _next(context);
  }
}
```

但是在注册租户解析类和存储类时，发现没有实现类型和生命周期做参数，根本无法注册。如果把两个参数当成方法签名，那不仅使这个方法变得丑陋，还固话了这个方法的使用。

所以最后我改成了上面用回调的方式，暴露给开发者自己去注册。所以这就要求开发者必须要清楚要注册那些内容。

所以后来一次偶然的机会看到相关的资料，告诉我其实可以借助 Program.cs 中的 Builder 模式改善代码，可以让代码结构更加表义化。第二版如下

```c#
public static class ServiceCollectionExtensions {
	public static TenantBuilder<T> AddMultiTenancy<T>(this IServiceColletion services) where T : Tenant {
		return new TenantBuilder<T>(services);
	}
}
```

```c#
public class TenantBuilder<T> where T : Tenant {
	private readonly IServiceCollection _services;
	public TenantBuilder(IServiceCollection services) {
		_services = services;
	}
	
	public TenantBuilder<T> WithTenantResolver<TIml>(ServiceLifetime lifttime = ServiceLifetime.Transient) where TIml : ITenantResolver {
		_services.TryAddSingleton<IHttpContextAccessor,HttpContextAccessotr>();  // 这一步很重要
		_services.Add(typeof(ITenantResolver), typeof(TImp), lifttime);
		return this;
	}
	
	public TenantBuilder<T> WithStore<TIml>(ServiceLifetime lifttime = ServiceLifetime.Transient) {
		_services.Add(typeof(ITenantStore), typeof(TIml), lifetime);
		return this;
	}
}
```

所以调用我们就变成这样了

```c#
services.AddMultiTenancy()
		.WithTenantResolver<DomainTenantResolver>()
		.WithTenantStore<InMemoryTenatnStore>();
```

这样看起来是不是更具表义化和优雅了呢。

我们重构了这一点，还有一点让我不满意。那就是为了获取当前用户租户信息，我必须得注入两个服务类 —— 解析类和存储类。这点既然想到了还是要解决的，因为很简单。就是平常我们使用的外观模式。

我们加入一个特定租户服务类来代替这两个类不就好了么。

```c#
public class TenantAppService<T> where T : Tenant {
	private readonly ITenantResolver _tenantResolver;
	private readonly ITenantStore _tenantStore;
	
	public TenantAppService(ITenantResolver tenantResolver, ITenantStore tenantStore) {
		_tenantResolver = tenantResolver;
		_tenantStore = tenantStore;
	}
	
	public async Task<T> GetTenantAsync() {
		var identifier = await _tenantResolver.GetTenantIdentifierAsync();
		return await _tenantStore.GetTenantAsync(identifier);
	}
}
```

这样我们就只需要注入 TenantAppService 即可。

其实现在我们实现一个多租户系统已经达到 90% 了。剩下的就是如何在数据访问层根据获取的租户信息切换数据库。实现方法其实也很简单，就是在注册完多租户后，在数据库上下文选择链接字符串那里替换你获取的多租户信息所对应的数据库 ID 即可。具体的代码实现这个后面再聊。

# 总结

回顾一下，我们目前做的事。

1. 发现问题：数据混在在一起无法做到完美的数据隔离，不好控制。
2. 了解原理：什么是多租户
3. 解决方案：为了解决问题想到的可实现的技术方案
4. 在架构上考虑如何优化重构一个模块。

发现没有，我们做事一定是要 “带着问题解决问题”。首先是解决问题，然后才是重构。千万不要在一开始就想着要重构。

其实我们在解决一个问题时，我们项目架构可能没有其中某一个模块，当要用到这个模块时，我们怎么做的。其实一个快速有效的访问，就是去看有这个模块功能开源框架，去学习里面的思想。看他们是如何做的。然后有了思路就可以依葫芦画瓢了，甚至是可以直接粘贴拷贝。



参考资料：https://michael-mckenna.com/multi-tenant-asp-dot-net-core-application-tenant-resolution **推荐阅读**



