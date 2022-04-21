using System;
using System.Collections.Generic;
using System.Text;

namespace signalre.Entities
{
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


        public DateTime fechaActualizacion { get; set; }

    }
}
