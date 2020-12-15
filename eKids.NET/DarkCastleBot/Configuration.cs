using System;
using System.Collections.Generic;
using System.Text;

namespace DarkCastleBot
{
    public static class Configuration
    {
        public readonly static string BotToken = "1458903805:AAGHWKlxrudPlNjBgco8jx-YjVXqxMr8nLk";

#if USE_PROXY
        public static class Proxy
        {
            public readonly static string Host = "{PROXY_ADDRESS}";
            public readonly static int Port = 8080;
        }
#endif
    }
}
