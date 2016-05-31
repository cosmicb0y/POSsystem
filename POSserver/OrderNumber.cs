using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSserver
{
    class OrderNumber
    {
        public string orderNumber { get; set; }
        public OrderNumber(string orderNumber)
        {
            this.orderNumber = orderNumber;
        }
    }
}
