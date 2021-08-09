﻿ 

namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Inventario")]
    public partial class Inventario
    {
        public int id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsCode { get; set; }
        public decimal OnHand { get; set; }
        public decimal IsCommited { get; set; }
        public decimal Stock { get; set; }
        public string skuid { get; set; }
        public decimal Precio { get; set; }
        public string Currency { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaActPrec { get; set; }
    }
}