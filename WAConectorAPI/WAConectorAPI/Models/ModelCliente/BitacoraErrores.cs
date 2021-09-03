 

namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BitacoraErrores")]
    public partial class BitacoraErrores
    {
        [Key]
        public int id { get; set; }
        public string DocNum { get; set; }
        public string Type { get; set; }
        public string Descripcion { get; set; }
        public string StackTrace { get; set; }
        public DateTime Fecha { get; set; }
    }
}