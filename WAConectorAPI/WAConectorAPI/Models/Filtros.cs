using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WAConectorAPI.Models
{
    public class Filtros
    {
        public string Texto { get; set; }
        public int Codigo1 { get; set; }
        public string Estado { get; set; }
        public string CodMoneda { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
    }
}