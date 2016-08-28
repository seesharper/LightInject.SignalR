using System;
using Xunit;

namespace LightInject.SignalR.Tests
{
    using System.Linq;

    using LightInject.SampleLibrary;

    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Tracing;

    using Moq;

    
    public class DependencyResolverTests
    {
        [Fact]
        public void GetService_ServiceRegisteredThroughAdapter_ReturnsInstance()
        {
            var container = new ServiceContainer();
            var resolver = new LightInjectDependencyResolver(container);
            resolver.Register(typeof(IFoo), () => new Foo());

            var instance = resolver.GetService(typeof(IFoo));

            Assert.IsAssignableFrom<IFoo>(instance);            
        }

        [Fact]
        public void GetService_ServiceRegisteredWithContainer_ReturnsInstance()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();            
            var resolver = new LightInjectDependencyResolver(container);           
            
            var instance = resolver.GetService(typeof(IFoo));

            Assert.IsAssignableFrom<IFoo>(instance);
        }

        [Fact]
        public void GetServices_ServiceRegisteredWithContainer_ReturnsServices()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            var resolver = new LightInjectDependencyResolver(container);

            var instances = resolver.GetServices(typeof(IFoo));

            Assert.Equal(1, instances.Count());
        }

        [Fact]
        public void GetServices_UnknownService_ReturnsNull()
        {
            var container = new ServiceContainer();            

            var resolver = new LightInjectDependencyResolver(container);

            var instances = resolver.GetServices(typeof(IFoo));

            Assert.Null(instances); 
        }

        [Fact]
        public void GetServices_ServiceTypeExistsInAdapterAndBase_ReturnsBothInstances()
        {
            var container = new ServiceContainer();                        
            var traceManagerMock = new Mock<ITraceManager>();
            container.RegisterInstance(traceManagerMock.Object);
            var resolver = new LightInjectDependencyResolver(container);            
            
            var instances = resolver.GetServices(typeof(ITraceManager));
            
            Assert.Equal(2, instances.Count());
        }


        [Fact]
        public void Dispose_Adapter_DisposesContainer()
        {
            var containerMock = new Mock<IServiceContainer>();

            using (new LightInjectDependencyResolver(containerMock.Object))
            {                
            }

            containerMock.Verify(c => c.Dispose(), Times.Once);            
        }
    }
}
