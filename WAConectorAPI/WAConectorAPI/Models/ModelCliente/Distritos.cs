 

namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Distritos")]
    public partial class Distritos
    {
        [Key]
        public int CodProvincia { get; set; }
        public int CodCanton { get; set; }
        public int CodDistrito { get; set; }
        public string NomDistrito { get; set; }
    }
}