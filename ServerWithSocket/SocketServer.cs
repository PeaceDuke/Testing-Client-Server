using ServerWithSocket.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerWithQueue
{
    class QueueServer
    {   
        static void Main(string[] args)
        {
            Console.Write("Введите число потоков для обработки: ");
            int n = Int32.Parse(Console.ReadLine());
            if(n < 1 || n > 10)
            {
                Console.WriteLine("Число потоков должно быть в диапазоне от 1 до 10");
            }
            else
            {
                StartProcessData(n);    
            }
        }

        public static void StartProcessData(int n)
        {
            TcpListener testsListner = null; //подключение для отправки тестовых файло
            TcpListener availableThreadListner = null;  //подключение для сигнала от свободных п
            TcpListener responceListner = null; //подключнеие для ответов от потоков
            try
            {
                //создаются прослушивающие подключения
                testsListner = new TcpListener(IPAddress.Parse("127.0.0.1"), 3456);
                testsListner.Start();
                availableThreadListner = new TcpListener(IPAddress.Parse("127.0.0.1"), 3457);
                availableThreadListner.Start();
                responceListner = new TcpListener(IPAddress.Parse("127.0.0.1"), 3458);
                responceListner.Start();
                Console.WriteLine("Ожидание подключений...");
                //создаются подключения
                TcpClient testsTcpClient = testsListner.AcceptTcpClient();
                TcpClient availableThreadsClient = availableThreadListner.AcceptTcpClient();
                TcpClient responceClient = responceListner.AcceptTcpClient();
                //содается n потоков, каждый из которых обрабатывает сообщения
                for (int i = 0; i < n; i++)
                {
                    ProcessComponent processObject = new ProcessComponent(i, testsTcpClient, availableThreadsClient, responceClient);
                    Thread processThread = new Thread(new ThreadStart(processObject.Process));
                    //каждый поток запускается с небольшой заержкой для генерации рандомных чисел
                    Thread.Sleep(200);
                    processThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (testsListner != null)
                    testsListner.Stop();
                if (availableThreadListner != null)
                    availableThreadListner.Stop();
                if (responceListner != null)
                    responceListner.Stop();
            }
        }
    }
}
