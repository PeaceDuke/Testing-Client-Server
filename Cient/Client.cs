using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.SQLite;
using System.Security.Cryptography;
using Messagess;

namespace Cient
{
    class Client
    {
        static bool testResult = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Идет подключение к сереру...");
            TcpClient testingTcpClient = null;
            do
            {
                //попытка установки соединения
                try
                {
                    testingTcpClient = new TcpClient("127.0.0.1", 3456);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Сервер не обнаружен!");
                }
            } while (testingTcpClient == null);
            Console.WriteLine("Соединение с сервером установлено!");
            Console.Write("Введите адрес до дериктории с файлами: ");
            var testFilesPath = "C:\\Users\\Professional\\Desktop\\Tests";
            //var testFilesPath = Console.ReadLine();
            ReadFiles(testingTcpClient, testFilesPath);
            if(testResult)
            {
                Console.WriteLine("Все файлы успешно прошли тестирование");
            }
            else
            {
                Console.WriteLine("Некоторые из файлов не прошли проверку");
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        //отправка сообщений на сервер
        private static void SendMessage(NetworkStream availableThreadStream, NetworkStream testingStream, TestMessage message)
        {
            var messageBuffer = ObjectToByteArray(message);
            //клиент сообщает о том, что у него есть файлы для тестирования
            availableThreadStream.Write(new byte[1], 0, 1);
            var responceBuffer = new byte[1024];
            //клиент ожидает, пока освободится какой-нибуь поток при этом получая результаты его работы
            var responceLenght = availableThreadStream.Read(responceBuffer, 0, responceBuffer.Length);
            //если поток закончил работу, он возвращает объект типа ResponceMessage
            if(responceLenght > 1)
            {
                var responce = (ResponseMessage)ByteArrayToObject(responceBuffer, responceLenght);
                Console.WriteLine("{0}: {1}", responce.FileName, responce.Response);
                if (!responce.Response)
                    testResult = false;
            }
            //клиент отправляет текст на сервер в виде объекта типа TestMessage
            testingStream.Write(messageBuffer, 0, messageBuffer.Length);
        }
        
        //чтение текста из файлов и инциализация 
        private static void ReadFiles(TcpClient testingTcpClient, string directoryPath)
        {
            var filePaths = Directory.GetFiles(directoryPath, "*.txt");
            var testingStream = testingTcpClient.GetStream();
            //для проверки доступных потоков создается еще одно подключение
            var availableThreadListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3457);
            availableThreadListener.Start();
            var availableThreadStream = availableThreadListener.AcceptTcpClient().GetStream();
            foreach (var filePath in filePaths)
            {
                var reader = new StreamReader(filePath);
                var text = reader.ReadToEnd();
                var message = new TestMessage(Path.GetFileName(filePath), text);
                SendMessage(availableThreadStream, testingStream, message);
            }
        }

        //метод сериализации
        private static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        //метод десириализации
        private static Object ByteArrayToObject(byte[] arrBytes, int lenght)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, lenght);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }
    }
}
