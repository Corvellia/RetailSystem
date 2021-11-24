using Messages;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing
{
    class OrderPlacedHandler : IHandleMessages<OrderPlaced>
    {
        public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            log.Info($"Received OrderPlaced, OrderId = {message.OrderId}");

            await context.Publish(new OrderBilled { OrderId = message.OrderId });

        }

        static ILog log = LogManager.GetLogger<OrderPlacedHandler>();
    }
}
