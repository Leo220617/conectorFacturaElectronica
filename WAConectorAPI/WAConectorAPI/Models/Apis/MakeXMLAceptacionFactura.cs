using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WAConectorAPI.Models.Apis
{
    public class MakeXMLAceptacionFactura
    {
        public string api_key { get; set; }
        public clave clave { get; set; }
        public emisor emisor { get; set; }
        public parametros parametros { get; set; }
    }
    public class clave
    {
        public string tipo { get; set; }
        public string sucursal { get; set; }
        public string terminal { get; set; }
        public string numero_documento { get; set; }
        public string numero_cedula_emisor { get; set; }
        public string fecha_emision_doc { get; set; }
        public string mensaje { get; set; }
        public string detalle_mensaje { get; set; }
        public string codigo_actividad { get; set; }
        public string condicion_impuesto { get; set; }
        public string impuesto_acreditar { get; set; }
        public string gasto_aplicable { get; set; }
        public string monto_total_impuesto { get; set; }
        public string total_factura { get; set; }
        public string numero_cedula_receptor { get; set; }
        public string num_consecutivo_receptor { get; set; }
        public string situacion_presentacion { get; set; }
        public string codigo_seguridad { get; set; }
    }
    public class identificacion
    {
        public string tipo { get; set; }
        public string numero { get; set; }
    }
    public class emisor
    {
        
        public identificacion identificacion { get; set; }
        


    }
    public class parametros
    {
        public string enviodgt { get; set; }
    }

}