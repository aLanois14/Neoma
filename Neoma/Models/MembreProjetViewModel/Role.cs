using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Models.MembreProjetViewModel
{
    public class RoleModel
    {
        [JsonProperty("Role")]
        public int Role { get; set; }

        [JsonProperty("Projet")]
        public int Projet { get; set; }
    }
}
