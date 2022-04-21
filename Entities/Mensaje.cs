using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using signalre.Entities;

namespace Company.Function
{
    public class Mensaje
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public Siata siata { get; set; }
        public AguaNatural aguaNatural { get; set; }
        public AguaPotable aguaPotable { get; set; }
        public NivelesCoagulacion nivelesCoagulacion { get; set; }

        [JsonProperty(PropertyName = "partition")]
        public string Partition { get; set; }
        public bool IsRegistered { get; set; }
        // The ToString() method is used to format the output, it's used for demo purpose only. It's not required by Azure Cosmos DB
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new IsoDateTimeConverter() { DateTimeFormat = "MM/dd/yy HH:mm:ss" });
        }
    }

}