namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Impuestos
    {
        [StringLength(10)]
        public string id { get; set; }

        [StringLength(2)]
        public string codigo { get; set; }

        [StringLength(2)]
        public string codigoTarifa { get; set; }

        public decimal? tarifa { get; set; }

        public decimal? factorIVA { get; set; }

        public decimal? exportacion { get; set; }
    }
}
