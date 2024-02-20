using System.Net.Sockets;
using System.Text;
using System;
using System.Net;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using Newtonsoft.Json;
using OtoRobotWeb2.Models.Response;
using OtoRobotWeb2.Controllers;

namespace OtoRobotWeb2.Helpers
{
    public class SocketProcess
    {
        private Socket ClientSocket;
        int offerProcessPort = 6160;
        //string ipadress = "192.168.1.103";
        //string ipadress = "88.247.127.91";
        string ipadress = "213.254.135.175";

        int _UserId = 0;
        public SocketProcess(int userid)
        {
            _UserId = userid;
        }
        private bool ConnectToServer()
        {
            ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int saniye = 0, currentPort = offerProcessPort;
            currentPort += _UserId;
            bool result = false;
             
            while (!ClientSocket.Connected)
            {
                try
                {
                    if (RequestController.localdsocketprocescalistir)
                        ClientSocket.Connect(IPAddress.Loopback, currentPort);
                    else
                        ClientSocket.Connect(ipadress, currentPort);
                    result = true;
                }
                catch (SocketException ex)
                {
                    result = false;
                }
                Thread.Sleep(1000);
                if (saniye == 15)
                {
                    break;
                }
                saniye++;
            }
            return result;
        }
        private void Exit()
        {
            SendString("exit"); // Tell the server we are exiting
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
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
        public SocketResponse SendRequestOffer(string Message)
        {
            SocketResponse socketResponse = null;
            if (ConnectToServer())
            {
                string request = Message;
                SendString(request);
                var sonuc = ReceiveResponse();
                try
                {
                    if (!string.IsNullOrEmpty(sonuc))
                        socketResponse = JsonConvert.DeserializeObject<SocketResponse>(sonuc);
                }
                catch (Exception)
                {
                }
                Exit();
            }
            return socketResponse;
        }

        private void SendString(string text)
        { 
            byte[] buffer = Encoding.UTF8.GetBytes(text);

            try
            {
                ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

            }
            catch (Exception)
            {
            }
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

                //return Encoding.ASCII.GetString(data);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }

}
