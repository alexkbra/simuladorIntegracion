using System;
using System.Collections.Generic;
using System.Text;

namespace signalre.Entities
{
    public class HistoricoCaogulante
    {
        public decimal recomendado { get; set; }
        public AguaNatural AguaNatural { get; set; } = new AguaNatural();
        public AguaPotable AguaPotable { get; set; } = new AguaPotable();
    }
}
