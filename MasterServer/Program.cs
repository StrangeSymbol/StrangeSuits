﻿using System;
using System.Net;
using System.Collections.Generic;

using Lidgren.Network;
using NetworkCommon;

namespace MasterServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Dictionary<long, IPEndPoint[]> registeredHosts = new Dictionary<long, IPEndPoint[]>();

            NetPeerConfiguration config = new NetPeerConfiguration("master");
            config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 15002;
            
            NetPeer peer = new NetPeer(config);
            peer.Start();

            // keep going until ESCAPE is pressed
            Console.WriteLine("Press ESC to quit");
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage msg;
                while ((msg = peer.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            Console.WriteLine("Discover message from: " + msg.SenderEndPoint);
                            // Create a response and write some example data to it
                            NetOutgoingMessage response = peer.CreateMessage();
                            response.Write("master");

                            // Send the response to the sender of the request
                            peer.SendDiscoveryResponse(response, msg.SenderEndPoint);
                            break;
                        case NetIncomingMessageType.UnconnectedData:
                            //
                            // We've received a message from a client or a host
                            //

                            // by design, the first byte always indicates action
                            switch ((MasterServerMessageType)msg.ReadByte())
                            {
                                case MasterServerMessageType.RegisterHost:

                                    // It's a host wanting to register its presence
                                    var id = msg.ReadInt64(); // server unique identifier

                                    Console.WriteLine("Got registration for host " + id);
                                    registeredHosts[id] = new IPEndPoint[]
									{
										msg.ReadIPEndPoint(), // internal
										msg.SenderEndPoint // external
									};
                                    break;

                                case MasterServerMessageType.RequestHostList:
                                    // It's a client wanting a list of registered hosts
                                    Console.WriteLine("Sending list of " + registeredHosts.Count + " hosts to client " + msg.SenderEndPoint);
                                    // Clear original list.
                                    NetOutgoingMessage outMsg = peer.CreateMessage();
                                    outMsg.Write(true);
                                    peer.SendUnconnectedMessage(outMsg, msg.SenderEndPoint);
                                    foreach (var kvp in registeredHosts)
                                    {
                                        // send registered host to client
                                        NetOutgoingMessage om = peer.CreateMessage();
                                        om.Write(false);
                                        om.Write(kvp.Key);
                                        om.Write(kvp.Value[0]);
                                        om.Write(kvp.Value[1]);
                                        peer.SendUnconnectedMessage(om, msg.SenderEndPoint);
                                    }

                                    break;
                                case MasterServerMessageType.RequestIntroduction:
                                    // It's a client wanting to connect to a specific (external) host
                                    IPEndPoint clientInternal = msg.ReadIPEndPoint();
                                    long hostId = msg.ReadInt64();
                                    string token = msg.ReadString();

                                    Console.WriteLine(msg.SenderEndPoint + " requesting introduction to " + hostId + " (token " + token + ")");

                                    // find in list
                                    IPEndPoint[] elist;
                                    if (registeredHosts.TryGetValue(hostId, out elist))
                                    {
                                        // found in list - introduce client and host to eachother
                                        Console.WriteLine("Sending introduction...");

                                        peer.Introduce(
                                            elist[0], // host internal
                                            elist[1], // host external
                                            clientInternal, // client internal
                                            msg.SenderEndPoint, // client external
                                            token // request token
                                        );
                                    }
                                    else
                                    {
                                        Console.WriteLine("Client requested introduction to nonlisted host!");
                                    }
                                    break;
                                case MasterServerMessageType.RemoveHost:
                                    long idRemove = msg.ReadInt64();
                                    Console.WriteLine("De-register host " + idRemove);
                                    registeredHosts.Remove(idRemove);
                                    break;
                                case MasterServerMessageType.HostConnected:
                                    long host = msg.ReadInt64();
                                    Console.WriteLine("host: " + host + " is connected.");
                                    registeredHosts.Remove(host); // Remove this host.
                                    break;
                            }
                            break;

                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            // print diagnostics message
                            Console.WriteLine(msg.ReadString());
                            break;
                    }
                }
            }

            peer.Shutdown("shutting down");
        }
    }
}