using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSserver
{
    class Order
    {
        public string orderNum { get; set; }
        public string name { get; set; }
        public string menuNum { get; set; }
        public Order(string orderNum, string name, string menuNum)
        {
            this.orderNum = orderNum;
            this.name = name;
            this.menuNum = menuNum;
        }
    }
}
