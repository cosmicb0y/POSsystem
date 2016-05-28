using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSserver
{
    class Order
    {
        public string orderNumber { get; set; }
        public string menuName { get; set; }
        public string menuNum { get; set; }
        public Order(string orderNumber, string menuName, string menuNum)
        {
            this.orderNumber = orderNumber;
            this.menuName = menuName;
            this.menuNum = menuNum;
        }
    }
}
