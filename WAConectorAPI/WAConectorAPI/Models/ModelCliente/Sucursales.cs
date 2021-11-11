namespace WAConectorAPI.Models.ModelCliente
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Sucursales
    {
        [Key]
        [StringLength(3)]
        public string codSuc { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string NombreComercial { get; set; }

        [StringLength(5)]
        public string Terminal { get; set; }

        [StringLength(2)]
        public string TipoCedula { get; set; }

        [StringLength(12)]
        public string Cedula { get; set; }

        [StringLength(1)]
        public string Provincia { get; set; }

        [StringLength(2)]
        public string Canton { get; set; }

        [StringLength(2)]
        public string Distrito { get; set; }

        [StringLength(2)]
        public string Barrio { get; set; }

        [StringLength(250)]
        public string sennas { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(100)]
        public string Correo { get; set; }

        public string Logo { get; set; }

        public string ApiKey { get; set; }

        public int? consecFac { get; set; }

        public int? consecTiq { get; set; }

        public int? consecNC { get; set; }

        public int? consecND { get; set; }

        public int consecFEC { get; set; }
        public int consecFEE { get; set; }
        public int consecAFC { get; set; }
        [StringLength(3)]
        public string codPais { get; set; }

        public int idConexion { get; set; }

        [StringLength(6)]
        public string CodActividadComercial { get; set; }
    }
}
