using Messagess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Cient
{
    class Client
    {
        static bool testResult = true;
        static List<Task> responseTasks = new List<Task>();

        static void Main(string[] args)
        {
            Console.WriteLine("Идет подключение к сереру...");
            TcpClient testingTcpClient = null;
            TcpClient availableThreadClient = null;
            do
            {
                //попытка установки соединения
                try
                {
                    testingTcpClient = new TcpClient("127.0.0.1", 3456);
                    availableThreadClient = new TcpClient("127.0.0.1", 3457);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Сервер не обнаружен!");
                }
            } while (testingTcpClient == null);
            Console.WriteLine("Соединение с сервером установлено!");
            Console.Write("Введите адрес до дериктории с файлами: ");
            var testFilesPath = Console.ReadLine();
            ReadFiles(testingTcpClient, availableThreadClient, testFilesPath);
            WaitTask();
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

        //проверка, что все ответы получены
        private static void WaitTask()
        {
            foreach(var task in responseTasks)
            {
                task.Wait();
            }
        }

        //чтение оветов из потока
        private static void ReadResponseStream(NetworkStream responseStream)
        {
            var responceBuffer = new byte[1024];
            var buffSize = responseStream.Read(responceBuffer, 0, responceBuffer.Length);
            var responce = (ResponseMessage)ByteArrayToObject(responceBuffer, buffSize);
            Console.WriteLine("{0}: {1}", responce.FileName, responce.Response);
            if(!responce.Response)
            {
                testResult = false;
            }
        }

        //задания для получения ответов от сервера
        private static void GetResponse(NetworkStream responseStream)
        {
            Task responseTask = new Task(() => ReadResponseStream(responseStream));
            responseTasks.Add(responseTask);
            responseTask.Start();
        }

        
        //отправка сообщений на сервер
        private static void SendMessage(NetworkStream availableThreadStream, NetworkStream testingStream, TestMessage message)
        {
            var messageBuffer = ObjectToByteArray(message);
            //клиент отправляет текст на сервер в виде объекта типа TestMessage
            testingStream.Write(messageBuffer, 0, messageBuffer.Length);
            availableThreadStream.Read(new byte[1], 0, 1);
        }

        //чтение текста из файлов и инциализация 
        private static void ReadFiles(TcpClient testingTcpClient, TcpClient availableThreadClient, string directoryPath)
        {
            var filePaths = Directory.GetFiles(directoryPath, "*.txt");
            if (filePaths.Length > 127)
            {
                Console.WriteLine("В выбранной дериктории слишком много файлов, установленно базовое ограниечение в 127");
            }
            else
            {
                var testingStream = testingTcpClient.GetStream();
                var availableThreadStream = availableThreadClient.GetStream();
                var responseStream = new TcpClient("127.0.0.1", 3458).GetStream();
                //для проверки доступных потоков создается еще одно подключение
                foreach (var filePath in filePaths)
                {
                    var reader = new StreamReader(filePath);
                    var text = reader.ReadToEnd();
                    var message = new TestMessage(Path.GetFileName(filePath), text);
                    SendMessage(availableThreadStream, testingStream, message);
                    GetResponse(responseStream);
                }
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
