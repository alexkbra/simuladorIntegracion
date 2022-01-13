using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

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

    public class Siata
    {
        public Sensores[] sensores{ get; set; }
    }

    public class Sensores
    {   
        public string id { get; set; }
        public string nombre { get; set; }
        public bool estado { get; set; }
        public string ubicacion { get; set; }
        public int idPrecipitacion { get; set; }
        public int precipitacion { get; set; }
        public DateTime precipitaciontime { get; set; }
        public int idTemperatura { get; set; }
        public int temperatura { get; set; }
        public DateTime temperaturatime { get; set; }
        public int idHumedad { get; set; }
        public int humedad { get; set; }
        public DateTime humedadtime { get; set; }
        public DateTime fechaActualizacion { get; set; }
    }

    public class AguaNatural
    {
        public int turbiedadentrada { get; set; }
        public DateTime turbiedadentradatime { get; set; }
        public int caudalentrada { get; set; }
        public DateTime caudalentradatime { get; set; }

        public int conductividad { get; set; }
        public DateTime conductividadtime { get; set; }

        public int ph { get; set; }
        public DateTime phtime { get; set; }

        public int presion { get; set; }
        public DateTime presiontime { get; set; }

        public int color { get; set; }
        public DateTime colortime { get; set; }

    }

    public class AguaPotable
    {
        public int turbiedadsalida { get; set; }
        public DateTime turbiedadsalidatime { get; set; }
        public int caudalsalida { get; set; }
        public DateTime caudalsalidatime { get; set; }

        public int niveltanques { get; set; }
        public DateTime niveltanquestime { get; set; }

        public int color { get; set; }
        public DateTime colortime { get; set; }
       
    }

    public class NivelesCoagulacion
    {
        public int actual { get; set; }
        public DateTime actualtime { get; set; }
        public int recomendado { get; set; }
        public DateTime recomendadotime { get; set; }

    }

    
}