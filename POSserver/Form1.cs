using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace POSserver
{
    public partial class Form1 : Form
    {
        DBManager dbmanager = DBManager.Instance;
        String restaurant_id="1"; //초기값은 1
        public Form1()
        {
            InitializeComponent();
        }
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //updating dataGridView when you select restaurant_name.
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            String restaurant_name = listBox1.SelectedItem.ToString();
            LinkedList<Menu> menulist = dbmanager.GetRestaurantMenu(restaurant_name);

            foreach (Menu m in menulist)
            {
                string[] row = { m.name, m.price };
                dataGridView1.Rows.Add(row);
            }

            //주문목록 불러오기
            LinkedList<OrderItem> order_list = dbmanager.GetRestaurantOrderList(restaurant_name);
            foreach (OrderItem m in order_list)
            {
                string[] row = { m.order_id,m.menu_name, m.order_num,m.order_time,m.ispayment };
                dataGridView2.Rows.Add(row);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Visible = false;
            
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: 이 코드는 데이터를 'posdbDataSet1.order' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
            //this.orderTableAdapter1.Fill(this.posdbDataSet1.order);
            LinkedList<string> list = dbmanager.GetRestaurantName();
            foreach(string m in list)
            {
                listBox1.Items.Add(m);
            }
            listBox1.SelectedIndex = 0;//초기값은 첫번째 선택
            radioButton1.Select(); //메뉴버튼선택
        }
    }
}
