using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
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
        //public string UnZip(byte[] compressedBytes)
        //{
        //    using (var msi = new MemoryStream(compressedBytes))
        //    using (var mso = new MemoryStream())
        //    {
        //        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        //        {
        //            CopyTo(gs, mso);
        //        }

        //        return Encoding.UTF8.GetString(mso.ToArray());
        //    }
        //}

        public string UnZip(byte[] compressedBytes)
        {
            using (var msi = new MemoryStream(compressedBytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    // Leer los datos descomprimidos en un búfer temporal
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = gs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        mso.Write(buffer, 0, bytesRead);
                    }
                }

                // Colocar el puntero al principio del flujo de salida para leerlo correctamente
                mso.Seek(0, SeekOrigin.Begin);

                // Leer los datos descomprimidos del flujo de salida como una cadena UTF-8
                return Encoding.UTF8.GetString(mso.ToArray());
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

                CultureInfo culture = CultureInfo.InvariantCulture;
                facturaxml.IdReceptor = GetXmlIdReceptor.Match(GetXmlReceptor.Match(xml).ToString().Replace("<Receptor>", "").Replace("</Receptor>", "")).ToString().Replace("<Numero>", "").Replace("</Numero>", "");
                decimal iva1 = 0;
                decimal iva2 = 0;
                decimal iva4 = 0;
                decimal iva8 = 0;
                decimal iva13 = 0;
               

                var xml2 = ConvertirArchivoaXElement(xml,facturaxml.IdReceptor);
                foreach (var item2 in xml2.Elements().Where(m => m.Name.LocalName == "DetalleServicio").Elements())
                {
                    

                    var ExoneracionPorcentajeCompra = decimal.Parse(ExtraerValorDeNodoXml(item2, "Impuesto/Exoneracion/PorcentajeCompra", true), culture);
                    var ImpuestoTarifa = Convert.ToDecimal(ExtraerValorDeNodoXml(item2, "Impuesto/Tarifa", true), culture);
                    var ImpuestoMonto = decimal.Parse(ExtraerValorDeNodoXml(item2, "Impuesto/Monto", true), culture);
                    var SubTotal = decimal.Parse(ExtraerValorDeNodoXml(item2, "SubTotal", true), culture);
                    var MontoDescuento = decimal.Parse(ExtraerValorDeNodoXml(item2.Elements().Where(a => a.Name.LocalName == "Descuento").FirstOrDefault(), "MontoDescuento", true), culture);

                    int opcion = Convert.ToInt32(ImpuestoTarifa);
                    decimal cantidadImpuesto = 0;
                    bool bandera = false;
                    if (ExoneracionPorcentajeCompra > 0)
                    {
                        bandera = true;
                        cantidadImpuesto = opcion - ExoneracionPorcentajeCompra;
                    }
                    switch (opcion)
                    {
                        case 1:
                            {
                                if (!bandera)
                                {
                                    iva1 += ImpuestoMonto;
                                }
                                else
                                {
                                    if (cantidadImpuesto > 0)
                                    {
                                        iva1 += ((SubTotal - MontoDescuento) * (cantidadImpuesto / 100));
                                    }
                                }
                                break;
                            }
                        case 2:
                            {
                                if (!bandera)
                                {
                                    iva2 += ImpuestoMonto;
                                }
                                else
                                {
                                    if (cantidadImpuesto > 0)
                                    {
                                        iva2 += ((SubTotal - MontoDescuento) * (cantidadImpuesto / 100));
                                    }
                                }
                                break;
                            }
                        case 4:
                            {
                                if (!bandera)
                                {
                                    iva4 += ImpuestoMonto;

                                }
                                else
                                {
                                    if (cantidadImpuesto > 0)
                                    {
                                        iva4 += ((SubTotal - MontoDescuento) * (cantidadImpuesto / 100));
                                    }
                                }
                                break;
                            }
                        case 8:
                            {
                                if (!bandera)
                                {
                                    iva8 += ImpuestoMonto;
                                }
                                else
                                {
                                    if (cantidadImpuesto > 0)
                                    {
                                        iva8 += ((SubTotal - MontoDescuento) * (cantidadImpuesto / 100));
                                    }
                                }
                                break;
                            }
                        case 13:
                            {
                                if (!bandera)
                                {
                                    iva13 += ImpuestoMonto;
                                }
                                else
                                {
                                    if (cantidadImpuesto > 0)
                                    {
                                        iva13 += ((SubTotal - MontoDescuento) * (cantidadImpuesto / 100));
                                    }
                                }
                                break;
                            }
                    }


              
                }

                facturaxml.IVA0 = 0;
                facturaxml.IVA1 = iva1;
                facturaxml.IVA2 = iva2;
                facturaxml.IVA4 = iva4;
                facturaxml.IVA8 = iva8;
                facturaxml.IVA13 = iva13;

            }
            catch (Exception ex)
            {
                facturaxml = null;
            }
            return facturaxml;
        }
        public static string ExtraerValorDeNodoXml(System.Xml.Linq.XElement elemento, string nombre, bool retornarCero = false)
        {
            try
            {
                string[] nombres = nombre.Split('/');
                string valor = "";

                if (nombres.Length == 1)
                    valor = elemento.Elements().Where(m => m.Name.LocalName == nombres[0]).FirstOrDefault().Value;
                else if (nombres.Length == 2)
                    valor = elemento.Elements().Where(m => m.Name.LocalName == nombres[0]).FirstOrDefault()
                        .Elements().Where(m => m.Name.LocalName == nombres[1]).FirstOrDefault().Value;
                else if (nombres.Length == 3)
                    valor = elemento.Elements().Where(m => m.Name.LocalName == nombres[0]).FirstOrDefault()
                        .Elements().Where(m => m.Name.LocalName == nombres[1]).FirstOrDefault()
                        .Elements().Where(m => m.Name.LocalName == nombres[2]).FirstOrDefault().Value;
                else if (nombres.Length == 4)
                    valor = elemento.Elements().Where(m => m.Name.LocalName == nombres[0]).FirstOrDefault()
                        .Elements().Where(m => m.Name.LocalName == nombres[1]).FirstOrDefault()
                        .Elements().Where(m => m.Name.LocalName == nombres[2]).FirstOrDefault()
                        .Elements().Where(m => m.Name.LocalName == nombres[3]).FirstOrDefault().Value;

                return valor;
            }
            catch (Exception ex)
            {
                if (retornarCero)
                    return "0";
                else
                    return "";
            }
        }
        public static XElement ConvertirArchivoaXElement(string result, string CodEmpresa = "1")
        {
            string codEmpresa = CodEmpresa;
            XElement xml = null;
            try
            {
                xml = XDocument.Parse(result).Elements().FirstOrDefault();
            }

            catch (Exception e)
            {
                // Error por UTF8 BOM .
                // leer el archivo original y convertirlo sin UTF8



                string rutaTemp = HttpContext.Current.Server.MapPath("~") + "\\Temp\\";

                if (!System.IO.Directory.Exists(rutaTemp))
                    System.IO.Directory.CreateDirectory(rutaTemp);

                string fic = HttpContext.Current.Server.MapPath("~") + $"\\Temp\\{codEmpresa}{TimeStamp(DateTime.Now)}.txt";
                string texto = result;

                System.IO.StreamWriter sw = new System.IO.StreamWriter(fic);
                sw.WriteLine(texto);
                sw.Close();
                // HttpContext.Current.Server.MapPath("~") + @"\" + nombreArchivo

                System.IO.StreamReader objReader = new System.IO.StreamReader(fic);
                texto = objReader.ReadToEnd();
                objReader.Close();

                // borrar archivod espues de utilizado
                System.IO.File.Delete(fic);
                try
                {
                    xml = XDocument.Parse(texto).Elements().FirstOrDefault();

                }
                catch (Exception ex)
                {
                  
                    int inicio = texto.IndexOf("<ds:Signature");
                    int fin = texto.IndexOf("</ds:Signature>") + "</ds:Signature>".Length;

                    string parte1 = texto.Substring(0, inicio);
                   
                    string parte3 = texto.Substring(fin);

                    // Cargar el XML en un objeto XmlDocument
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(parte1 + parte3);

                    // Obtener el XML correctamente formateado
                    xml = XDocument.Parse(doc.OuterXml).Elements().FirstOrDefault(); 

                }
            }

            return xml;
        }
        public static string TimeStamp(DateTime fechaActual)
        {
            long ticks = fechaActual.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000000; //Convert windows ticks to seconds
            return ticks.ToString();

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
            IVA0 = 0;
            IVA1 = 0;
            IVA2 = 0;
            IVA4 = 0;
            IVA8 = 0;
            IVA13 = 0;
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
        public decimal IVA0 { get; set; }
        public decimal IVA1 { get; set; }

        public decimal IVA2 { get; set; }

        public decimal IVA4 { get; set; }

        public decimal IVA8 { get; set; }

        public decimal IVA13 { get; set; }
    }


}