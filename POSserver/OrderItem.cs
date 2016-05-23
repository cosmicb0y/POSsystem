using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSserver
{
    class OrderItem
    {
        public String order_id { get; set; }
        public String menu_name { get; set; }
        public String order_num { get; set; }
        public String order_time { get; set; }
        public String ispayment { get; set; }
        public OrderItem(String order_id, String menu_name, String order_num, String order_time, String ispayment)
        {
            this.order_id = order_id;
            this.menu_name = menu_name;
            this.order_num = order_num;
            this.order_time = order_time;
            if (ispayment.Equals("0"))
            {
                this.ispayment = "결제대기";
            }
            else if (ispayment.Equals("1"))
            {
                this.ispayment = "결제완료";
            }
            else if (ispayment.Equals("2"))
            {
                this.ispayment = "주문취소";
            }
        }
    }
}
