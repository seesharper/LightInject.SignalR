namespace LightInject.SignalR.Tests
{
    using LightInject.Interception;

    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;
    using Xunit;

    
    public class HubActivatorTests
    {
        [Fact]
        public void GetInstance_HubActivator_ReturnsLightInjectHubActivator()
        {
            var container = new ServiceContainer();            
            var resolver = new LightInjectDependencyResolver(container);

            var instance = resolver.GetService(typeof(IHubActivator));

            Assert.IsType(typeof(LightInjectHubActivator), instance);
        }
                
        [Fact]
        public void Create_Hub_ReturnsHubProxy()
        {                                    
            var container = new ServiceContainer();
            container.EnableSignalR();
            container.Register<SampleHub>(new PerScopeLifetime());
            var activator = new LightInjectHubActivator(container);                     
            var hubDescriptor = new HubDescriptor { HubType = typeof(SampleHub) };
            
            var hub = activator.Create(hubDescriptor);

            Assert.IsAssignableFrom<IProxy>(hub);            
        }

        [Fact]
        public void Create_Hub_StartsScope()
        {
            var container = new ServiceContainer();
            container.EnableSignalR();
            container.Register<SampleHub>();
            var activator = new LightInjectHubActivator(container);
            var hubDescriptor = new HubDescriptor { HubType = typeof(SampleHub) };
            using (activator.Create(hubDescriptor))
            {
                Assert.NotNull(container.ScopeManagerProvider.GetScopeManager().CurrentScope);
            }                       
        }

        [Fact]
        public void Dispose_Hub_ClosesScope()
        {
            var container = new ServiceContainer();
            container.EnableSignalR();
            container.Register<SampleHub>();
            var activator = new LightInjectHubActivator(container);
            var hubDescriptor = new HubDescriptor { HubType = typeof(SampleHub) };

            var hub = activator.Create(hubDescriptor);

            hub.Dispose();

            Assert.Null(container.ScopeManagerProvider.GetScopeManager().CurrentScope);
        }
    }
}