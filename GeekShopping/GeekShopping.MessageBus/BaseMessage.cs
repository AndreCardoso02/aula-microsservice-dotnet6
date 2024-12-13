using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShopping.MessageBus
{
    public class BaseMessage
    {
        public long Id { get; set; }
        public DateTime CreatedMessage { get; set; }
    }
}
