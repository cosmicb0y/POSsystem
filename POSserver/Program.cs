using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

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
            try
            {
                //client의 접속이 올때까지 block되는 부분(Thread)
                //백그라운드 thread에 처리 맡김
                //clientSocket = tcpListener.AcceptSocket();

                //클라이언트의 데이터를 읽고, 쓰기 위한 스트림을 만든다
                stream = new NetworkStream(clientSocket);



                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                reader = new StreamReader(stream, encode);

                //getting restuarant_id
               
                
                Console.WriteLine("hello_client");
                
                while (true)
                {
                    str = reader.ReadLine();
                    if (str.IndexOf("<EOF>") > -1)
                    {
                        Console.WriteLine("Bye Bye");
                        break;
                    }
                    Console.WriteLine("data_get : " + str);
                    restaurant_name = dbmanager.GetRestaurantNameFromId(str);
                    menu_list = dbmanager.GetRestaurantMenu(restaurant_name);
                    //sending restuarant_menu
                    foreach (Menu m in menu_list)
                    {
                        string menu_data_json = JsonConvert.SerializeObject(m);
                        menu_data_json += "\r\n";
                        byte[] dataWrite = Encoding.Default.GetBytes(menu_data_json);
                        stream.Write(dataWrite, 0, dataWrite.Length);


                    }
                    string end_menu = "end_menu\r\n";
                    byte[] end_menu_dataWrite = Encoding.Default.GetBytes(end_menu);
                    stream.Write(end_menu_dataWrite, 0, end_menu_dataWrite.Length);
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
                IPAddress ipAd = IPAddress.Parse("127.0.0.1");

                //TcpListener class를 이용하여 클라이언트 연결 받아 들임
                tcpListener = new TcpListener(ipAd, 5001);
                tcpListener.Start();

                Console.WriteLine("멀티스레드 Test 창 :Waiting for connections...");

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