﻿using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabric.Logging.RemotingServiceDispatcher;
using ServiceInterfaces;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using ServiceFabric.Logging.Extensions;

namespace MyStateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class MyStateless : StatelessService, IMyService
    {
        private readonly ILogger logger;
        private int requestCount;

        public MyStateless(StatelessServiceContext context, ILogger logger)
            : base(context)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context =>
                            new WcfServiceRemotingListener(context,
                            new TracingEnabledRemotingServiceDispatcher(context, this),null,"ServiceEndpoint")) };
        }

        public Task<int> CalculateSum(int a, int b)
        {
            logger.LogMetric("Times requested", ++requestCount);

            return Task.FromResult(a + b);
        }
    }
}
