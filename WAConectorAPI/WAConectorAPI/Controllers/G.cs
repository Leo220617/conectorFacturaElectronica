using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WAConectorAPI.Models.ModelCliente;

namespace WAConectorAPI.Controllers
{
    public class G
    {
        ModelCliente db = new ModelCliente();

        public string GuardarPDF(byte[] result, string idFac)
        {
            

            try
            {
                byte[] bytes = result;

                string path = HttpContext.Current.Server.MapPath("~") + $"\\Temp\\{idFac}.pdf";
               


                System.IO.File.WriteAllBytes(path, bytes);


                return idFac + ".pdf";

            }

            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.DocNum = "";
                be.Type = "";
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                return "";
            }

        }
        public byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionLevel.Optimal))
                {
                    //msi.CopyTo(gs);

                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static FacturaXml ObtenerDatosXmlRechazado(string xml)
        {

            Regex GetXmlNumeroConsecutivo = new Regex("<Clave>([^\"]*)</Clave>");
            Regex GetXmlNumeroConsecutivo1 = new Regex("<NumeroConsecutivo>([^\"]*)</NumeroConsecutivo>");
            Regex GetXmlFechaEmision = new Regex("<FechaEmision>([^\"]*)</FechaEmision>");
            Regex GetXmlEmisor = new Regex("<Emisor>([^\"]*)</Emisor>");
            Regex GetXmlEmisorNombre = new Regex("<Nombre>([^\"]*)</Nombre>");
            Regex GetXmlNumero = new Regex("<Numero>([^\"]*)</Numero>");
            Regex GetXmlCodigoMoneda = new Regex("<CodigoMoneda>([^\"]*)</CodigoMoneda>");
            Regex GetXmlTotalComprobante = new Regex("<TotalComprobante>([^\"]*)</TotalComprobante>");
            Regex GetXmlTotalImpuesto = new Regex("<TotalImpuesto>([^\"]*)</TotalImpuesto>");
            Regex GetXmlReceptor = new Regex("<Receptor>([^\"]*)</Receptor>");
            Regex GetXmlIdReceptor = new Regex("<Numero>([^\"]*)</Numero>");
            Regex GetXmlIdEmisor = new Regex("<Tipo>([^\"]*)</Tipo>");

            FacturaXml facturaxml = new FacturaXml();
            string s = "<Fax xsi:nil=" + '"' + "true" + '"' + "/>";
            xml = xml.Trim().Replace("<Telefono xsi:nil=\"", "").Replace("true\"", "").Replace(" />", "");

            xml = xml.Trim().Replace("<Fax xsi:nil=\"", "").Replace("true\"", "").Replace(" />", "");
            try
            {
                facturaxml.NumeroConsecutivo = GetXmlNumeroConsecutivo.Match(xml).ToString().Replace("<Clave>", "").Replace("</Clave>", "");
                var Consec = GetXmlNumeroConsecutivo1.Match(xml).ToString().Replace("<NumeroConsecutivo>", "").Replace("</NumeroConsecutivo>", "");
                facturaxml.TipoDocumento = Consec.Substring(8, 2);
                if (facturaxml.TipoDocumento == "01")
                    facturaxml.TipoDocumentoDescripcion = "Factura Electrónica";
                else if (facturaxml.TipoDocumento == "02")
                    facturaxml.TipoDocumentoDescripcion = "Nota de Débito";
                else if (facturaxml.TipoDocumento == "03")
                    facturaxml.TipoDocumentoDescripcion = "Nota de Crédito";
                else
                    facturaxml.TipoDocumento = "Tiquete Electrónico";

                facturaxml.tipoIdentificacionEmisor = GetXmlIdEmisor.Match(GetXmlEmisor.Match(xml).ToString().Replace("<Emisor>", "").Replace("</Emisor>", "")).ToString().Replace("<Tipo>", "").Replace("</Tipo>", "");

                string _FechaEmision = GetXmlFechaEmision.Match(xml).ToString().Replace("<FechaEmision>", "").Replace("</FechaEmision>", "").Substring(0, 10);
                string[] Array_FechaEmision = _FechaEmision.Split('-');
                if (Array_FechaEmision.Length == 3)
                    facturaxml.FechaEmision = Array_FechaEmision[2] + "/" + Array_FechaEmision[1] + "/" + Array_FechaEmision[0];
                facturaxml.NombreEmisor = GetXmlEmisorNombre.Match(GetXmlEmisor.Match(xml).ToString().Replace("<Emisor>", "").Replace("</Emisor>", "")).ToString().Replace("<Nombre>", "").Replace("</Nombre>", "");
                facturaxml.Numero = GetXmlNumero.Match(GetXmlEmisor.Match(xml).ToString().Replace("<Emisor>", "").Replace("</Emisor>", "")).ToString().Replace("<Numero>", "").Replace("</Numero>", "");
                facturaxml.CodigoMoneda = GetXmlCodigoMoneda.Match(xml).ToString().Replace("<CodigoMoneda>", "").Replace("</CodigoMoneda>", "");
                var Total = GetXmlTotalComprobante.Match(xml).ToString().Replace("<TotalComprobante>", "").Replace("</TotalComprobante>", "");

                try
                {
                    facturaxml.TotalComprobante = Convert.ToDecimal(Total);
                }
                catch (Exception)
                {

                    facturaxml.TotalComprobante = Convert.ToDecimal(Total.Replace(".",","));
                }

                

                var TImpuesto = GetXmlTotalImpuesto.Match(xml).ToString().Replace("<TotalImpuesto>", "").Replace("</TotalImpuesto>", "");

                try
                {
                    facturaxml.Impuesto = Convert.ToDecimal(TImpuesto);
                }
                catch (Exception)
                {
                    try
                    {
                        facturaxml.Impuesto = Convert.ToDecimal(TImpuesto.Replace(".", ","));
                    }
                    catch (Exception)
                    {

                        facturaxml.Impuesto = 0;
                    }
                    

                }
                 

                facturaxml.IdReceptor = GetXmlIdReceptor.Match(GetXmlReceptor.Match(xml).ToString().Replace("<Receptor>", "").Replace("</Receptor>", "")).ToString().Replace("<Numero>", "").Replace("</Numero>", "");
            }
            catch (Exception ex)
            {
                facturaxml = null;
            }
            return facturaxml;
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public partial class FacturaXml
    {
        public FacturaXml()
        {
            NumeroConsecutivo = "";
            TipoDocumento = "";
            TipoDocumentoDescripcion = "";
            FechaEmision = "";
            NombreEmisor = "";
            Numero = "";
            CodigoMoneda = "";
            TotalComprobante = 0;
            IdReceptor = "";
            Impuesto = 0;
            tipoIdentificacionEmisor = "";
        }

        public string NumeroConsecutivo { get; set; }
        public string TipoDocumento { get; set; }
        public string TipoDocumentoDescripcion { get; set; }
        public string FechaEmision { get; set; }
        public string NombreEmisor { get; set; }
        public string Numero { get; set; }
        public string CodigoMoneda { get; set; }
        public decimal TotalComprobante { get; set; }
        public string IdReceptor { get; set; }
        public decimal Impuesto { get; set; }
        public string tipoIdentificacionEmisor { get; set; }
    }


}