using System;
using System.Net;
using System.Collections.Generic;
using Lidgren.Network;

namespace StrangeSuits
{
    static class SSEngine
    {
        public static MenuStage VersionStage { set; get; }
        public static bool Disconnected { get; set; }
        public static bool Player1First { set; get; }
        public static bool? Player1 { get; set; }
        public static bool IsACardMoving { set; get; }
        public static float QuarterSceenWidth { set; get; }
        public static float QuarterSceenHeight { set; get; }
        public static bool? Player1SetThree { get; set; }
        public static BroadcastClient LANClient { get; set; }
        public static NetPeer Peer { get; set; }
        public const int MasterServerPort = 15002;
        public static IPEndPoint MasterServerEndpoint { get; set; }
        public static bool? IsHost { get; set; }
        public static Dictionary<string, CardSprite> DeckSprites { get; set; }
    }
}