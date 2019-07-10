using System;

namespace NetworkCommon
{
    public enum MasterServerMessageType
    {
        RegisterHost,
        RequestHostList,
        RequestIntroduction,
        RemoveHost,
        HostConnected,
    }
}
