using Newtonsoft.Json;
using OtoRobotWeb2.Models.Response;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace  OtoRobotWeb2.Helpers
{
    public class SocketScreen
    {
        private Socket ClientSocket;
        private const int offerProcessPort = 3737;

        private bool ConnectToServer(bool isLoginProcess = false)
        {
            ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int attempts = 0, currentPort = offerProcessPort;
            bool result = false;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    //ClientSocket.Connect(ipadress, currentPort);
                    ClientSocket.Connect(IPAddress.Loopback, currentPort);
                    result = true;
                    //Console.WriteLine("Connected");
                }
                catch (SocketException ex)
                {
                    result = false;
                }
                Thread.Sleep(1000);
                if (attempts == 15)
                {
                    break;
                }
            }


            return result;
        }

        public void SendRequest(string Message)
        {
            if (ConnectToServer())
            {
                SendString(Message);
                ReceiveResponse();
                Exit();
            }
        }
        public string SendRequestOffer(string Message)
        {
            //SocketResponse socketResponse = null;
            //if (ConnectToServer())
            //{
            //string request = Message;
            //SendString(request);
            //var sonuc = ReceiveResponse();
            //try
            //{
            //    if (!string.IsNullOrEmpty(sonuc))
            //        socketResponse = JsonConvert.DeserializeObject<SocketResponse>(sonuc);
            //}
            //catch (Exception)
            //{
            //}
            //Exit();
            //}
            //return socketResponse;
            string sonucRe = null;
            if (ConnectToServer())
            {
                string request = Message;
                SendString(request);
                var sonuc = ReceiveResponse();
                try
                {
                    if (!string.IsNullOrEmpty(sonuc))
                        sonucRe = sonuc;
                }
                catch (Exception)
                {
                }
                Exit();
            }
            return sonucRe;
        }

        private string ReceiveResponse()
        {
            try
            {
                var buffer = new byte[9999999];
                //int received = ClientSocket.Receive(buffer, SocketFlags.None);
                var tempreceived = ClientSocket.ReceiveAsync(buffer, SocketFlags.None);
                tempreceived.Wait();
                int received = tempreceived.Result;
                if (received == 0) return "";
                var data = new byte[received];
                Array.Copy(buffer, data, received);

                return Encoding.UTF8.GetString(data);
            }
            catch (Exception)
            {
                return "";
            }
        }
        private void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            try
            {
                ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

            }
            catch (Exception)
            {
            }
        }

        private void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
        }
    }
}
