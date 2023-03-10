namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EncDocumento")]
    public partial class EncDocumento
    {
        public int id { get; set; }

        [Required]
        [StringLength(3)]
        public string idSucursal { get; set; }

        [Required]
        public string consecutivoSAP { get; set; }

        public int? consecutivoInterno { get; set; }

        [StringLength(2)]
        public string TipoDocumento { get; set; }

        public string DocEntry { get; set; }
        public DateTime? Fecha { get; set; }

        public string CodActividadEconomica { get; set; }
        [StringLength(20)]
        public string CardCode { get; set; }
        [StringLength(100)]
        public string CardName { get; set; }
        [StringLength(20)]
        public string LicTradNum { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(2)]
        public string TipoIdentificacion { get; set; }

        [StringLength(2)]
        public string condicionVenta { get; set; }

        public int? plazoCredito { get; set; }

        [StringLength(15)]
        public string medioPago { get; set; }

        [Column(TypeName = "money")]
        public decimal? montoOtrosCargos { get; set; }

        [StringLength(3)]
        public string moneda { get; set; }

        [Column(TypeName = "money")]
        public decimal? tipoCambio { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalserviciogravado { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalservicioexento { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalservicioexonerado { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalmercaderiagravado { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalmercaderiaexonerado { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalmercaderiaexenta { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalgravado { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalexento { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalexonerado { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalventa { get; set; }

        [Column(TypeName = "money")]
        public decimal? totaldescuentos { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalventaneta { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalimpuestos { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalivadevuelto { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalotroscargos { get; set; }

        [Column(TypeName = "money")]
        public decimal? totalcomprobante { get; set; }

        public string RefTipoDocumento { get; set; }

        public string RefNumeroDocumento { get; set; }

        public DateTime RefFechaEmision { get; set; }

        public string RefCodigo { get; set; }

        public string RefRazon { get; set; }

        public bool procesadaHacienda { get; set; }

        public string RespuestaHacienda { get; set; }

        public string XMLFirmado { get; set; }
        public string ClaveHacienda { get; set; }
        public string ConsecutivoHacienda { get; set; }
        public string ErrorCyber { get; set; }
        public int code { get; set; }
        public string JSON { get; set; }
        public bool sincronizadaSAP { get; set; }
        public string Comentarios { get; set; }
    }
}
