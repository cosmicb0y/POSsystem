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

namespace POSserver
{
    public class ClientHandler
    {
        public Socket clientSocket;
        public void runClient()
        {
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                //client의 접속이 올때까지 block되는 부분(Thread)
                //백그라운드 thread에 처리 맡김
                //clientSocket = tcpListener.AcceptSocket();

                //클라이언트의 데이터를 읽고, 쓰기 위한 스트림을 만든다
                stream = new NetworkStream(clientSocket);



                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                reader = new StreamReader(stream, encode);

                while (true)
                {
                    string str = reader.ReadLine();
                    if (str.IndexOf("<EOF>") > -1)
                    {
                        Console.WriteLine("Bye Bye");
                        break;
                    }
                    Console.WriteLine(str);
                    str += "\r\n";

                    byte[] dataWrite = Encoding.Default.GetBytes(str);

                    stream.Write(dataWrite, 0, dataWrite.Length);
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
        public static string cn = "Server=localhost;Database=posdb;Uid=lee;Pwd=1212;Charset=utf8";
        public static MySqlConnection conn;
        public static void Main(string[] args)
        {
            new EchoServer().ServerThread();
            conn = new MySqlConnection(cn);
            conn.Open();
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