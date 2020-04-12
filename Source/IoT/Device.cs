using System.Collections.Generic;
using System.Net.WebSockets;

namespace SkyCloud.IoT
{
    public class Device
    {
        private Queue<string> messages;

        public WebSocket Socket { get; set; }
    }
}