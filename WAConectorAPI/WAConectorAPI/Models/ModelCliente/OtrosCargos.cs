namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OtrosCargos
    {
        public int id { get; set; }

        public int? idEncabezado { get; set; }

        [StringLength(2)]
        public string tipoDocumento { get; set; }

        [StringLength(160)]
        public string detalle { get; set; }

        public decimal? porcentaje { get; set; }
        public decimal monto { get; set; }
    }
}
