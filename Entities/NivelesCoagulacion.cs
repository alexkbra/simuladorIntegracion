using System;
using System.Collections.Generic;
using System.Text;

namespace signalre.Entities
{
    public class NivelesCoagulacion
    {
        public int actual { get; set; }
        public DateTime actualtime { get; set; }
        public decimal recomendado { get; set; }
        public DateTime recomendadotime { get; set; }
        public DateTime fechaActualizacion { get; set; }
    }
}
