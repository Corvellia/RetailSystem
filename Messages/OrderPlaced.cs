using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class OrderPlaced : IEvent
    {
        public string OrderId { get; set; }
    }
}
