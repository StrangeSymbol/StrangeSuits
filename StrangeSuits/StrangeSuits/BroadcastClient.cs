using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace StrangeSuits
{
    enum NetGame : byte { SuitChanged, Draw, EndTurn, Back, Shuffle, Set}

    class Message
    {
        public IPAddress Address;
        public int Port;
        public byte[] Bytes;
    }

    class BroadcastClient
    {
        const int udpRangeStart = 15123;
        const int localMaximumPortCount = 16;
        UdpClient udpClient;
        IPEndPoint udpReceiveEndPoint;
        List<IPEndPoint> udpSendEndPoints;

        public int LocalPort;
        public bool IsListening = false;

        Queue<Message> messagesReceived = new Queue<Message>();

        public BroadcastClient()
        {
            BeginListening();
            SetupSendPorts();
        }

        //public void Send(byte[] data)
        //{
        //    foreach (var endpoint in udpSendEndPoints)
        //    {
        //        udpClient.BeginSend(data, data.Length, endpoint, UdpMessageSent, udpClient);
        //    }
        //}

        public void Send(params byte[] data)
        {
            foreach (IPEndPoint endpoint in udpSendEndPoints)
            {
                udpClient.BeginSend(data, data.Length, endpoint, UdpMessageSent, udpClient);
            }
        }

        private void UdpMessageSent(IAsyncResult asyncResult)
        {
        }

        private void SetupSendPorts()
        {
            udpSendEndPoints = new List<IPEndPoint>();
            for (int sendPortOffset = 0; sendPortOffset < localMaximumPortCount; sendPortOffset++)
            {
                udpSendEndPoints.Add(new IPEndPoint(IPAddress.Broadcast, udpRangeStart + sendPortOffset));
            }
        }

        private void BeginListening()
        {
            int portTestCount = 0;
            bool udpPortFound = false;
            do
            {
                try
                {
                    LocalPort = udpRangeStart + portTestCount;
                    udpReceiveEndPoint = new IPEndPoint(IPAddress.Any, LocalPort);
                    udpClient = new UdpClient(udpReceiveEndPoint);
                    udpPortFound = true;
                }
                catch (SocketException)
                {
                    portTestCount++;
                }

            } while (!udpPortFound && portTestCount < localMaximumPortCount);

            if (udpPortFound)
            {
                udpClient.BeginReceive(UdpMessageReceived, udpClient);
                IsListening = true;
            }
        }

        public bool HasMessage()
        {
            return IsListening && messagesReceived.Count > 0;
        }

        public Message ReceiveMessage()
        {
            if (HasMessage())
                return messagesReceived.Dequeue();
            else
                return null;
        }

        public Message PeekMessage()
        {
            if (HasMessage())
                return messagesReceived.Peek();
            else
                return null;
        }

        private void UdpMessageReceived(IAsyncResult asyncResult)
        {
            byte[] receivedBytes = udpClient.EndReceive(asyncResult, ref udpReceiveEndPoint);
            udpClient.BeginReceive(UdpMessageReceived, udpClient);
            if (udpReceiveEndPoint.Port != LocalPort)
            {
                messagesReceived.Enqueue(
                    new Message()
                    {
                        Address = udpReceiveEndPoint.Address,
                        Port = udpReceiveEndPoint.Port,
                        Bytes = receivedBytes
                    });
            }
        }

    }
}