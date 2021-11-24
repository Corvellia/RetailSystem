using Messages;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping
{
    public class ShippingPolicy : Saga<ShippingPolicy.SagaData>, 
        IAmStartedByMessages<OrderPlaced>, 
        IAmStartedByMessages<OrderBilled>,
        IHandleTimeouts<ShippingPolicy.OrderNotBilledTimeout>
    {
        public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            this.Data.Placed = true;
            log.Info($"Received OrderPlaced, OrderId={message.OrderId}, should we ship? (Placed={Data.Placed}, Billed={Data.Billed}");

            await ProcessOrder(context);

            if(!Data.Billed)
            {
                await this.RequestTimeout<OrderNotBilledTimeout> (context, TimeSpan.FromSeconds(5));
            }
        }

        public async Task Handle(OrderBilled message, IMessageHandlerContext context)
        {
            this.Data.Billed = true;
            log.Info($"Received OrderBilled, orderId={message.OrderId}, should we ship? (Placed={Data.Placed}, Billed={Data.Billed}");


            await ProcessOrder(context);
        }

        private async Task ProcessOrder(IMessageHandlerContext context)
        {
            if(Data.Placed && Data.Billed)
            {
                log.Info($"Order is placed and billed, time to ship!");
                MarkAsComplete();
            }
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
        {
            mapper.MapSaga(sagaData => sagaData.OrderId)
                .ToMessage<OrderPlaced>(msg => msg.OrderId)
                .ToMessage<OrderBilled>(msg => msg.OrderId);
        }

        public Task Timeout(OrderNotBilledTimeout state, IMessageHandlerContext context)
        {
            log.Info($"Uh-oh, order {Data.OrderId} has not been billed! Publishing OrderNotBilled.");
            return Task.CompletedTask;
        }

        static ILog log = LogManager.GetLogger<ShippingPolicy>();

        public class SagaData : ContainSagaData
        {
            public string OrderId { get; set; }
            public bool Placed { get; set; }
            public bool Billed { get; set; }

        }

        public class OrderNotBilledTimeout { }
    }
}
