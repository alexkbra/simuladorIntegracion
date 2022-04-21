using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace signalre.Entities
{
    public class Plantas
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Url { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Barrio { get; set; }
        public string Municipio { get; set; }

    }
}
