using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class OrderList
{
    public int flag = 1;
    public LinkedList<OrderMenu> menu_list = new LinkedList<OrderMenu>();
    public OrderList()   {   }
    public void addMenu(string name,int num)
    {
        OrderMenu ordermenu = new OrderMenu(name, num);
        menu_list.AddLast(ordermenu);
    }
}

