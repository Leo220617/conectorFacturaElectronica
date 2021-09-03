namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ConexionSAP")]
    public partial class ConexionSAP
    {
        public int id { get; set; }

        [StringLength(50)]
        public string SQLUser { get; set; }

        [StringLength(50)]
        public string SQLServer { get; set; }

        [StringLength(60)]
        public string SQLPass { get; set; }

        [StringLength(50)]
        public string SQLBD { get; set; }
    }
}
