﻿using Newtonsoft.Json;

namespace Sockets
{
    public class Armlet
    {
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        [JsonProperty("s")]
        public int S { get; set; }
    }
}