 
namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("BandejaEntrada")]
    public partial class BandejaEntrada
    {
        public int Id { get; set; }

        public byte[] XmlFactura { get; set; }

        public string XmlConfirmacion { get; set; }

        public byte[] Pdf { get; set; }

        public DateTime FechaIngreso { get; set; }

        [StringLength(1)]
        public string Procesado { get; set; }

        public DateTime? FechaProcesado { get; set; }

        public string Mensaje { get; set; }

        public string Asunto { get; set; }

        public string Remitente { get; set; }

        [StringLength(100)]
        public string NumeroConsecutivo { get; set; }

        [StringLength(20)]
        public string TipoDocumento { get; set; }

        [StringLength(20)]
        public string FechaEmision { get; set; }

        [StringLength(200)]
        public string NombreEmisor { get; set; }

        [StringLength(100)]
        public string IdEmisor { get; set; }

        [StringLength(20)]
        public string CodigoMoneda { get; set; }

        [Column(TypeName = "money")]
        public decimal? TotalComprobante { get; set; }
        public string tipo { get; set; }
        public decimal Impuesto { get; set; }
        public string DetalleMensaje { get; set; }
        public string CodigoActividad { get; set; }
        public string CondicionImpuesto { get; set; }
        public decimal impuestoAcreditar { get; set; }
        public decimal gastoAplicable { get; set; }
        public string situacionPresentacion { get; set; }
        public string tipoIdentificacionEmisor { get; set; }
        public string JSON { get; set; }
        public string RespuestaHacienda { get; set; }
        public string XMLRespuesta { get; set; }
        public string ClaveReceptor { get; set; }
        public string ConsecutivoReceptor { get; set; }
    }
}