using System;
using System.Collections.Generic;
using System.Text;

namespace signalre.Entities
{
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

        public DateTime fechaActualizacion { get; set; }

    }
}
