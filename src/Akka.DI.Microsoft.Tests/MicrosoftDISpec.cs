using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.TestKit;
using Akka.TestKit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Akka.DI.Microsoft.Tests
{
  public class MicrosoftDISpec : DiResolverSpec
  {
    protected override object NewDiContainer()
    {
      var services = new ServiceCollection();

      return services;
    }

    protected override IDependencyResolver NewDependencyResolver(object diContainer, ActorSystem system)
    {
      var services = ToServiceCollection(diContainer);
      var serviceProvider = services.BuildServiceProvider();
      var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

      return new MicrosoftDependencyResolver(scopeFactory, system);
    }

    protected override void Bind<T>(object diContainer, Func<T> generator)
    {
      var services = ToServiceCollection(diContainer);

      services.AddScoped(typeof(T), provider => generator());
    }

    protected override void Bind<T>(object diContainer)
    {
      var services = ToServiceCollection(diContainer);

      services.AddScoped(typeof(T));
    }

    private static IServiceCollection ToServiceCollection(object diContainer)
    {
      var container = diContainer as IServiceCollection;

      Assert.True(container != null, "container != null");

      return container;
    }
  }
}
