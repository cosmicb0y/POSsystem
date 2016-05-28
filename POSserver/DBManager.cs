using System;
using System.Collections.Generic;
using System.Collections;
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
        public String GetRestaurantNameFromId(string id)
        {
            String query = "select restaurant_name from restaurant where restaurant_id='" + id + "'";
            MySqlCommand command = new MySqlCommand(query, conn);
            MySqlDataReader reader = command.ExecuteReader();
            string restaurant_name=null;
            while (reader.Read())//리더에 데이터가 있으면.
            {
                restaurant_name=(reader["restaurant_name"].ToString());
            }
            reader.Close();
            return restaurant_name;
        }
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
        public LinkedList<Order> InsertMenuList(ArrayList menuName, ArrayList menuNum, int restaurant_id, ref int order_id, int table_num)
        {
            LinkedList < Order > list = new LinkedList<Order>();
            for (int i = 0; i < menuName.Count; i++)
            {
                String query = "select menu_id from menu where menu.menu_name='" + menuName[i].ToString() + "'";
                MySqlCommand command = new MySqlCommand(query, conn);
                MySqlDataReader reader = command.ExecuteReader();
                int menu_id = -1;
                while (reader.Read())//리더에 데이터가 있으면.
                {
                     menu_id = System.Convert.ToInt32(reader["menu_id"].ToString());

                }
                reader.Close();

                query = "insert into order_(order_id, restaurant_id, order_time, menu_id, num_of_order, payment_index, table_num) values (@order_id, @restaurant_id, @order_time, @menu_id, @num_of_order, @payment_index, @table_num)";
                command = new MySqlCommand(query, conn);

                order_id++;
                command.Parameters.Add("@order_id", MySqlDbType.Int32, order_id);
                command.Parameters.Add("@restaurant_id", MySqlDbType.Int32, restaurant_id);
                command.Parameters.AddWithValue("@order_time", DateTime.Now);
                command.Parameters.Add("@menu_id", MySqlDbType.Int32, menu_id);
                command.Parameters.Add("@num_of_order", MySqlDbType.Int32, System.Convert.ToInt32(menuNum[i].ToString()));
                command.Parameters.Add("@payment_index", MySqlDbType.Int32, 0);
                command.Parameters.Add("@talbe_num", MySqlDbType.Int32, table_num);

                Order order = new Order(order_id.ToString(), menuName[i].ToString(), menuNum[i].ToString());
                list.AddLast(order);

                command.ExecuteNonQuery();
            }

            return list;
        }

        public void payment(int order_id)
        {
            String query = "update order_ set payment_inex=@payment_index where order_id=@order_id";
            MySqlCommand command = new MySqlCommand(query, conn);

            command.Parameters.Add("@payment_index", MySqlDbType.Int32, 1);
            command.Parameters.Add("@order_id", MySqlDbType.Int32, order_id);

            command.ExecuteNonQuery();
        }
    }
}
