using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using WAConectorAPI.Models;

using WAConectorAPI.Models.ModelCliente;

namespace WAConectorAPI.Controllers
{
    public class Metodos
    {
        ModelCliente db = new ModelCliente();

        public bool SendV2(string para, string copia, string copiaOculta, string de, string displayName, string asunto,
           string html, string HostServer, int Puerto, bool EnableSSL, string UserName, string Password, List<Attachment> ArchivosAdjuntos = null)
        {
            try
            {

                MailMessage mail = new MailMessage();
                mail.Subject = asunto;
                mail.Body = html;
                mail.IsBodyHtml = true;

                // * mail.From = new MailAddress(WebConfigurationManager.AppSettings["UserName"], displayName);
                mail.From = new MailAddress(de, displayName);

                var paraList = para.Split(';');
                foreach (var p in paraList)
                {
                    if (p.Trim().Length > 0)
                        mail.To.Add(p.Trim());
                }
                var ccList = copia.Split(';');
                foreach (var cc in ccList)
                {
                    if (cc.Trim().Length > 0)
                        mail.CC.Add(cc.Trim());
                }
                var ccoList = copiaOculta.Split(';');
                foreach (var cco in ccoList)
                {
                    if (cco.Trim().Length > 0)
                        mail.Bcc.Add(cco.Trim());
                }



                if (ArchivosAdjuntos != null)
                {
                    foreach (var archivo in ArchivosAdjuntos)
                    {
                        //if (!string.IsNullOrEmpty(archivo))
                        mail.Attachments.Add(archivo);
                    }
                }


                SmtpClient client = new SmtpClient();
                client.Host = HostServer;
                client.Port = Puerto;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = EnableSSL;
                client.Credentials = new NetworkCredential(UserName, Password);

                client.Send(mail);
                client.Dispose();
                mail.Dispose();

                return true;

            }
            catch (Exception ex)
            {


                return false;
            }
        }


        public string DevuelveCadena(string CodSucursal)
        {
            try
            {
                var Sucursal = db.Sucursales.Where(a => a.codSuc == CodSucursal).FirstOrDefault();
                var Datos = db.ConexionSAP.Where(a => a.id == Sucursal.idConexion).FirstOrDefault();

                var sql = "server=" + Datos.SQLServer + "; database=" + Datos.SQLBD + "; uid=" + Datos.SQLUser + "; pwd=" + Datos.SQLPass + ";";

                return sql;
            }
            catch (Exception ex)
            {

                return ex.StackTrace;
            }

        }

        public string DevuelveCadenaHana(string CodSucursal)
        {
            try
            {

                var Datos = db.ConexionSAP.Where(a => a.id == 2).FirstOrDefault();
                // var sql =  "DSN=HANA64;"+"SERVERNODE=" + Datos.SQLServer+ ";UID=" + Datos.SQLUser +";PWD="+ Datos.SQLPass +";DATABASENAME=" +Datos.SQLBD + ";";
                var sql = "DSN=HANA64;Server=" + Datos.SQLServer + ";UserName=" + Datos.SQLUser + ";Password=" + Datos.SQLPass;


                return sql;
            }
            catch (Exception ex)
            {

                return ex.StackTrace;
            }
        }

        public string DevuelveSCHEMA(string CodSucursal)
        {
            try
            {

                var Datos = db.ConexionSAP.Where(a => a.id == 2).FirstOrDefault();
                var sql = Datos.SQLBD;


                return sql;
            }
            catch (Exception ex)
            {

                return ex.StackTrace;
            }
        }


