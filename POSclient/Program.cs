using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

class POSclient
{
    static void Main(string[] args)
    {
        TcpClient client = null;

        try
        {
            //LocalHost에 지정포트로 TCP Connection생성 후 데이터 송수신 스트림 얻음
            client = new TcpClient();
            client.Connect("127.0.0.1", 5001);
            NetworkStream writeStream = client.GetStream();

            Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
            StreamReader readerStream = new StreamReader(writeStream, encode);
            while (true)
            {
                //("1 : 봉구스밥버거");
                //("2 : 김밥천국");
                //("3 : 봉봉치킨");
                //("4 : 맘스터치");
                Console.WriteLine("봉구스 밥버거에 오신 것을 환영합니다");
                //보낼 데이터를 읽어 Default형식의 바이트 스트림으로 변환

                byte[] data = Encoding.Default.GetBytes("1\r\n");
                writeStream.Write(data, 0, data.Length);
                while (true)
                {

                    // if (dataToSend.IndexOf("<EOF>") > -1)
                    //  break;

                    string menuData;
                    menuData = readerStream.ReadLine();
                    Console.WriteLine("JSON_DATA : " + menuData);
                    if (menuData.Equals("end_menu")) break;
                    Menu menu = JsonConvert.DeserializeObject<Menu>(menuData);
                    Console.WriteLine(menu.name + "\t\t|\t" + menu.price + "원");

                    //dataToSend = Console.ReadLine();
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            client.Close();
        }
    }
}
