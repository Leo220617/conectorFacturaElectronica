 

namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CondicionesVenta")]
    public partial class CondicionesVenta
    {
        [Key]
        public string codSAP { get; set; }
        public string codCyber { get; set; }
        public string Nombre { get; set; }
    }
}