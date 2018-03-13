# Akka.DI.Microsoft

**Actor Producer Extension** backed by the [Microsoft Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) Dependency Injection Container for the [Akka.NET](https://github.com/akkadotnet/akka.net) framework.

## What is it?

**Akka.DI.Microsoft** is an **ActorSystem extension** for the Akka.NET framework that provides an alternative to the basic capabilities of [Props](http://getakka.net/docs/Props) when you have Actors with multiple dependencies. 

## How to you use it?

The best way to understand how to use it is by example. If you are already considering this extension then we will assume that you know how how to use the [Simple Injector](https://Microsoft.org/index.html) container. This example is demonstrating a system using [ConsistentHashing](http://getakka.net/docs/working-with-actors/Routers#consistenthashing) routing along with this extension.

Start by creating your container and registering your actors and dependencies.

```csharp
// Setup Microsoft Dependency Injection
var services = new ServiceCollection();
services.AddScoped<IWorkerService, WorkerService>();
services.AddScoped<ITypedWorker, TypedWorker>();
```

Next you have to create your ```ActorSystem``` and inject that system reference along with the container reference into a new instance of the ```MicrosoftDependencyResolver```.

```csharp
// Create the ActorSystem
using (var system = ActorSystem.Create("MySystem"))
{
    // Create the dependency resolver
    var resolver = new MicrosoftDependencyResolver(scopeFactory, system);

    // we'll fill in the rest in the following steps
}
```

To register the actors with the system use method ```Akka.Actor.Props Create<TActor>()``` of the  ```IDependencyResolver``` interface implemented by the ```MicrosoftDependencyResolver```.

```csharp
// Register the actors with the system
system.ActorOf(resolver.Create<TypedWorker>(), "Worker1");
system.ActorOf(resolver.Create<TypedWorker>(), "Worker2");

// OR

system.DI().Props<TypedWorker>("Worker1");
system.DI().Props<TypedWorker>("Worker2");
```

Finally create your router, message and send the message to the router.

```csharp
// Create the router
var router = system.ActorOf(Props.Empty.WithRouter(new ConsistentHashingGroup(config)));

// Create the message to send
var message = new TypedActorMessage
{
   Id = 1,
   Name = Guid.NewGuid().ToString()
};

// Send the message to the router
router.Tell(message);
```

The resulting code should look similar to the the following:
```csharp
// Setup Microsoft Dependency Injection
var services = new ServiceCollection();
services.AddScoped<IWorkerService, WorkerService>();
services.AddScoped<ITypedWorker, TypedWorker>();

// Create the ActorSystem
using (var system = ActorSystem.Create("MySystem"))
{
    // Create the dependency resolver
    var resolver = new MicrosoftDependencyResolver(container, system);

    // Register the actors with the system
    system.ActorOf(resolver.Create<TypedWorker>(), "Worker1");
    system.ActorOf(resolver.Create<TypedWorker>(), "Worker2");

    // OR

    var props1 = system.DI().Props<TypedWorker>("Worker1");
    var props2 = system.DI().Props<TypedWorker>("Worker2");

    // Create the router
    var router = system.ActorOf(Props.Empty.WithRouter(new ConsistentHashingGroup(config)));

    // Create the message to send
    var message = new TypedActorMessage
    {
       Id = 1,
       Name = Guid.NewGuid().ToString()
    };

    // Send the message to the router
    router.Tell(message);
}
```
