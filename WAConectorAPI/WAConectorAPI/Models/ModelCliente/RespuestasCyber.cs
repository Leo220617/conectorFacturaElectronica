 
namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RespuestasCyber")]
    public partial class RespuestasCyber
    {
        public int id { get; set; }
        public string Detalle { get; set; }
    }
}