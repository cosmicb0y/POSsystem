using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace POSserver
{
    public class ClientHandler
    {
        DBManager dbmanager = DBManager.Instance;
        public Socket clientSocket;
        public void runClient()
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            string str = null;
            string restaurant_name = null;
            LinkedList<Menu> menu_list = null;
            JObject jobj = null;
            string table_num = null;
            int order_num = 1;
            try
            {
                //client의 접속이 올때까지 block되는 부분(Thread)
                //백그라운드 thread에 처리 맡김
                //clientSocket = tcpListener.AcceptSocket();

                //클라이언트의 데이터를 읽고, 쓰기 위한 스트림을 만든다
                stream = new NetworkStream(clientSocket);



                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                reader = new StreamReader(stream, encode);

                //getting restuarant_id
               
                
                Console.WriteLine("hello_client");
                
                while (true)
                {
                    str = reader.ReadLine(); // 최조 접속 ex) {"restaurant_name":"김밥천국","restaurant_id":1,"table_num":4}
                    if (str.IndexOf("<EOF>") > -1)
                    {
                        Console.WriteLine("Bye Bye");
                        break;
                    }
                    jobj = JObject.Parse(str);
                    string restaurant_id = jobj["restaurant_id"].ToString();
                    string restaurant_name_from_client = jobj["restaurant_name"].ToString();
                    table_num = jobj["table_num"].ToString();
                    Console.WriteLine("restaurant_id : " + restaurant_id);
                    restaurant_name = dbmanager.GetRestaurantNameFromId(restaurant_id);
                    if(!restaurant_name.Equals(restaurant_name_from_client)) //클라이언트로부터 받은 식당이름과 데이터베이스의 식당 이름이 다를경우
                    {
                        Console.WriteLine("no match restaurant_name\n");
                        break;
                    }
                    menu_list = dbmanager.GetRestaurantMenu(restaurant_name);
                    //sending restuarant_menu
                    //foreach (Menu m in menu_list)
                    {
                        string menu_data_json = JsonConvert.SerializeObject(menu_list);
                        menu_data_json += "\r\n";
                        Console.WriteLine("menu_data_json : " + menu_data_json);
                        byte[] dataWrite = Encoding.UTF8.GetBytes(menu_data_json);
                        stream.Write(dataWrite, 0, dataWrite.Length);
                    }
                    string end_menu = "end_menu\r\n";
                    byte[] end_menu_dataWrite = Encoding.UTF8.GetBytes(end_menu);
                    stream.Write(end_menu_dataWrite, 0, end_menu_dataWrite.Length);

                    bool orderProcess = true;
                    while (orderProcess)
                    {
                        str = reader.ReadLine(); // menu function
                        if (str.IndexOf("<EOF>") > -1)
                        {
                            Console.WriteLine("Bye Bye");
                            break;
                        }
                        jobj = JObject.Parse(str);
                        string flagStr = jobj["flag"].ToString();
                        int flag = System.Convert.ToInt32(flagStr);

                        switch (flag)
                        {
                            case 1: // order menu
                                ArrayList menuName = new ArrayList();
                                ArrayList menuNum = new ArrayList();

                                Array menuList = jobj["menu_list"].ToArray();
                                
                                for (int i = 0; i < menuList.Length; i++)
                                {
                                    JObject menuJobj = JObject.Parse(menuList.GetValue(i).ToString());
                                    menuName.Add(menuJobj["name"].ToString());
                                    menuNum.Add(menuJobj["num"].ToString());
                                }

                                order_num = dbmanager.InsertMenuList(menuName, menuNum, System.Convert.ToInt32(restaurant_id), order_num, System.Convert.ToInt32(table_num));
                                break;
                            case 2: // payment
                                string orderNumberStr = jobj["orderNumber"].ToString();
                                int orderNumber = System.Convert.ToInt32(orderNumberStr);
                                dbmanager.payment(orderNumber);

                                orderProcess = false;

                                break;
                            case 3: // adjust menu
                                // need implement
                                break;
                            default: // error
                                string err = "Flag parse error\r\n";
                                byte[] err_dataWrite = Encoding.UTF8.GetBytes(err);
                                stream.Write(err_dataWrite, 0, err_dataWrite.Length);
                                orderProcess = false;
                                break;
                        }


                    } 

                }
                
            }
            catch (Exception e)
            {
                //   Console.WriteLine(e.ToString());
            }
            finally
            {
                clientSocket.Close();
            }
        }
    }

    public class EchoServer
    {
        DBManager dbmanager = DBManager.Instance;
        public static void Main(string[] args)
        {

            new EchoServer().ServerThread();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
          

        }
        private void ServerThread()
        {
            Thread thread = new Thread(new ThreadStart(Startserver));
            thread.Start();
            //thread.Join();
        }
        private void Startserver()
        {
            TcpListener tcpListener = null;
            try
            {
                //ip주소를 나타내는 객체 생성. TcpListener생성시 인자로 사용
                IPAddress ipAd = IPAddress.Parse("155.230.52.66");

                //TcpListener class를 이용하여 클라이언트 연결 받아 들임
                tcpListener = new TcpListener(ipAd, 5001);
                tcpListener.Start();

                Console.WriteLine("멀티스레드 Test 창 :Waiting for connections...");
                Console.WriteLine("ver 0.0.1");

                while (true)
                {
                    //accepting the connection
                    Socket client = tcpListener.AcceptSocket();


                    ClientHandler cHandler = new ClientHandler();
                    //passing calue to the thread class
                    cHandler.clientSocket = client;

                    //creating a new thread for the client
                    Thread clientThread = new Thread(new ThreadStart(cHandler.runClient));
                    clientThread.Start();
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception :" + exp);
            }
            finally
            {
                tcpListener.Stop();
            }
        }
    }
}