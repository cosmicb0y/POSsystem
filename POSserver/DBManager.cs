using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace POSserver
{
    class DBManager
    {
        private static DBManager instance=null;
        private DBManager()
        {
            conn = new MySqlConnection(cn);
            conn.Open();
        }
        public static DBManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBManager();
                }
                return instance;
            }
        }
        private string cn = "Server=localhost;Database=posdb;Uid=lee;Pwd=1212;Charset=utf8";
        public MySqlConnection conn;
        public LinkedList<string> GetRestaurantName()
        {
            String query = "select restaurant_name from restaurant";
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlDataReader reader= command.ExecuteReader();
            LinkedList<string> list = new LinkedList<string>();
            while (reader.Read())//리더에 데이터가 있으면.
            {
                list.AddLast(reader["restaurant_name"].ToString()); 
            }
            reader.Close();
            return list;
        }
        public LinkedList<Menu> GetRestaurantMenu(string name)
        {
            String query = "select menu_name,menu_price from restaurant,menu where menu.restaurant_id=restaurant.restaurant_id and restaurant.restaurant_name='" + name + "'";
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlDataReader reader = command.ExecuteReader();
            LinkedList<Menu> list = new LinkedList<Menu>();
            while (reader.Read())//리더에 데이터가 있으면.
            {
                Menu menu = new Menu(reader["menu_name"].ToString(), reader["menu_price"].ToString());
                list.AddLast(menu);
            }
            reader.Close();
            return list;
        }
        public LinkedList<OrderItem> GetRestaurantOrderList(string name)
        {
            String query = "select order_.order_id,menu.menu_name,order_.num_of_order,order_.order_time,order_.payment_index from order_,menu,restaurant where order_.menu_id=menu.menu_id and restaurant.restaurant_id=order_.restaurant_id and restaurant.restaurant_name='" + name + "'";
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlDataReader reader = command.ExecuteReader();
            LinkedList<OrderItem> list = new LinkedList<OrderItem>();
            while (reader.Read())//리더에 데이터가 있으면.
            {
                OrderItem orderitem = new OrderItem(reader["order_id"].ToString(), reader["menu_name"].ToString(),
                     reader["num_of_order"].ToString(), reader["order_time"].ToString(), reader["payment_index"].ToString());
                list.AddLast(orderitem);
            }
            reader.Close();
            return list;
        }
    }
}
