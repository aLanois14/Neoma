using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.MatchingViewModel
{
    public class ProjetModel
    {
        [JsonProperty("Role")]
        public int Projet { get; set; }
    }
}
