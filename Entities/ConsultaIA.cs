using System;
using System.Collections.Generic;
using System.Text;

namespace signalre.Entities
{
    public class ConsultaIA
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public int Hora { get; set; }
        public decimal Turbieda { get; set; }
        public decimal Conductividad { get; set; }
        public decimal Ph { get; set; }
        public decimal Color { get; set; }
        public decimal Caudal { get; set; }
    }
}
