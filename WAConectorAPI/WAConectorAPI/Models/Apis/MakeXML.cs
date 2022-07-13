using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WAConectorAPI.Models
{
    public class MakeXML
    {
        public string api_key { get; set; }
        public clave clave { get; set; }
        public encabezado encabezado { get; set; }
        public emisor emisor { get; set; }
        public receptor receptor { get; set; }
        public detalleP[] detalle { get; set; }
        public otroscargos[] otroscargos { get; set; }
        public resumen resumen { get; set; }
        public compra_entrega compra_entrega { get; set; }
        public referencia[] referencia { get; set; }
        public otros[] otros { get; set; }
        public envio envio { get; set; }
    }

    public class clave
    {
        public string sucursal { get; set; }
        public string terminal { get; set; }
        public string tipo { get; set; }
        public string comprobante { get; set; }
        public string pais { get; set; }
        public string dia { get; set; }
        public string mes { get; set; }
        public string anno { get; set; }
        public string situacion_presentacion { get; set; }
        public string codigo_seguridad { get; set; }
    }

    public class encabezado
    {
        public string codigo_actividad { get; set; }
        public string fecha { get; set; }
        public string condicion_venta { get; set; }
        public string plazo_credito { get; set; }
        public string medio_pago { get; set; }
    }

    public class identificacion
    {
        public string tipo { get; set; }
        public string numero { get; set; }
    }

    public class ubicacion
    {
        public string provincia { get; set; }
        public string canton { get; set; }
        public string distrito { get; set; }
        public string barrio { get; set; }
        public string sennas { get; set; }
    }

    public class telefono
    {
        public string cod_pais { get; set; }
        public string numero { get; set; }
    }

    public class emisor
    {
        public string nombre { get; set; }
        public identificacion identificacion { get; set; }
        public string nombre_comercial { get; set; }
        public ubicacion ubicacion { get; set; }
        public telefono telefono { get; set; }
        public string correo_electronico { get; set; }


    }

    public class receptor
    {
        public string nombre { get; set; }
        public identificacion identificacion { get; set; }
        public string IdentificacionExtranjero { get; set; }
        public string correo_electronico { get; set; }
        public string sennas_extranjero { get; set; }
    }

    public class codigos
    {
        public string tipo { get; set; }
        public string codigo { get; set; }
    }

    public class descuento
    {
        public string monto { get; set; }
        public string naturaleza { get; set; }
    }


    public class exoneracion
    {
        public string tipodocumento { get; set; }
        public string numerodocumento { get; set; }
        public string nombreinstitucion { get; set; }
        public string fechaemision { get; set; }
        public string porcentajeexoneracion { get; set; } // int o string
        public string montoexoneracion { get; set; } //decimal o string
    }

    public class impuestos
    {
        public string codigo { get; set; }
        public string codigotarifa { get; set; }
        public string tarifa { get; set; }// string o decimal
        public string factoriva { get; set; } //string o decimal
        public string monto { get; set; } //string o decimal
        public string exportacion { get; set; }
        public exoneracion exoneracion { get; set; }

    }

    public class detalleP
    {
        public string numero { get; set; }
        public string partida { get; set; } //factura de exportacion y venta de mercancia
        public string codigo_hacienda { get; set; }
        public codigos[] codigo { get; set; }
        public string cantidad { get; set; } //ver si es decimal o string
        public string unidad_medida { get; set; }
        public string unidad_medida_comercial { get; set; }
        public string detalle { get; set; }
        public string precio_unitario { get; set; } //string o decimal
        public string monto_total { get; set; } // string o decimal
        public descuento[] descuento { get; set; }
        public string subtotal { get; set; }
        public string baseimponible { get; set; }//string o decimal
        public impuestos[] impuestos { get; set; }
        public string impuestoneto { get; set; } //decimal o string
        public string montototallinea { get; set; } //decimal o string
    }

    public class otroscargos
    {
        public string tipodocumento { get; set; }
        public string nombre { get; set; }
        public string numeroidentificacion { get; set; }
        public string detalle { get; set; }
        public string porcentaje { get; set; } //decimal o string
        public string montocargo { get; set; } //decimal o string
    }

    public class resumen
    {
        public string moneda { get; set; }
        public string tipo_cambio { get; set; } //string o decimal
        public string totalserviciogravado { get; set; } //decumal o string
        public string totalservicioexento { get; set; } //decimal o string
        public string totalservicioexonerado { get; set; }
        public string totalmercaderiagravado { get; set; }
        public string totalmercaderiaexento { get; set; }
        public string totalmercaderiaexonerado { get; set; }
        public string totalgravado { get; set; }
        public string totalexento { get; set; }
        public string totalexonerado { get; set; }
        public string totalventa { get; set; }
        public string totaldescuentos { get; set; }
        public string totalventaneta { get; set; }
        public string totalimpuestos { get; set; }
        public string totalivadevuelto { get; set; }
        public string totalotroscargos { get; set; }
        public string totalcomprobante { get; set; }
    }

    public class otros
    {
        public string codigo { get; set; }
        public string texto { get; set; }
    }

    public class referencia
    {
        public string tipo_documento { get; set; }
        public string numero_documento { get; set; }
        public string fecha_emision { get; set; }
        public string codigo { get; set; }
        public string razon { get; set; }
    }

    public class emisorF
    {
        public string correo { get; set; }
    }

    public class receptorF
    {
        public string correo { get; set; }
    }

    public class envio
    {
        public string aplica { get; set; }
        public emisorF emisor { get; set; }
        public receptorF receptor { get; set; }
        public string logo { get; set; }
        public string texto { get; set; }

    }
    public class compra_entrega
    {
        public string numeroorden { get; set; }
    }
}