        public MakeXML RellenarXML(EncDocumento enc, DetDocumento[] det, bool FCE = false)
        {

            try
            {
                MakeXML xml = new MakeXML();
                var Sucursal = db.Sucursales.Where(a => a.codSuc == enc.idSucursal).FirstOrDefault();


                xml.api_key = Sucursal.ApiKey;
                //Generacion del nodo clave
                xml.clave = new clave();
                xml.clave.sucursal = Sucursal.codSuc;
                xml.clave.terminal = Sucursal.Terminal;
                xml.clave.tipo = enc.TipoDocumento;
                xml.clave.comprobante = enc.consecutivoInterno.ToString().Replace(",", ".");
                xml.clave.pais = Sucursal.codPais;
                xml.clave.dia = (enc.Fecha.Value.Day < 10 ? "0" + enc.Fecha.Value.Day.ToString() : enc.Fecha.Value.Day.ToString()).ToString();
                xml.clave.mes = (enc.Fecha.Value.Month < 10 ? "0" + enc.Fecha.Value.Month.ToString() : enc.Fecha.Value.Month.ToString()).ToString();
                xml.clave.anno = enc.Fecha.Value.Year.ToString().Substring(2, 2);
                xml.clave.situacion_presentacion = (DateTime.Now.Date - enc.Fecha.Value.Date).TotalDays == 0 ? "1" : "3";
                xml.clave.codigo_seguridad = GeneraNumero(8);

                //Generacion del nodo de encabezado

                xml.encabezado = new encabezado();
                xml.encabezado.codigo_actividad = enc.CodActividadEconomica;
                xml.encabezado.fecha = enc.Fecha.Value.ToString("yyyy-MM-ddThh:mm:ss-06:00");
                xml.encabezado.condicion_venta = enc.condicionVenta;
                xml.encabezado.plazo_credito = enc.plazoCredito.ToString().Replace(",", ".");
                xml.encabezado.medio_pago = enc.medioPago;


                //Generacion del nodo emisor

                xml.emisor = new emisor();

                if (!FCE)
                {
                    xml.emisor.nombre = Sucursal.Nombre;
                    //Generacion del nodo hijo de emisor de identificacion
                    xml.emisor.identificacion = new identificacion();
                    xml.emisor.identificacion.tipo = Sucursal.TipoCedula;
                    xml.emisor.identificacion.numero = Sucursal.Cedula;

                    xml.emisor.nombre_comercial = Sucursal.NombreComercial;

                    //Generacion del nodo hijo de emisor de ubicacion
                    xml.emisor.ubicacion = new ubicacion();
                    xml.emisor.ubicacion.provincia = Sucursal.Provincia.ToString();
                    xml.emisor.ubicacion.canton = Sucursal.Canton.ToString();
                    xml.emisor.ubicacion.distrito = Sucursal.Distrito.ToString();
                    xml.emisor.ubicacion.barrio = Sucursal.Barrio.ToString();
                    xml.emisor.ubicacion.sennas = Sucursal.sennas.ToString();

                    //Generacion del nodo hijo de emisor de telefono
                    xml.emisor.telefono = new telefono();
                    xml.emisor.telefono.cod_pais = Sucursal.codPais;
                    xml.emisor.telefono.numero = Sucursal.Telefono;

                    xml.emisor.correo_electronico = Sucursal.Correo;

                    if (enc.TipoIdentificacion != null) //Es tiquete electronico
                    {
                        //Generacion del nodo de receptor
                        xml.receptor = new receptor();
                        xml.receptor.nombre = enc.CardName;

                        if (enc.TipoIdentificacion == "EX")
                        // if(enc.LicTradNum.Length >= 12 && (enc.TipoDocumento == "09" || enc.TipoDocumento == "03" || enc.TipoDocumento == "02"))
                        {
                            xml.receptor.correo_electronico = enc.Email;
                            xml.receptor.IdentificacionExtranjero = enc.LicTradNum;
                            xml.receptor.sennas_extranjero = "ninguna";
                        }
                        else
                        {
                            //Generacion del nodo hijo de receptor de identificacion
                            xml.receptor.identificacion = new identificacion();
                            xml.receptor.identificacion.tipo = enc.TipoIdentificacion;
                            xml.receptor.identificacion.numero = enc.LicTradNum;
                        }


                    }
                    else
                    {
                        xml.receptor = new receptor();
                        xml.receptor.nombre = enc.CardName;
                    }
                }
                else //Factura de compra
                {
                    xml.emisor.nombre = enc.CardName;
                    //Generacion del nodo hijo de emisor de identificacion
                    xml.emisor.identificacion = new identificacion();
                    xml.emisor.identificacion.tipo = enc.TipoIdentificacion;
                    xml.emisor.identificacion.numero = enc.LicTradNum;

                    xml.emisor.correo_electronico = enc.Email;
                    //Generacion del nodo hijo de emisor de ubicacion
                    var Para = db.Parametros.FirstOrDefault();
                    var SQL = " " + Para.UbicacionCliente + "'" + enc.CardCode + "' ";
                    var conexion = DevuelveCadena(enc.idSucursal);

                    SqlConnection Cn = new SqlConnection(conexion);
                    SqlCommand Cmd = new SqlCommand(SQL, Cn);
                    SqlDataAdapter Da = new SqlDataAdapter(Cmd);
                    DataSet Ds = new DataSet();
                    Cn.Open();
                    Da.Fill(Ds, "Proveedor");

                    var Provincia = Convert.ToInt32(Ds.Tables["Proveedor"].Rows[0]["State"].ToString());
                    var Canton = Ds.Tables["Proveedor"].Rows[0]["County"].ToString();
                    var Distrito = Ds.Tables["Proveedor"].Rows[0]["Block"].ToString();



                    Cn.Close();
                    xml.emisor.ubicacion = new ubicacion();
                    xml.emisor.ubicacion.provincia = Provincia.ToString();
                    xml.emisor.ubicacion.canton = db.Cantones.Where(a => a.NomCanton.ToLower().Contains(Canton.ToLower())).FirstOrDefault().CodCanton.ToString();
                    var canton = db.Cantones.Where(a => a.NomCanton.ToLower().Contains(Canton.ToLower())).FirstOrDefault().CodCanton;
                    xml.emisor.ubicacion.distrito = db.Distritos.Where(a => a.NomDistrito.ToLower().Contains(Distrito.ToLower()) && a.CodCanton == canton && a.CodProvincia == Provincia).FirstOrDefault().CodDistrito.ToString();
                    var distrito = db.Distritos.Where(a => a.NomDistrito.ToLower().Contains(Distrito.ToLower()) && a.CodCanton == canton && a.CodProvincia == Provincia).FirstOrDefault().CodDistrito;
                    xml.emisor.ubicacion.barrio = db.Barrios.Where(a => a.NomBarrio.ToLower().Contains(Distrito.ToLower())).FirstOrDefault() != null ? db.Barrios.Where(a => a.NomBarrio.ToLower().Contains(Distrito.ToLower())).FirstOrDefault().CodBarrio.ToString() : "1";
                    xml.emisor.ubicacion.sennas = Ds.Tables["Proveedor"].Rows[0]["Street"].ToString();

                    //Generacion del nodo de receptor
                    xml.receptor = new receptor();
                    xml.receptor.nombre = Sucursal.Nombre;

                    //Generacion del nodo hijo de receptor de identificacion
                    xml.receptor.identificacion = new identificacion();
                    xml.receptor.identificacion.tipo = Sucursal.TipoCedula;
                    xml.receptor.identificacion.numero = Sucursal.Cedula;
                    xml.receptor.correo_electronico = Sucursal.Correo;



                }




                xml.detalle = new detalleP[det.Length];

                int i = 0;
                foreach (var item in det)
                {
                    detalleP de = new detalleP();
                    de.numero = item.NumLinea.ToString();
                    de.partida = item.partidaArancelaria.ToString();
                    de.codigo_hacienda = item.CodCabys;

                    if (!string.IsNullOrEmpty(item.codPro) && !string.IsNullOrEmpty(item.tipoCod))
                    {
                        de.codigo = new codigos[1];
                        codigos cod = new codigos();
                        cod.tipo = item.tipoCod;
                        cod.codigo = item.codPro;
                        de.codigo[0] = cod;
                    }



                    de.cantidad = Math.Round(item.cantidad, 2).ToString().Replace(",", ".");
                    de.unidad_medida = item.unidadMedida;
                    de.unidad_medida_comercial = item.unidadMedidaComercial;
                    de.detalle = item.NomPro;
                    de.precio_unitario = Math.Round(item.PrecioUnitario, 2).ToString().Replace(",", ".");
                    de.monto_total = item.MontoTotal.ToString().Replace(",", ".");
                    de.descuento = new descuento[1];
                    descuento desc = new descuento();
                    desc.monto = item.MontoDescuento.ToString().Replace(",", ".");
                    desc.naturaleza = item.NaturalezaDescuento;
                    de.descuento[0] = desc;

                    de.subtotal = item.SubTotal.ToString().Replace(",", ".");


                    if (enc.TipoDocumento == "09")
                    {
                        de.baseimponible = null;
                        de.impuestos = null;
                    }
                    else
                    {
                        de.baseimponible = item.baseImponible.ToString().Replace(",", ".");
                        de.impuestos = new impuestos[1];
                        impuestos imp = new impuestos();
                        var Impuesto = db.Impuestos.Where(a => a.id == item.idImpuesto).FirstOrDefault();

                        imp.codigo = Impuesto.codigo;
                        imp.codigotarifa = Impuesto.codigoTarifa;
                        imp.tarifa = Impuesto.tarifa.ToString().Replace(",", ".");
                        imp.factoriva = item.factorIVA == 0 ? null : Math.Round(item.factorIVA).ToString().Replace(",", ".");
                        imp.monto = item.montoImpuesto.ToString().Replace(",", ".");
                        imp.exportacion = item.exportacion == 0 ? null : item.exportacion.ToString().Replace(",", ".");
                        if (!string.IsNullOrEmpty(item.exonNumdoc))
                        {

                            imp.exoneracion = new exoneracion();
                            imp.exoneracion.tipodocumento = item.exonTipoDoc;
                            imp.exoneracion.numerodocumento = item.exonNumdoc;
                            imp.exoneracion.nombreinstitucion = item.exonNomInst;
                            imp.exoneracion.fechaemision = item.exonFecEmi.ToString("yyyy-MM-ddThh:mm:ss-06:00");
                            imp.exoneracion.porcentajeexoneracion = item.exonPorExo.ToString().Replace(",", ".");
                            imp.exoneracion.montoexoneracion = item.exonMonExo.ToString().Replace(",", ".");
                        }
                        de.impuestos[0] = imp;

                    }
                    de.impuestoneto = item.impNeto.ToString().Replace(",", ".");
                    de.montototallinea = item.totalLinea.ToString().Replace(",", ".");

                    xml.detalle[i] = de;
                    i++;

                }

                var OtrosCargos = db.OtrosCargos.Where(a => a.idEncabezado == enc.id).ToList();

                if (OtrosCargos.Count() > 0)
                {
                    xml.otroscargos = new otroscargos[OtrosCargos.Count()];

                    var z = 0;
                    foreach (var item in OtrosCargos)
                    {
                        otroscargos oc = new otroscargos();
                        oc.tipodocumento = item.tipoDocumento;
                        oc.nombre = "";
                        oc.numeroidentificacion = "";
                        oc.detalle = item.detalle;
                        oc.porcentaje = "";
                        oc.montocargo = item.monto.ToString().Replace(",", ".");


                        xml.otroscargos[z] = oc;
                        z++;
                    }
                }

                xml.resumen = new resumen();
                xml.resumen.moneda = enc.moneda == "COL" ? "CRC" : enc.moneda;
                xml.resumen.tipo_cambio = Math.Round(enc.tipoCambio.Value, 2).ToString().Replace(",", ".");
                xml.resumen.totalserviciogravado = enc.totalserviciogravado == 0 ? null : enc.totalserviciogravado.ToString().Replace(",", ".");
                xml.resumen.totalservicioexento = enc.totalservicioexento.ToString().Replace(",", ".");
                xml.resumen.totalservicioexonerado = enc.totalservicioexonerado.ToString().Replace(",", ".");
                xml.resumen.totalmercaderiagravado = enc.totalmercaderiagravado == 0 ? null : enc.totalmercaderiagravado.ToString().Replace(",", ".");
                xml.resumen.totalmercaderiaexento = enc.totalmercaderiaexenta.ToString().Replace(",", ".");
                xml.resumen.totalmercaderiaexonerado = enc.totalmercaderiaexonerado == 0 ? null : enc.totalmercaderiaexonerado.ToString().Replace(",", ".");
                xml.resumen.totalgravado = enc.totalgravado.ToString().Replace(",", ".");
                xml.resumen.totalexento = enc.totalexento.ToString().Replace(",", ".");
                xml.resumen.totalexonerado = enc.totalexonerado.ToString().Replace(",", ".");
                xml.resumen.totalventa = enc.totalventa.ToString().Replace(",", ".");
                xml.resumen.totaldescuentos = enc.totaldescuentos.ToString().Replace(",", ".");
                xml.resumen.totalventaneta = enc.totalventaneta.ToString().Replace(",", ".");
                xml.resumen.totalimpuestos = enc.totalimpuestos.ToString().Replace(",", ".");
                xml.resumen.totalivadevuelto = enc.totalivadevuelto == 0 ? null : enc.totalivadevuelto.ToString().Replace(",", ".");
                xml.resumen.totalotroscargos = enc.totalotroscargos == 0 ? null : enc.totalotroscargos.ToString().Replace(",", ".");
                xml.resumen.totalcomprobante = enc.totalcomprobante.ToString().Replace(",", ".");



                if (!string.IsNullOrEmpty(enc.RefNumeroDocumento) && enc.RefNumeroDocumento != "0")
                {
                    xml.referencia = new referencia[1];
                    xml.referencia[0] = new referencia();
                    xml.referencia[0].tipo_documento = enc.RefTipoDocumento;
                    xml.referencia[0].numero_documento = enc.RefNumeroDocumento;
                    xml.referencia[0].fecha_emision = enc.RefFechaEmision.ToString("yyyy-MM-ddThh:mm:ss-06:00");
                    xml.referencia[0].codigo = enc.RefCodigo;
                    xml.referencia[0].razon = enc.RefRazon;


                }



                var OtrosTexto = db.OtrosTextos.Where(a => a.idEncabezado == enc.id).ToList();
                var comprasEntregas = db.OtrosTextos.Where(a => a.idEncabezado == enc.id && a.codigo == "OC").FirstOrDefault();

                if (OtrosTexto.Count() > 0)
                {
                    //if(comprasEntregas != null)
                    //{
                    //    var cantidad = OtrosTexto.Count() + 1;
                    //    xml.otros = new otros[cantidad];
                    //}
                    //else
                    //{
                    //    xml.otros = new otros[OtrosTexto.Count()];

                    //}
                    xml.otros = new otros[OtrosTexto.Count()];

                    if (comprasEntregas != null)
                    {
                        OtrosTexto.Remove(comprasEntregas);
                    }

                    var z = 0;
                    foreach (var item in OtrosTexto)
                    {
                        otros oc = new otros();
                        oc.codigo = item.codigo;
                        oc.texto = item.detalle;



                        xml.otros[z] = oc;
                        z++;
                    }

                    if (comprasEntregas != null)
                    {
                        otros oc = new otros();
                        oc.codigo = "CECR_TPL_ORDEN";
                        oc.texto = comprasEntregas.detalle;
                        xml.otros[z] = oc;
                    }

                }

                xml.envio = new envio();
                xml.envio.aplica = "1";
                xml.envio.emisor = new emisorF();
                xml.envio.emisor.correo = Sucursal.Correo;
                xml.envio.receptor = new receptorF();
                if (!string.IsNullOrEmpty(enc.Email))
                {
                    xml.envio.receptor.correo = enc.Email;

                }
                else
                {
                    xml.envio.receptor.correo = Sucursal.Correo;
                }
                xml.envio.logo = Sucursal.Logo;
                xml.envio.texto = enc.Comentarios;


                return xml;
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
                return new MakeXML();
            }


        }




        public string GeneraNumero(int cant = 8)
        {
            string codigo = "";
            var seed = Environment.TickCount;
            var random = new Random(seed);
            for (int i = 0; i < cant; i++)
            {


                codigo += random.Next(0, 10);
            }


            return codigo;
        }



        public string ObtenerConfig(string v)
        {
            try
            {
                return WebConfigurationManager.AppSettings[v];
            }
            catch
            {
                return "";
            }
        }
    }
}