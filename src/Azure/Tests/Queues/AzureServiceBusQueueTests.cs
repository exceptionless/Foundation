﻿using Foundatio.Queues;
using Foundatio.Tests.Queue;
using Foundatio.Tests.Utility;
using Microsoft.ServiceBus;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Foundatio.Azure.Tests.Queue {
    public class AzureServiceBusQueueTests : QueueTestBase {
        private static readonly string QueueName = Guid.NewGuid().ToString("N");

        public AzureServiceBusQueueTests(ITestOutputHelper output) : base(output) {}

        protected override IQueue<SimpleWorkItem> GetQueue(int retries = 1, TimeSpan? workItemTimeout = null, TimeSpan? retryDelay = null, int deadLetterMaxItems = 100, bool runQueueMaintenance = true) {
            if (String.IsNullOrEmpty(ConnectionStrings.Get("ServiceBusConnectionString")))
                return null;

            if (!retryDelay.HasValue)
                retryDelay = TimeSpan.Zero;

            var maxBackoff = retryDelay.Value > TimeSpan.Zero
                ? retryDelay.Value + retryDelay.Value
                : TimeSpan.FromSeconds(1);
            var retryPolicy = new RetryExponential(retryDelay.Value, maxBackoff, retries + 1);

            var factory = new AzureServiceBusQueue<SimpleWorkItem>.Factory(ConnectionStrings.Get("ServiceBusConnectionString"))
                .Queue(QueueName)
                .Retries(retries)
                .RecreateQueue(false)
                .RetryPolicy(retryPolicy)
                .LoggerFactory(Log);

            if (workItemTimeout != null)
                factory.Timeout(workItemTimeout.Value);

            return factory.Build().Result;
        }

        [Fact]
        public override Task CanQueueAndDequeueWorkItem() {
            return base.CanQueueAndDequeueWorkItem();
        }
        
        [Fact]
        public override Task CanDequeueWithCancelledToken() {
            return base.CanDequeueWithCancelledToken();
        }

        [Fact]
        public override Task CanQueueAndDequeueMultipleWorkItems() {
            return base.CanQueueAndDequeueMultipleWorkItems();
        }

        [Fact]
        public override Task WillWaitForItem() {
            return base.WillWaitForItem();
        }

        [Fact]
        public override Task DequeueWaitWillGetSignaled() {
            return base.DequeueWaitWillGetSignaled();
        }

        [Fact]
        public override Task CanUseQueueWorker() {
            return base.CanUseQueueWorker();
        }

        [Fact]
        public override Task CanHandleErrorInWorker() {
            return base.CanHandleErrorInWorker();
        }

        [Fact]
        public override Task WorkItemsWillTimeout() {
            return base.WorkItemsWillTimeout();
        }

        [Fact]
        public override Task WorkItemsWillGetMovedToDeadletter() {
            return base.WorkItemsWillGetMovedToDeadletter();
        }

        [Fact]
        public override Task CanAutoCompleteWorker() {
            return base.CanAutoCompleteWorker();
        }

        [Fact(Skip="Base code incompatible: ServiceBus expects multiple QueueClients, not Queues.")]
        public override Task CanHaveMultipleQueueInstances() {
            return base.CanHaveMultipleQueueInstances();
        }

        [Fact]
        public override Task CanRunWorkItemWithMetrics() {
            return base.CanRunWorkItemWithMetrics();
        }

        [Fact]
        public override Task CanRenewLock() {
            return base.CanRenewLock();
        }

        [Fact(Skip = "Abandon doesn't throw after throwing once.")]
        public override Task CanAbandonQueueEntryOnce() {
            return base.CanAbandonQueueEntryOnce();
        }

        [Fact]
        public override Task CanCompleteQueueEntryOnce() {
            return base.CanCompleteQueueEntryOnce();
        }

        // NOTE: Not using this test because you can set specific delay times for servicebus
        public override Task CanDelayRetry() {
            return base.CanDelayRetry();
        }
    }
}