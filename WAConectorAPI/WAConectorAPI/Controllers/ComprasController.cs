using Newtonsoft.Json;
using S22.Imap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using WAConectorAPI.Models;
using WAConectorAPI.Models.Apis;
using WAConectorAPI.Models.ModelCliente;

namespace WAConectorAPI.Controllers
{
    public class ComprasController:ApiController
    {
        ModelCliente db = new ModelCliente();
        Metodos metodo = new Metodos();


        [Route("api/Compras/RealizarLecturaEmail")]

        public async System.Threading.Tasks.Task<HttpResponseMessage> GetRealizarLecturaEmailsAsync()
        {
            try
            {

                G G = new G();
                var Parametros = db.Parametros.FirstOrDefault();
                var Correos = db.CorreosRecepcion.ToList();

                foreach (var item in Correos)
                {


                    using (ImapClient client = new ImapClient(item.RecepcionHostName, (int)(item.RecepcionPort),
                               item.RecepcionEmail, item.RecepcionPassword, AuthMethod.Login, (bool)(item.RecepcionUseSSL)))
                    {
                        IEnumerable<uint> uids = client.Search(SearchCondition.Unseen());

                        DateTime recepcionUltimaLecturaImap = DateTime.Now;
                        if (item.RecepcionUltimaLecturaImap != null)
                            recepcionUltimaLecturaImap = item.RecepcionUltimaLecturaImap.Value;

                        uids.Concat(client.Search(SearchCondition.SentSince(recepcionUltimaLecturaImap)));

                        foreach (var uid in uids)
                        {
                            System.Net.Mail.MailMessage message = client.GetMessage(uid);

                            if (message.Attachments.Count > 0)
                            {
                                try
                                {
                                    byte[] ByteArrayPDF = null;
                                    int i = 1;

                                    decimal idGeneral = 0;
                                    foreach (var attachment in message.Attachments)
                                    {

                                        try
                                        {
                                            System.IO.StreamReader sr = new System.IO.StreamReader(attachment.ContentStream);



                                            string texto = sr.ReadToEnd();

                                            if (texto.Substring(0, 3) == "???")
                                                texto = texto.Substring(3);

                                            if (texto.Contains("PDF"))
                                            {

                                                ByteArrayPDF = ((MemoryStream)attachment.ContentStream).ToArray();
                                                //ByteArrayPDF = G.Zip(texto);


                                            }


                                            if (texto.Contains("FacturaElectronica")
                                                    || texto.Contains("NotaCreditoElectronica")
                                                    && !texto.Contains("TiqueteElectronico")

                                                    //  && !texto.Contains("NotaCreditoElectronica")
                                                    && !texto.Contains("NotaDebitoElectronica"))
                                            {
                                                var emailByteArray = G.Zip(texto);

                                                decimal id = db.Database.SqlQuery<decimal>("Insert Into BandejaEntrada(XmlFactura, Procesado, Asunto, Remitente,Pdf,impuestoAcreditar,gastoAplicable) " +
                                                        " VALUES (@EmailJson, 0, @Asunto, @Remitente, @Pdf,0,0); SELECT SCOPE_IDENTITY(); ",
                                                        new SqlParameter("@EmailJson", emailByteArray),
                                                        new SqlParameter("@Asunto", message.Subject),
                                                        new SqlParameter("@Remitente", message.From.ToString()),
                                                        new SqlParameter("@Pdf", (ByteArrayPDF == null ? new byte[0] : ByteArrayPDF))).First();
                                                idGeneral = id;
                                                try
                                                {

                                                    var datos = G.ObtenerDatosXmlRechazado(texto);
                                                    datos.NumeroConsecutivo = datos.NumeroConsecutivo.Trim();
                                                    var Detalle = db.BandejaEntrada.Where(a => a.NumeroConsecutivo == datos.NumeroConsecutivo && a.IdEmisor == datos.Numero).FirstOrDefault();
                                                    if(datos.IdReceptor == db.Sucursales.FirstOrDefault().Cedula && Detalle == null && !string.IsNullOrEmpty(datos.NumeroConsecutivo))
                                                    {
                                                        db.Database.ExecuteSqlCommand("Update BandejaEntrada set NumeroConsecutivo=@NumeroConsecutivo, " +
                                                       " TipoDocumento = @TipoDocumento, FechaEmision = @FechaEmision , " +
                                                       " NombreEmisor = @NombreEmisor,IdEmisor = @IdEmisor ,CodigoMoneda = @CodigoMoneda , " +
                                                       " TotalComprobante = @TotalComprobante, " +
                                                       " Impuesto = @TotalImpuesto, " +
                                                       " tipoIdentificacionEmisor = @EmisorId" +
                                                       " WHERE Id=@Id ",
                                                        new SqlParameter("@NumeroConsecutivo", datos.NumeroConsecutivo),
                                                        new SqlParameter("@TipoDocumento", datos.TipoDocumento),
                                                        new SqlParameter("@FechaEmision", datos.FechaEmision),
                                                        new SqlParameter("@NombreEmisor", datos.NombreEmisor),
                                                        new SqlParameter("@IdEmisor", datos.Numero),
                                                        new SqlParameter("@CodigoMoneda", datos.CodigoMoneda),
                                                        new SqlParameter("@TotalComprobante", datos.TotalComprobante),
                                                        new SqlParameter("@Id", id),
                                                        new SqlParameter("@TotalImpuesto", datos.Impuesto),
                                                        new SqlParameter("@EmisorId", datos.tipoIdentificacionEmisor));
                                                    }
                                                    else
                                                    {

                                                        db.Database.ExecuteSqlCommand("DELETE FROM BANDEJAENTRADA where Id=" + id);
                                                        throw new Exception("Este documento no es para este usuario o ya esta registrado");
                                                    }

                                                   
                                                }
                                                catch { }
                                            }

                                            if (i == message.Attachments.Count())
                                            {
                                                if (idGeneral > 0)
                                                {
                                                    var bandeja = db.BandejaEntrada.Where(a => a.Id == idGeneral).FirstOrDefault();

                                                    if (bandeja.Pdf.Count() == 0)
                                                    {
                                                        db.Database.ExecuteSqlCommand("Update BandejaEntrada set Pdf=@Pdf " +

                                                   " WHERE Id=@Id ",
                                                    new SqlParameter("@Pdf", ByteArrayPDF),

                                                    new SqlParameter("@Id", idGeneral));
                                                    }
                                                    bandeja = db.BandejaEntrada.Where(a => a.Id == idGeneral).FirstOrDefault();
                                                    db.Entry(bandeja).State = EntityState.Modified;
                                                    bandeja.impuestoAcreditar = 0;
                                                    bandeja.gastoAplicable = 0;
                                                    bandeja.CodigoActividad = db.Sucursales.FirstOrDefault().CodActividadComercial;
                                                    bandeja.XmlConfirmacion = G.GuardarPDF(ByteArrayPDF, bandeja.NumeroConsecutivo + "_" + bandeja.NombreEmisor);
                                                    db.SaveChanges();

                                                }
                                            }

                                            i++;
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

                                        }
                                    }
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

                                }
                            }
                            message.Dispose();

                            await System.Threading.Tasks.Task.Delay(100);
                        }
                        db.Entry(item).State = EntityState.Modified;
                        item.RecepcionUltimaLecturaImap = DateTime.Now;
                        db.SaveChanges();

                    }

                }

                
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {

                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
               
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();

                
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }



        [Route("api/Compras/Listado")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetListado([FromUri] Filtros filtro)
        {
            try
            {
                DateTime time = new DateTime();


                var Compras = db.BandejaEntrada.Where(a => (filtro.FechaInicial != time ? a.FechaIngreso >= filtro.FechaInicial : true)).ToList();

                if(filtro.FechaFinal != time)
                {
                    filtro.FechaFinal = filtro.FechaFinal.AddDays(1);
                    Compras = Compras.Where(a => a.FechaIngreso <= filtro.FechaFinal).ToList();
                }


                return Request.CreateResponse(HttpStatusCode.OK, Compras);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        public async System.Threading.Tasks.Task<HttpResponseMessage> GetAsync([FromUri] string DocNum, string ObjType = "18", string CodSucursal = "001")
        {
            var t = db.Database.BeginTransaction();
            try
            {
                Parametros parametros = db.Parametros.FirstOrDefault();

                var SQL = " ";
                var consecInterno = 0;

                SQL += parametros.FECEnc + DocNum + "  and t0.Series=" + parametros.SerieFEC;
                  
                var conexion = metodo.DevuelveCadena(CodSucursal);

                SqlConnection Cn = new SqlConnection(conexion);
                SqlCommand Cmd = new SqlCommand(SQL, Cn);
                SqlDataAdapter Da = new SqlDataAdapter(Cmd);
                DataSet Ds = new DataSet();
                Cn.Open();
                Da.Fill(Ds, "Encabezado");

                EncDocumento enc = new EncDocumento();
                enc.idSucursal = CodSucursal;
                enc.consecutivoSAP = DocNum;

                var Serie = Convert.ToInt32(Ds.Tables["Encabezado"].Rows[0]["Series"]);
                var Sucursal = db.Sucursales.Where(a => a.codSuc == CodSucursal).FirstOrDefault();
                var tipoDocumento = "08";

              

                var Factura = db.EncDocumento.Where(a => a.consecutivoSAP == DocNum && a.TipoDocumento == tipoDocumento && a.code == 1 && a.RespuestaHacienda.Contains("aprobado")).FirstOrDefault();

                if (Factura == null)
                {
                    if (metodo.ObtenerConfig("ValidarNumerico") == "1")
                    {
                        consecInterno = Sucursal.consecFEC;
                        db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                        Sucursal.consecFEC++;
                        db.SaveChanges();
                    }
                    else
                    {
                        consecInterno = Convert.ToInt32(DocNum) - Sucursal.consecFEC;

                    }



                    var DocEntry = Ds.Tables["Encabezado"].Rows[0]["DocEntry"].ToString();
                    enc.DocEntry = DocEntry;
                    enc.consecutivoInterno = consecInterno;
                    enc.TipoDocumento = tipoDocumento;
                    enc.Fecha = Convert.ToDateTime(Ds.Tables["Encabezado"].Rows[0]["DocDate"]);
                    enc.Fecha = enc.Fecha.Value.AddHours( DateTime.Now.Hour);
                    enc.Fecha = enc.Fecha.Value.AddMinutes(DateTime.Now.Minute);
                    enc.Fecha = enc.Fecha.Value.AddSeconds(DateTime.Now.Second);
                    enc.CardCode = Ds.Tables["Encabezado"].Rows[0]["CardCode"].ToString();
                    enc.CardName = Ds.Tables["Encabezado"].Rows[0]["NombreCliente"].ToString();
                    enc.LicTradNum = Ds.Tables["Encabezado"].Rows[0]["Identificacion"].ToString();
                    enc.LicTradNum = enc.LicTradNum.Replace("-", "");
                    enc.Email = Ds.Tables["Encabezado"].Rows[0]["Correo"].ToString();
                    enc.CodActividadEconomica = Ds.Tables["Encabezado"].Rows[0]["ActividadComercial"].ToString();

                    if (enc.LicTradNum.Length == 9)
                    {
                        enc.TipoIdentificacion = "01";

                    }
                    else if (enc.LicTradNum.Length == 10)
                    {
                        enc.TipoIdentificacion = "02";
                    }
                    else if (enc.LicTradNum.Length > 10)
                    {
                        enc.TipoIdentificacion = "03";
                    }
                    else
                    {
                        enc.TipoIdentificacion = null;
                    }

                    enc.condicionVenta = Ds.Tables["Encabezado"].Rows[0]["CondicionVenta"].ToString();
                    enc.condicionVenta = db.CondicionesVenta.Where(a => a.codSAP == enc.condicionVenta).FirstOrDefault().codCyber;
                    enc.plazoCredito = Convert.ToInt32(Ds.Tables["Encabezado"].Rows[0]["PlazoCredito"]);
                    enc.medioPago = Ds.Tables["Encabezado"].Rows[0]["MedioPago"].ToString();
                    enc.moneda = Ds.Tables["Encabezado"].Rows[0]["Moneda"].ToString();
                    enc.tipoCambio = Convert.ToDecimal(Ds.Tables["Encabezado"].Rows[0]["TipoCambio"]);
                    enc.RefNumeroDocumento = Ds.Tables["Encabezado"].Rows[0]["Referencia"].ToString();
                    enc.RefRazon = Ds.Tables["Encabezado"].Rows[0]["Comentario"].ToString();
                    enc.RefFechaEmision = DateTime.Now;
                    enc.procesadaHacienda = false;
                    db.EncDocumento.Add(enc);
                    db.SaveChanges();


                    try
                    {
                        if (!string.IsNullOrEmpty(Ds.Tables["Encabezado"].Rows[0]["OtrosTextos"].ToString()))
                        {
                            OtrosTextos ot = new OtrosTextos();
                            ot.idEncabezado = enc.id;
                            ot.codigo = "99";
                            ot.detalle = Ds.Tables["Encabezado"].Rows[0]["OtrosTextos"].ToString();
                            db.OtrosTextos.Add(ot);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {

                        BitacoraErrores be = new BitacoraErrores();
                        be.DocNum = DocNum;
                        be.Type = ObjType;
                        be.Descripcion = ex.Message;
                        be.StackTrace = ex.StackTrace;
                        be.Fecha = DateTime.Now;
                        db.BitacoraErrores.Add(be);
                        db.SaveChanges();
                    }

                    Cn.Close();


                    SQL = " ";

                    SQL += parametros.FECDet.Replace("@reemplazo", DocEntry) + " ";

                    List<DetDocumento> Detalles = new List<DetDocumento>();

                    Cn = new SqlConnection(conexion);
                    Cmd = new SqlCommand(SQL, Cn);
                    Da = new SqlDataAdapter(Cmd);
                    Ds = new DataSet();
                    Cn.Open();
                    Da.Fill(Ds, "Detalle");

                    decimal totalserviciosexonerado = 0;
                    decimal totalmercaderiasexoneradas = 0;
                    decimal totalsergravados = 0;
                    decimal totalmercaderiagravada = 0;
                    decimal totalservexentos = 0;
                    decimal totalmercexenta = 0;
                    var i = 1;
                    foreach (DataRow item in Ds.Tables["Detalle"].Rows)
                    {
                        DetDocumento det = new DetDocumento();
                        det.idEncabezado = enc.id;
                        det.NumLinea = i; //Convert.ToInt32(item["NumLinea"]);
                        det.partidaArancelaria = item["PartidaArancelaria"].ToString();
                        det.exportacion = Convert.ToDecimal(item["Exportacion"]);
                        det.partidaArancelaria = det.exportacion == 0 ? "" : det.partidaArancelaria;
                        det.CodCabys = item["CodigoCabys"].ToString();
                        //Discordia de modulo de tipo de codigo
                        det.tipoCod = item["tipoCod"].ToString();
                        det.codPro = item["CodPro"].ToString();
                        //Discordia
                        det.cantidad = Convert.ToDecimal(item["Cantidad"]) == 0 ? 1: Convert.ToDecimal(item["Cantidad"]);
                        det.unidadMedida = item["UnidadMedida"].ToString();
                        det.unidadMedida = db.UnidadesMedida.Where(a => a.codSAP == det.unidadMedida).FirstOrDefault().codCyber;
                        det.unidadMedidaComercial = item["UnidadMedida"].ToString();
                        det.unidadMedidaComercial = db.UnidadesMedida.Where(a => a.codSAP == det.unidadMedidaComercial).FirstOrDefault().Nombre;
                        det.NomPro = item["NomPro"].ToString();
                        det.PrecioUnitario = Convert.ToDecimal(item["PrecioUnitario"]);
                        det.MontoTotal = Math.Round(det.cantidad * det.PrecioUnitario, 2);
                        var desc = Convert.ToDecimal(item["PorDesc"]) / 100;
                        det.MontoDescuento = Math.Round(det.MontoTotal * desc, 2);
                        det.NaturalezaDescuento = string.IsNullOrEmpty(item["NaturalezaDescuento"].ToString()) ? "Descuento" : item["NaturalezaDescuento"].ToString();
                        det.SubTotal = Math.Round(det.MontoTotal - det.MontoDescuento, 2);
                        det.idImpuesto = item["idImpuesto"].ToString();
                        det.factorIVA = Convert.ToDecimal(item["FactorIVA"]);
                        det.baseImponible = Math.Round(det.SubTotal, 2);
                        var Impuesto = db.Impuestos.Where(a => a.id == det.idImpuesto).FirstOrDefault();
                        det.montoImpuesto = Math.Round(Convert.ToDecimal(det.SubTotal * (Impuesto.tarifa / 100)), 2);

                        var TaxOnly = item["TaxOnly"].ToString();
                        if (TaxOnly == "Y")
                        {
                            det.PrecioUnitario = 0;
                            det.MontoDescuento = 0;
                            det.SubTotal = 0;
                            //  det.baseImponible = det.SubTotal;
                            det.MontoTotal = 0;
                        }


                        if (!string.IsNullOrEmpty(item["DocumentoExoneracion"].ToString()))
                        {
                            if (Convert.ToInt32(item["DocumentoExoneracion"]) > 0)
                            {
                                try
                                {
                                    var SQL2 = " " + parametros.Exoneracion + Convert.ToInt32(item["DocumentoExoneracion"]) + " ";
                                    var Cn2 = new SqlConnection(conexion);
                                    var Cmd2 = new SqlCommand(SQL2, Cn2);
                                    var Da2 = new SqlDataAdapter(Cmd2);
                                    var Ds2 = new DataSet();
                                    Cn2.Open();
                                    Da2.Fill(Ds2, "Exoneracion");

                                    det.exonTipoDoc = Ds2.Tables["Exoneracion"].Rows[0]["TipoDocumento"].ToString();
                                    det.exonNumdoc = Ds2.Tables["Exoneracion"].Rows[0]["NumeroDocumento"].ToString();
                                    det.exonNomInst = Ds2.Tables["Exoneracion"].Rows[0]["Emisora"].ToString();
                                    det.exonFecEmi = Convert.ToDateTime(Ds2.Tables["Exoneracion"].Rows[0]["FechaEmision"]);
                                    var tipoImp = Ds2.Tables["Exoneracion"].Rows[0]["CodTarifa"].ToString();
                                    det.exonPorExo = Convert.ToInt32(Ds2.Tables["Exoneracion"].Rows[0]["Prcnt"].ToString().Substring(0, 2)); // Convert.ToInt32(db.Impuestos.Where(a => a.id == tipoImp).FirstOrDefault().tarifa.Value);

                                    det.exonMonExo = Math.Round((det.SubTotal * det.exonPorExo / 100), 2);

                                    Cn2.Close();
                                    decimal total = 0;
                                    if (det.unidadMedida == "Sp")
                                    {
                                        if (Impuesto.tarifa > 0)
                                        {
                                            if (det.exonPorExo > 0)
                                            {
                                                if (Impuesto.tarifa - det.exonPorExo < 0)
                                                {
                                                    totalserviciosexonerado += det.MontoTotal;
                                                    total = det.MontoTotal;
                                                }
                                                else
                                                {
                                                    totalserviciosexonerado += Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                                    total = Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                                }


                                                totalsergravados += det.MontoTotal + det.MontoDescuento - total;
                                            }
                                            else
                                            {
                                                totalsergravados += (1 - (det.exonPorExo / 100)) * det.MontoTotal;
                                            }
                                        }
                                        else
                                        {
                                            totalservexentos += det.MontoTotal;
                                        }
                                    }
                                    else
                                    {
                                        if (Impuesto.tarifa > 0)
                                        {
                                            if (det.exonPorExo > 0)
                                            {
                                                if (Impuesto.tarifa - det.exonPorExo < 0)
                                                {
                                                    totalmercaderiasexoneradas += det.MontoTotal;
                                                    total = det.MontoTotal;
                                                }
                                                else
                                                {
                                                    totalmercaderiasexoneradas += Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                                    total = Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                                }

                                                totalmercaderiagravada += det.MontoTotal - total;
                                            }
                                            else
                                            {
                                                totalmercaderiagravada += (1 - (det.exonPorExo / 100)) * det.MontoTotal;
                                            }
                                        }
                                        else
                                        {
                                            totalmercexenta += det.MontoTotal;
                                        }
                                    }


                                }
                                catch (Exception ex1)
                                {
                                    det.exonFecEmi = DateTime.Now;
                                    det.exonMonExo = 0;

                                }

                            }
                            else
                            {
                                det.exonFecEmi = DateTime.Now;
                                det.exonMonExo = 0;

                                decimal total = 0;
                                if (det.unidadMedida == "Sp")
                                {
                                    if (Impuesto.tarifa > 0)
                                    {
                                        if (det.exonPorExo > 0)
                                        {
                                            if (Impuesto.tarifa - det.exonPorExo < 0)
                                            {
                                                totalserviciosexonerado += det.MontoTotal;
                                                total = det.MontoTotal;
                                            }
                                            else
                                            {
                                                totalserviciosexonerado += Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                                total = Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                            }


                                            totalsergravados += det.MontoTotal + det.MontoDescuento - total;
                                        }
                                        else
                                        {
                                            totalsergravados += (1 - (det.exonPorExo / 100)) * det.MontoTotal;
                                        }
                                    }
                                    else
                                    {
                                        totalservexentos += det.MontoTotal;
                                    }
                                }
                                else
                                {
                                    if (Impuesto.tarifa > 0)
                                    {
                                        if (det.exonPorExo > 0)
                                        {
                                            if (Impuesto.tarifa - det.exonPorExo < 0)
                                            {
                                                totalmercaderiasexoneradas += det.MontoTotal;
                                                total = det.MontoTotal;
                                            }
                                            else
                                            {
                                                totalmercaderiasexoneradas += Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                                total = Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                            }

                                            totalmercaderiagravada += det.MontoTotal - total;
                                        }
                                        else
                                        {
                                            totalmercaderiagravada += (1 - (det.exonPorExo / 100)) * det.MontoTotal;
                                        }
                                    }
                                    else
                                    {
                                        totalmercexenta += det.MontoTotal;
                                    }
                                }

                            }
                        }
                        else
                        {
                            det.exonFecEmi = DateTime.Now;
                            det.exonMonExo = 0;

                            decimal total = 0;
                            if (det.unidadMedida == "Sp")
                            {
                                if (Impuesto.tarifa > 0)
                                {
                                    if (det.exonPorExo > 0)
                                    {
                                        if (Impuesto.tarifa - det.exonPorExo < 0)
                                        {
                                            totalserviciosexonerado += det.MontoTotal;
                                            total = det.MontoTotal;
                                        }
                                        else
                                        {
                                            totalserviciosexonerado += Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                            total = Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                        }


                                        totalsergravados += det.MontoTotal + det.MontoDescuento - total;
                                    }
                                    else
                                    {
                                        totalsergravados += (1 - (det.exonPorExo / 100)) * det.MontoTotal;
                                    }
                                }
                                else
                                {
                                    totalservexentos += det.MontoTotal;
                                }
                            }
                            else
                            {
                                if (Impuesto.tarifa > 0)
                                {
                                    if (det.exonPorExo > 0)
                                    {
                                        if (Impuesto.tarifa - det.exonPorExo < 0)
                                        {
                                            totalmercaderiasexoneradas += det.MontoTotal;
                                            total = det.MontoTotal;
                                        }
                                        else
                                        {
                                            totalmercaderiasexoneradas += Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                            total = Convert.ToDecimal(det.MontoTotal * (det.exonPorExo / Impuesto.tarifa));
                                        }

                                        totalmercaderiagravada += det.MontoTotal - total;
                                    }
                                    else
                                    {
                                        totalmercaderiagravada += (1 - (det.exonPorExo / 100)) * det.MontoTotal;
                                    }
                                }
                                else
                                {
                                    totalmercexenta += det.MontoTotal;
                                }
                            }

                        }


                        det.impNeto = det.montoImpuesto - det.exonMonExo;
                        det.totalLinea = det.SubTotal + det.impNeto;

                        db.DetDocumento.Add(det);
                        db.SaveChanges();
                        Detalles.Add(det);
                        i++;
                    }

                    db.Entry(enc).State = System.Data.Entity.EntityState.Modified;
                    // enc.totalserviciogravado = Math.Round(Detalles.Where(a => a.unidadMedida.ToLower() == "sp" && a.exonTipoDoc == null).Sum(d => d.MontoTotal), 2);
                    //enc.totalservicioexento = Math.Round(Detalles.Where(a => a.unidadMedida.ToLower() == "sp" && a.exonTipoDoc != null).Sum(d => d.MontoTotal), 2);
                    //enc.totalservicioexonerado = Math.Round(totalserviciosexonerado, 2);

                    enc.totalserviciogravado =  Math.Round(totalsergravados, 2);
                    enc.totalservicioexento = Math.Round(totalservexentos, 2);
                    enc.totalservicioexonerado = Math.Round(totalserviciosexonerado, 2);



                    // enc.totalmercaderiagravado = Math.Round(Detalles.Where(a => a.unidadMedida.ToLower() != "sp" && a.exonTipoDoc == null).Sum(d => d.MontoTotal), 2);
                    //enc.totalmercaderiaexenta = Math.Round(Detalles.Where(a => a.unidadMedida.ToLower() != "sp" && a.exonTipoDoc != null).Sum(d => d.MontoTotal), 2);
                    //enc.totalmercaderiaexonerado = Math.Round(totalmercaderiasexoneradas, 2);

                    enc.totalmercaderiagravado = Math.Round(totalmercaderiagravada, 2);
                    enc.totalmercaderiaexenta = Math.Round(totalmercexenta, 2);
                    enc.totalmercaderiaexonerado = Math.Round(totalmercaderiasexoneradas, 2);


                    enc.totalgravado = Math.Round((enc.totalserviciogravado + enc.totalmercaderiagravado).Value, 2);
                    enc.totalexento = Math.Round((enc.totalservicioexento + enc.totalmercaderiaexenta).Value, 2);
                    enc.totalexonerado = Math.Round((enc.totalservicioexonerado + enc.totalmercaderiaexonerado).Value, 2);

                    enc.totalventa = Math.Round((enc.totalgravado + enc.totalexento + enc.totalexonerado).Value, 2);
                    enc.totaldescuentos = Math.Round(Detalles.Sum(a => a.MontoDescuento), 2);
                    enc.totalventaneta = Math.Round((enc.totalventa - enc.totaldescuentos).Value, 2);
                    enc.totalimpuestos = Math.Round(Detalles.Sum(a => a.impNeto), 2);
                    enc.totalivadevuelto = 0; //Servicios de salud no aplicables
                    enc.totalotroscargos = Math.Round(db.OtrosCargos.Where(a => a.idEncabezado == enc.id).FirstOrDefault() == null ? 0 : db.OtrosCargos.Where(a => a.idEncabezado == enc.id).Sum(d => d.monto), 2);
                    enc.montoOtrosCargos = Math.Round(enc.totalotroscargos.Value, 2);
                    enc.totalcomprobante = Math.Round((enc.totalventaneta + enc.totalimpuestos + enc.totalotroscargos - enc.totalivadevuelto).Value, 2);

                    db.SaveChanges();




                    Cn.Close();





                    t.Commit();

                    //Crear el xml a enviar
                    MakeXML xml = metodo.RellenarXML(enc, Detalles.ToArray(), true);
                    HttpClient cliente = new HttpClient();

                    var httpContent = new StringContent(JsonConvert.SerializeObject(xml), Encoding.UTF8, "application/json");
                    cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        HttpResponseMessage response = await cliente.PutAsync(parametros.urlCyber, httpContent);

                        if (response.IsSuccessStatusCode)
                        {
                            response.Content.Headers.ContentType.MediaType = "application/json";
                            var resp = await response.Content.ReadAsAsync<respuesta>();



                            db.Entry(enc).State = System.Data.Entity.EntityState.Modified;
                            enc.procesadaHacienda = true;
                            enc.code = resp.code;
                            enc.RespuestaHacienda = resp.hacienda_mensaje;
                            enc.XMLFirmado = resp.data;
                            enc.ClaveHacienda = resp.clave;
                            enc.JSON = JsonConvert.SerializeObject(xml);
                            if (resp.clave != null)
                            {
                                if (resp.clave.Length > 3)
                                {
                                    enc.ConsecutivoHacienda = enc.ClaveHacienda.Substring(21, 20);

                                     
                                }
                            }
                            enc.ErrorCyber = resp.xml_error;

                            if (enc.code == 1)
                            {
                                //REspuesta de hacienda
                                cuerpoRespuesta cuerpo = new cuerpoRespuesta();
                                cuerpo.api_key = Sucursal.ApiKey;
                                cuerpo.clave = enc.ClaveHacienda;
                                HttpClient cliente2 = new HttpClient();

                                var httpContent2 = new StringContent(JsonConvert.SerializeObject(cuerpo), Encoding.UTF8, "application/json");
                                cliente2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                try
                                {
                                    HttpResponseMessage response2 = await cliente2.PostAsync(parametros.urlCyberRespHacienda, httpContent2);
                                    if (response2.IsSuccessStatusCode)
                                    {
                                        response2.Content.Headers.ContentType.MediaType = "application/json";
                                        var resp2 = await response2.Content.ReadAsAsync<respuestaHacienda>();

                                        if (resp2.data.ind_estado.Contains("aceptado"))
                                        {
                                            enc.RespuestaHacienda = resp2.data.ind_estado;
                                            enc.XMLFirmado = resp2.data.respuesta_xml;

                                           

                                        }
                                        else
                                        {
                                            enc.RespuestaHacienda = resp2.data.ind_estado;
                                            
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {


                                }

                                //
                            }



                            db.SaveChanges();
                      

                        }

                    }
                    catch (Exception ex)
                    {
                        BitacoraErrores be = new BitacoraErrores();
                        be.DocNum = DocNum;
                        be.Type = ObjType;
                        be.Descripcion = ex.Message;
                        be.StackTrace = ex.StackTrace;
                        be.Fecha = DateTime.Now;
                        db.BitacoraErrores.Add(be);
                        db.SaveChanges();
                    }

                }
                else
                {
                    //Se vuelve a enviar en el caso de que haya existido un error
                    t.Commit();

                    if (Factura.code != 1)
                    {
                        var DetFactura = db.DetDocumento.Where(a => a.idEncabezado == Factura.id).ToList();

                        MakeXML xml = metodo.RellenarXML(Factura, DetFactura.ToArray(), true);
                        HttpClient cliente = new HttpClient();

                        var httpContent = new StringContent(JsonConvert.SerializeObject(xml), Encoding.UTF8, "application/json");
                        cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        try
                        {
                            HttpResponseMessage response = await cliente.PutAsync(parametros.urlCyber, httpContent);

                            if (response.IsSuccessStatusCode)
                            {
                                response.Content.Headers.ContentType.MediaType = "application/json";
                                var resp = await response.Content.ReadAsAsync<respuesta>();
                                db.Entry(Factura).State = System.Data.Entity.EntityState.Modified;
                                Factura.procesadaHacienda = true;
                                Factura.code = resp.code;
                                Factura.RespuestaHacienda = resp.hacienda_mensaje;
                                Factura.XMLFirmado = resp.data; 
                                Factura.ClaveHacienda = resp.clave;
                                Factura.JSON = JsonConvert.SerializeObject(xml);

                                if (resp.clave != null)
                                {

                                    if (resp.clave.Length > 3)
                                    {
                                        Factura.ConsecutivoHacienda = Factura.ClaveHacienda.Substring(21, 20);
 
                                    }
                                }
                                Factura.ErrorCyber = resp.xml_error;

                                if (enc.code == 1)
                                {
                                    //REspuesta de hacienda
                                    cuerpoRespuesta cuerpo = new cuerpoRespuesta();
                                    cuerpo.api_key = Sucursal.ApiKey;
                                    cuerpo.clave = enc.ClaveHacienda;
                                    HttpClient cliente2 = new HttpClient();

                                    var httpContent2 = new StringContent(JsonConvert.SerializeObject(cuerpo), Encoding.UTF8, "application/json");
                                    cliente2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    try
                                    {
                                        HttpResponseMessage response2 = await cliente2.PostAsync(parametros.urlCyberRespHacienda, httpContent2);
                                        if (response2.IsSuccessStatusCode)
                                        {
                                            response2.Content.Headers.ContentType.MediaType = "application/json";
                                            var resp2 = await response2.Content.ReadAsAsync<respuestaHacienda>();

                                            if (resp2.data.ind_estado.Contains("aceptado"))
                                            {
                                                Factura.RespuestaHacienda = resp2.data.ind_estado;
                                                Factura.XMLFirmado = resp2.data.respuesta_xml;

                                             

                                            }
                                            else
                                            {
                                                Factura.RespuestaHacienda = resp2.data.ind_estado;
 




                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {


                                    }

                                    //
                                }

                                db.SaveChanges();
                                //product = await response.Content.ReadAsAsync<ListaOrdenes>();

                            }

                        }
                        catch (Exception ex)
                        {
                            BitacoraErrores be = new BitacoraErrores();
                            be.DocNum = DocNum;
                            be.Type = ObjType;
                            be.Descripcion = ex.Message;
                            be.StackTrace = ex.StackTrace;
                            be.Fecha = DateTime.Now;
                            db.BitacoraErrores.Add(be);
                            db.SaveChanges();
                        }
                    }

                }
               
                
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                try
                {
                    t.Rollback();
                }
                catch (Exception x)
                {

                
                }
               
                BitacoraErrores be = new BitacoraErrores();
                be.DocNum = DocNum;
                be.Type = ObjType;
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [Route("api/Compras/Correo")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetPruebaCorreo( )
        {
            try
            {

                G G = new G();
                var Parametros = db.Parametros.FirstOrDefault();
                var Correos = db.CorreosRecepcion.ToList();
                var mensaje = "";
                foreach (var item in Correos)
                {

                    try
                    {
                        using (ImapClient client = new ImapClient(item.RecepcionHostName, (int)(item.RecepcionPort),
                               item.RecepcionEmail, item.RecepcionPassword, AuthMethod.Login, (bool)(item.RecepcionUseSSL)))
                        {
                            mensaje += "\n " + item.RecepcionEmail + " -> Acceso Correcto";
                        }
                    }
                    catch (Exception ex)
                    {
                        mensaje += "\n " + item.RecepcionEmail + " -> " + ex.Message;


                    }


                }

                return Request.CreateResponse(HttpStatusCode.OK, mensaje);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }





    }

}