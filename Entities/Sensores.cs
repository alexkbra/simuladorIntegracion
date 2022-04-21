using System;
using System.Collections.Generic;
using System.Text;

namespace signalre.Entities
{
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
}
