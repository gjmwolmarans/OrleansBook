using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.TestingHost;
using OrleansBook.GrainClasses;

namespace OrleansBook.Tests;

class SiloBuilderConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddMemoryGrainStorage("robotStateStore");
        var mockState = new Mock<IPersistentState<RobotState>>();
        mockState.Setup(s => s.State).Returns(new RobotState());
        siloBuilder.ConfigureServices(services =>
        {
            services.AddSingleton(mockState.Object);
            services.AddSingleton(new Mock<ILogger<RobotGrain>>().Object);
        });
    }
}