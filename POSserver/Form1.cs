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
       
        String restaurant_id="1"; //초기값은 1
        public Form1()
        {
            InitializeComponent();
            
        }
        private MySqlDataReader get_Reader_from_query(String query)
        {
            MySqlCommand command = new MySqlCommand(query, EchoServer.conn);
            return command.ExecuteReader();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            String str = listBox1.SelectedItem.ToString();
            MySqlDataReader reader = get_Reader_from_query("select restaurant_id from restaurant where restaurant_name='" + str + "'");

            while (reader.Read())//리더에 데이터가 있으면.
            {
                restaurant_id=reader[0].ToString();
            }
            reader.Close();
            //메뉴불러오기
            reader = get_Reader_from_query("select * from menu where restaurant_id='"+ restaurant_id + "'");
            while (reader.Read())//리더에 데이터가 있으면.
            {
                string[] row = { reader[2].ToString(), reader[3].ToString() };
                dataGridView1.Rows.Add(row);
            }
            reader.Close();
            //주문목록 불러오기

            reader = get_Reader_from_query("select * from order_ where restaurant_id='" + restaurant_id + "'");
            String[,] row_arr = new String[reader.FieldCount + 1, 5];
            int i = 0;
            while (reader.Read())//리더에 데이터가 있으면.
            {
                String payment_index = reader[5].ToString();

                //0 : 결제대기 1: 결제완료 2:주문취소
                if (payment_index.Equals("0"))
                {
                    payment_index = "결제대기";
                }
                else if (payment_index.Equals("1"))
                {
                    payment_index = "결제완료";
                }
                else if (payment_index.Equals("2"))
                {
                    payment_index = "주문취소";
                }
                row_arr[i, 0] = reader[0].ToString();
                row_arr[i, 1] = reader[2].ToString();
                row_arr[i, 2] = reader[4].ToString();
                row_arr[i, 3] = reader[3].ToString();
                row_arr[i, 4] = payment_index;
                i++;

            }
            reader.Close();
            for (int j = 0; j < i - 1; j++)
            {
                reader = get_Reader_from_query("select menu_name from menu where menu_id='" + row_arr[j, 1] + "'");
                while (reader.Read())//리더에 데이터가 있으면.
                {
                    row_arr[j, 1] = reader[0].ToString();
                }
                reader.Close();

                string[] row_str = { row_arr[j, 0], row_arr[j, 1], row_arr[j, 2], row_arr[j, 3], row_arr[j, 4] };
                dataGridView2.Rows.Add(row_str);
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

            MySqlDataReader reader = get_Reader_from_query("select restaurant_name from restaurant");

            while (reader.Read())//리더에 데이터가 있으면.
            {
                listBox1.Items.Add(reader[0].ToString());
            }
            reader.Close();
            listBox1.SelectedIndex = 0;//초기값은 첫번째 선택
            radioButton1.Select(); //메뉴버튼선택
        }
    }
}
