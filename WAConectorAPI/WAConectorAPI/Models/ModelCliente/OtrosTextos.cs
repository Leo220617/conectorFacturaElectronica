namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OtrosTextos
    {
        public int id { get; set; }

        public int? idEncabezado { get; set; }

        [StringLength(2)]
        public string codigo { get; set; }

      
        public string detalle { get; set; }
    }
}
