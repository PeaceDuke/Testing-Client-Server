using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Messagess;

namespace ServerWithSocket.Models
{
    public class ProcessComponent
    {
        public TcpClient testsTcpclient;
        public TcpClient availableClient;
        public int numberOfThread;
        public ProcessComponent(int n, TcpClient tcpClient, TcpClient _availableClient)
        {
            testsTcpclient = tcpClient;
            numberOfThread = n;
            availableClient = _availableClient;
        }

        public void Process()
        {
            NetworkStream testsStream = null;
            var availableThreadStream = availableClient.GetStream();
            try
            {
                testsStream = testsTcpclient.GetStream();
                while (testsTcpclient.Connected)
                {
                    byte[] data = new byte[2048];
                    byte[] lastResponse = new byte[1];
                    int bytes = 0;
                    //поток ожидает запроса от клинета, о необходимости обработать данные
                    availableThreadStream.Read(new byte[1], 0, 1);
                    //поток сообщает о том, что он готов выполнять тестирования
                    availableThreadStream.Write(lastResponse, 0, lastResponse.Length);
                    do
                    {
                        //поток считывает текст для тестирования
                        bytes = testsStream.Read(data, 0, data.Length);
                    }
                    while (testsStream.DataAvailable);
                    if (bytes != 0)
                    {
                        Console.WriteLine("Пооток {0} прочитал данные длинной {1}", numberOfThread, bytes);
                        
                        var message = (TestMessage)ByteArrayToObject(data, bytes);
                        var response = new ResponseMessage(message.FileName, true);
                        var n = message.FileText.Length;
                        for (int i = 0; i < n / 2; i++)
                        {
                            if (message.FileText[i] != message.FileText[n - i - 1])
                            {
                                response.Response = false;
                                break;
                            }
                        }
                        //ожидание для имитации высокой нагрузки
                        Random random = new Random();
                        Thread.Sleep(random.Next(1000,5000));
                        Console.WriteLine("{0}: {1}", response.FileName, response.Response);
                        //сохраняем результат для дальнейшей отправки
                        lastResponse = ObjectToByteArray(response);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (testsStream != null)
                    testsStream.Close();
                if (testsTcpclient != null)
                    testsTcpclient.Close();
            }
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
    }
}
