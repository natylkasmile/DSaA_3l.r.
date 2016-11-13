using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace RV_Server
{
    class Program
    {
        static int port = 8005;

        static void Main(string[] args)
        {
            ConcurrentBag<string> cb_speed = new ConcurrentBag<string>();
            ConcurrentBag<string> cb_count = new ConcurrentBag<string>();

            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                int endl = 29;
                while (endl>0)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);
                    String[] mas = builder.ToString().Split(' ');
                    cb_speed.Add(mas[0]);
                    cb_count.Add(mas[1]);
                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ":Сервер принимает: Скорость " + mas[0].ToString() +" Количество " + mas[1].ToString());

                    // отправляем ответ
                    string message = "ваше сообщение доставлено";
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    endl--;
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                Console.ReadLine();
                Console.WriteLine("Список средних скоростей:");
                foreach (string sp in cb_speed)
                    Console.Write(sp.ToString() + " ");
                Console.WriteLine();
                Console.WriteLine("Список Количесва машин:");
                foreach (string coun in cb_count)
                    Console.Write(coun.ToString() + " ");
                Console.Read();
            }
        }

    }
 }
