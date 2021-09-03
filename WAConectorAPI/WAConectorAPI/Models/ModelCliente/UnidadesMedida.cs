 

namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("UnidadesMedida")]
    public partial class UnidadesMedida
    {
        [Key]
        [StringLength(7)]
        public string codSAP { get; set; }

        [StringLength(4)]
        public string codCyber { get; set; }

        [StringLength(50)]
        public string Nombre { get; set; }
    }
}