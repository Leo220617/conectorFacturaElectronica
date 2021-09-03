 

namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cantones")]
    public partial class Cantones
    {
        [Key ] 
        public int CodProvincia { get; set; }
    
        public int CodCanton { get; set; }
        public string NomCanton { get; set; }
    }
}