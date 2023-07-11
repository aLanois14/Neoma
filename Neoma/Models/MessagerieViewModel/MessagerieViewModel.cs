using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.MessagerieViewModel
{
    public class MessagerieViewModel
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("conversation")]
        public int Conversation { get; set; }
    }
}
