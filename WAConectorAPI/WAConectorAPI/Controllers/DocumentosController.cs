using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
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
    public class DocumentosController : ApiController
    {
        ModelCliente db = new ModelCliente();
        Metodos metodo = new Metodos();

        public async System.Threading.Tasks.Task<HttpResponseMessage> GetAsync( [FromUri] string DocNum, string ObjType = "13" ,string CodSucursal = "001", bool ND = false)
        {
            var t = db.Database.BeginTransaction();
            try
            {
                Parametros parametros = db.Parametros.FirstOrDefault();

                var SQL = " ";
                var consecInterno = 0;
                switch(ObjType)
                {
                    case "13":
                        {
                            if(!ND)
                            {

                                SQL += parametros.FETEEnc + DocNum + " ";
                            }
                            else
                            {
                                SQL += parametros.FETEEnc + DocNum + "  and t0.Series=" + parametros.SerieND;
                            }
                            
                            break;
                        }
                    case "14":
                        {
                            SQL += parametros.NCEnc + DocNum + " ";
                            break;
                        }
                }

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
                var tipoDocumento = "";
                if (metodo.ObtenerConfig("ValidarNumerico") == "1") //Si NO le importa el consecutivo
                {
                    if (ObjType == "13")
                    {
                        if (Serie == Convert.ToInt32(parametros.SerieFE))
                        {
                            tipoDocumento = "01";
                            consecInterno = Sucursal.consecFac.Value;
                            db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                            Sucursal.consecFac++;
                            db.SaveChanges();
                        }
                        else if (Serie == Convert.ToInt32(parametros.SerieTE))
                        {
                            tipoDocumento = "04";
                            consecInterno = Sucursal.consecTiq.Value;
                            db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                            Sucursal.consecTiq++;
                            db.SaveChanges();
                        }
                        else if (Serie == Convert.ToInt32(parametros.SerieND))
                        {
                            tipoDocumento = "02";
                            consecInterno = Sucursal.consecND.Value;
                            db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                            Sucursal.consecND++;
                            db.SaveChanges();
                        }
                        else if (Serie == Convert.ToInt32(parametros.SerieFEE))
                        {
                            tipoDocumento = "09";
                            consecInterno = Sucursal.consecFEE;
                            db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                            Sucursal.consecFEE++;
                            db.SaveChanges();
                        }
                    }
                    else if (ObjType == "14")
                    {
                        if (Serie == Convert.ToInt32(parametros.SerieNC))
                        {
                            tipoDocumento = "03";
                            consecInterno = Sucursal.consecNC.Value;
                            db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                            Sucursal.consecNC++;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (ObjType == "13")
                    {
                        if (Serie == Convert.ToInt32(parametros.SerieFE))
                        {
                            tipoDocumento = "01";
                            consecInterno = Convert.ToInt32(DocNum) - Sucursal.consecFac.Value;
                            
                        }
                        else if (Serie == Convert.ToInt32(parametros.SerieTE))
                        {
                            tipoDocumento = "04";
                            consecInterno = Convert.ToInt32(DocNum) - Sucursal.consecTiq.Value;
                           
                        }
                        else if (Serie == Convert.ToInt32(parametros.SerieND))
                        {
                            tipoDocumento = "02";
                            consecInterno = Convert.ToInt32(DocNum) - Sucursal.consecND.Value;
                            
                        }
                        else if (Serie == Convert.ToInt32(parametros.SerieFEE))
                        {
                            tipoDocumento = "09";
                            consecInterno = Convert.ToInt32(DocNum) - Sucursal.consecFEE;
                            
                        }
                    }
                    else if (ObjType == "14")
                    {
                        if (Serie == Convert.ToInt32(parametros.SerieNC))
                        {
                            tipoDocumento = "03";
                            consecInterno = Convert.ToInt32(DocNum) - Sucursal.consecNC.Value;
                             
                        }
                    }
                }
                    


                if(string.IsNullOrEmpty(tipoDocumento))
                {
                    throw new Exception("Este documento no es electronico");
                }

                var Factura = db.EncDocumento.Where(a => a.consecutivoSAP == DocNum && a.TipoDocumento == tipoDocumento && a.code == 1 && (a.RespuestaHacienda.Contains("aceptado") || a.RespuestaHacienda == null)).FirstOrDefault();
                if (Factura == null)
                {
                    var DocEntry = Ds.Tables["Encabezado"].Rows[0]["DocEntry"].ToString();
                    //Se toma el docentry del encabezado en la consulta
                    enc.DocEntry = DocEntry;

                    if (metodo.ObtenerConfig("ValidarNumerico") == "1") //Si NO le importa el consecutivo
                    {
                        //Preguntaremos si ya existe un documento creado con status != 1 y que el consecutivo de hacienda sea diferente de null
                        var Busqueda = db.EncDocumento.Where(a => a.consecutivoSAP == DocNum && a.TipoDocumento == tipoDocumento && a.code != 1 && a.code != 44).FirstOrDefault(); //probando si es  44 debe crear un consecutivo nuevo, porque esta malo el que tiene
                        if (Busqueda == null)
                        {
                            enc.consecutivoInterno = consecInterno; //Se toma el consecutivo  dependiendo el documento electronico a generar
                        }
                        else // No se genera un consecutivo nuevo por lo tanto no deberia de generar una sumatoria a la tabla de sucursales
                        {
                            switch (tipoDocumento)
                            {
                                case "01":
                                    {
                                        db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                                        Sucursal.consecFac--;
                                        db.SaveChanges();
                                        break;
                                    }
                                case "02":
                                    {
                                        db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                                        Sucursal.consecND--;
                                        db.SaveChanges();
                                        break;
                                    }
                                case "03":
                                    {
                                        db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                                        Sucursal.consecNC--;
                                        db.SaveChanges();
                                        break;
                                    }
                                case "04":
                                    {
                                        db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                                        Sucursal.consecTiq--;
                                        db.SaveChanges();
                                        break;
                                    }
                            }
                            enc.consecutivoInterno = Busqueda.consecutivoInterno; //Se toma el consecutivo  dependiendo el documento electronico a generar
                        }
                    }
                    else
                    {
                        enc.consecutivoInterno = consecInterno; //Se toma el consecutivo  dependiendo el documento electronico a generar

                    }




                    enc.TipoDocumento = tipoDocumento; //Tipo de documento a generar dependiendo la serie
                    enc.Fecha = Convert.ToDateTime(Ds.Tables["Encabezado"].Rows[0]["DocDate"]); // Se toma la fecha del comprobante y se le pone la hora actual
                    enc.Fecha = enc.Fecha.Value.AddHours(DateTime.Now.Hour);
                    enc.Fecha = enc.Fecha.Value.AddMinutes(DateTime.Now.Minute);
                    enc.Fecha = enc.Fecha.Value.AddSeconds(DateTime.Now.Second);
                    enc.CardCode = Ds.Tables["Encabezado"].Rows[0]["CardCode"].ToString();
                    enc.CardName = Ds.Tables["Encabezado"].Rows[0]["NombreCliente"].ToString();
                    enc.LicTradNum = Ds.Tables["Encabezado"].Rows[0]["Identificacion"].ToString();
                    enc.LicTradNum = enc.LicTradNum.Replace("-", "");
                    enc.Email = Ds.Tables["Encabezado"].Rows[0]["Correo"].ToString();
                    enc.CodActividadEconomica = Ds.Tables["Encabezado"].Rows[0]["ActividadEconomicaEmisor"].ToString();//Sucursal.CodActividadComercial;

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


                    try
                    {
                        var tipoIdentificacion = Ds.Tables["Encabezado"].Rows[0]["TipoIdentificacion"].ToString();
                        if(tipoIdentificacion == "EX")
                        {
                            enc.TipoIdentificacion = tipoIdentificacion;

                        }
                    }
                    catch (Exception)
                    {

                       
                    }

                    if(metodo.ObtenerConfig("ValidarCedula") == "1")
                    {
                        if(string.IsNullOrEmpty(enc.TipoIdentificacion) && enc.TipoDocumento == "01") //Preguntamos si viene vacio la identificacion para hacerlo tiquete
                        {
                            enc.TipoDocumento = "04";
                            db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                            Sucursal.consecFac--;
                            enc.consecutivoInterno = Sucursal.consecTiq;
                            Sucursal.consecTiq++;
                           db.SaveChanges();
                        }
                    }



                    enc.condicionVenta = Ds.Tables["Encabezado"].Rows[0]["CondicionVenta"].ToString(); //Se toma la condicion de venta de sap y se busca en la base de datos intermedia
                    enc.condicionVenta = db.CondicionesVenta.Where(a => a.codSAP == enc.condicionVenta).FirstOrDefault().codCyber;
                    enc.plazoCredito = Convert.ToInt32(Ds.Tables["Encabezado"].Rows[0]["PlazoCredito"]);
                    enc.medioPago = Ds.Tables["Encabezado"].Rows[0]["MedioPago"].ToString();
                    enc.moneda = Ds.Tables["Encabezado"].Rows[0]["Moneda"].ToString();
                    enc.tipoCambio = Convert.ToDecimal(Ds.Tables["Encabezado"].Rows[0]["TipoCambio"]);
                    enc.RefNumeroDocumento = Ds.Tables["Encabezado"].Rows[0]["Referencia"].ToString();
                    enc.RefRazon = Ds.Tables["Encabezado"].Rows[0]["Comentario"].ToString();
                    enc.RefFechaEmision = DateTime.Now;
                    enc.procesadaHacienda = false;
                    try
                    {
                        enc.Comentarios = G.Base64Encode( Ds.Tables["Encabezado"].Rows[0]["PDFComentario"].ToString());
                    }
                    catch (Exception)
                    {

                        enc.Comentarios = "";
                    }
                    


                    db.EncDocumento.Add(enc);
                    db.SaveChanges();

                    try
                    {
                        if (!string.IsNullOrEmpty(Ds.Tables["Encabezado"].Rows[0]["OC"].ToString()))
                        {
                            OtrosTextos ot = new OtrosTextos();
                            ot.idEncabezado = enc.id;
                            ot.codigo = "OC";
                            ot.detalle = Ds.Tables["Encabezado"].Rows[0]["OC"].ToString();
                            db.OtrosTextos.Add(ot);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception)
                    {

                        
                    }

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

                    switch (ObjType)
                    {
                        case "13":
                            {
                                SQL += parametros.FETEDet.Replace("@reemplazo", DocEntry) + " ";

                                break;
                            }
                        case "14":
                            {
                                SQL += parametros.NCDet.Replace("@reemplazo", DocEntry) + " ";
                                break;
                            }
                    }


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
                        det.NumLinea = i;//Convert.ToInt32(item["NumLinea"]);
                        if(enc.TipoDocumento == "09")
                        {
                            det.partidaArancelaria = item["PartidaArancelaria"].ToString();
                            var exp = item["idImpuesto"].ToString();
                            var ImpuestoE = db.Impuestos.Where(a => a.id == exp).FirstOrDefault();
                            det.exportacion = Math.Round(Convert.ToDecimal(det.SubTotal * (ImpuestoE.tarifa / 100)), 2);
                        }
                        else
                        {
                            det.partidaArancelaria = item["PartidaArancelaria"].ToString();
                            det.exportacion = Convert.ToDecimal(item["Exportacion"]);
                            det.partidaArancelaria = det.exportacion == 0 ? "" : det.partidaArancelaria;
                        }
                       
                        det.CodCabys = item["CodigoCabys"].ToString();
                        det.tipoCod = item["tipoCod"].ToString();
                        det.codPro = item["CodPro"].ToString();
                        det.cantidad =  Convert.ToDecimal(item["Cantidad"]) == 0 ? 1: Convert.ToDecimal(item["Cantidad"]);
                        det.unidadMedida = item["UnidadMedida"].ToString();
                        det.unidadMedida = db.UnidadesMedida.Where(a => a.codSAP == det.unidadMedida).FirstOrDefault().codCyber;
                        det.unidadMedidaComercial = item["UnidadMedida"].ToString();
                        det.unidadMedidaComercial = db.UnidadesMedida.Where(a => a.codSAP == det.unidadMedidaComercial).FirstOrDefault().Nombre;
                        det.NomPro = item["NomPro"].ToString();
                        det.PrecioUnitario = Math.Round(Convert.ToDecimal(item["PrecioUnitario"]),2);
                        det.MontoTotal = Math.Round(det.cantidad * det.PrecioUnitario, 2);
                        var desc = Convert.ToDecimal(item["PorDesc"]) / 100;
                        det.MontoDescuento = det.MontoTotal * desc < 0 ? 0 : Math.Round(det.MontoTotal * desc, 2);
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

                                    det.exonMonExo = Math.Round( ( det.SubTotal * det.exonPorExo / 100),2);

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
                                            totalmercaderiasexoneradas += Convert.ToDecimal(det.MontoTotal * (det.exonPorExo/Impuesto.tarifa ));
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

                    enc.totalserviciogravado = Math.Round(totalsergravados, 2);
                    enc.totalservicioexento = Math.Round(totalservexentos, 2);
                    enc.totalservicioexonerado = Math.Round(totalserviciosexonerado, 2);



                    // enc.totalmercaderiagravado = Math.Round(Detalles.Where(a => a.unidadMedida.ToLower() != "sp" && a.exonTipoDoc == null).Sum(d => d.MontoTotal), 2);
                    //enc.totalmercaderiaexenta = Math.Round(Detalles.Where(a => a.unidadMedida.ToLower() != "sp" && a.exonTipoDoc != null).Sum(d => d.MontoTotal), 2);
                    //enc.totalmercaderiaexonerado = Math.Round(totalmercaderiasexoneradas, 2);

                    enc.totalmercaderiagravado = Math.Round(totalmercaderiagravada, 2);
                    enc.totalmercaderiaexenta = Math.Round(totalmercexenta, 2);
                    enc.totalmercaderiaexonerado = Math.Round(totalmercaderiasexoneradas, 2);


                    enc.totalgravado = Math.Round((enc.totalserviciogravado + enc.totalmercaderiagravado).Value,2);
                    enc.totalexento = Math.Round ((enc.totalservicioexento + enc.totalmercaderiaexenta).Value , 2);
                    enc.totalexonerado = Math.Round( (enc.totalservicioexonerado + enc.totalmercaderiaexonerado).Value ,2);

                    enc.totalventa = Math.Round( (enc.totalgravado + enc.totalexento + enc.totalexonerado).Value ,2);
                    enc.totaldescuentos = Math.Round( Detalles.Sum(a => a.MontoDescuento),2);
                    enc.totalventaneta = Math.Round( (enc.totalventa - enc.totaldescuentos).Value ,2);
                    enc.totalimpuestos = Math.Round( Detalles.Sum(a => a.impNeto),2);
                    enc.totalivadevuelto = 0; //Servicios de salud no aplicables
                    enc.totalotroscargos = Math.Round(db.OtrosCargos.Where(a => a.idEncabezado == enc.id).FirstOrDefault() == null ? 0 : db.OtrosCargos.Where(a => a.idEncabezado == enc.id).Sum(d => d.monto), 2);
                    enc.montoOtrosCargos = Math.Round(enc.totalotroscargos.Value,2);
                    enc.totalcomprobante = Math.Round( (enc.totalventaneta + enc.totalimpuestos + enc.totalotroscargos - enc.totalivadevuelto ).Value,2);

                    if (enc.TipoDocumento == "03") //Si es nota de credito
                    {

                        var Encabezado = db.EncDocumento.Where(a => a.consecutivoSAP == enc.RefNumeroDocumento && !string.IsNullOrEmpty(a.ClaveHacienda)).FirstOrDefault();

                        if(Encabezado != null)
                        {
                            enc.RefTipoDocumento = Encabezado.TipoDocumento;
                            enc.RefFechaEmision = Encabezado.Fecha.Value;

                            try
                            {
                                if (string.IsNullOrEmpty(enc.RefRazon))
                                {

                                    if (Math.Abs((decimal)(enc.totalcomprobante - Encabezado.totalcomprobante)) < 1)
                                    {
                                        enc.RefCodigo = "01";
                                        enc.RefRazon = $"Anula documento electrónico { Encabezado.ClaveHacienda}";
                                    }
                                    else
                                    {
                                        enc.RefCodigo = "02";
                                        enc.RefRazon = $"Corrige monto documento electrónico { Encabezado.ClaveHacienda}";
                                    }
                                }
                                else
                                {
                                    if (Math.Abs((decimal)(enc.totalcomprobante - Encabezado.totalcomprobante)) < 1)
                                    {
                                        enc.RefCodigo = "01";

                                    }
                                    else
                                    {
                                        enc.RefCodigo = "02";

                                    }
                                }

                                enc.RefNumeroDocumento = Encabezado.ClaveHacienda;



                            }
                            catch (Exception pp)
                            {
                                enc.RefCodigo = "01";
                                enc.RefRazon = $"Anula documento electrónico { Encabezado.ClaveHacienda}";

                            }
                        }
                        else
                        {
                            if(enc.RefNumeroDocumento.Length < 20)
                            {
                                enc.RefTipoDocumento = "99";
                                enc.RefCodigo = "99";
                                enc.RefRazon = $"Nota de Credito Financiera { enc.RefNumeroDocumento}";
                            }
                            else
                            {
                                var DocEntry1 = enc.RefNumeroDocumento;
                                SQL = parametros.FETEEnc + enc.RefNumeroDocumento + " ";
                                SqlConnection CnR = new SqlConnection(conexion);
                                SqlCommand CmdR = new SqlCommand(SQL, CnR);
                                SqlDataAdapter DaR = new SqlDataAdapter(CmdR);
                                DataSet DsR = new DataSet();
                                CnR.Open();
                                DaR.Fill(DsR, "RefEnc");
                                enc.RefTipoDocumento = "01";
                                try
                                {
                                    enc.RefFechaEmision = Convert.ToDateTime(DsR.Tables["RefEnc"].Rows[0]["DocDate"]);
                                }
                                catch (Exception)
                                {


                                }


                                try
                                {
                                    enc.RefNumeroDocumento = Convert.ToString(DsR.Tables["RefEnc"].Rows[0]["ClaveHacienda"]);
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

                                enc.RefCodigo = "01";
                                enc.RefRazon = $"Anula documento electrónico { enc.RefNumeroDocumento}";



                                CnR.Close();
                            }
                            
                        }
                        

                    }



                    if (enc.TipoDocumento == "02") //Si es nota de debito
                    {

                        var Encabezado = db.EncDocumento.Where(a => a.consecutivoSAP == enc.RefNumeroDocumento && !string.IsNullOrEmpty(a.ClaveHacienda)).FirstOrDefault();
                        
                        if(Encabezado != null)
                        {
                            enc.RefTipoDocumento = Encabezado.TipoDocumento;
                            enc.RefFechaEmision = Encabezado.Fecha.Value;

                            try
                            {
                                if (string.IsNullOrEmpty(enc.RefRazon))
                                {

                                    if (Math.Abs((decimal)(enc.totalcomprobante - Encabezado.totalcomprobante)) < 1)
                                    {
                                        enc.RefCodigo = "01";
                                        enc.RefRazon = $"Anula documento electrónico { Encabezado.ClaveHacienda}";
                                    }
                                    else
                                    {
                                        enc.RefCodigo = "02";
                                        enc.RefRazon = $"Corrige monto documento electrónico { Encabezado.ClaveHacienda}";
                                    }
                                }
                                else
                                {
                                    if (Math.Abs((decimal)(enc.totalcomprobante - Encabezado.totalcomprobante)) < 1)
                                    {
                                        enc.RefCodigo = "01";

                                    }
                                    else
                                    {
                                        enc.RefCodigo = "02";

                                    }
                                }

                                enc.RefNumeroDocumento = Encabezado.ClaveHacienda;

                            }
                            catch (Exception pp)
                            {
                                enc.RefCodigo = "01";
                                enc.RefRazon = $"Anula documento electrónico { Encabezado.ClaveHacienda}";

                            }
                        }
                        else
                        {
                            if (enc.RefNumeroDocumento.Length < 20)
                            {
                                enc.RefTipoDocumento = "99";

                                enc.RefCodigo = "99";
                                enc.RefRazon = $"Nota de Debito Financiera { enc.RefNumeroDocumento}";
                            }
                            else
                            {
                                var DocEntry1 = enc.RefNumeroDocumento;
                                SQL = parametros.NCEnc + enc.RefNumeroDocumento + " ";
                                SqlConnection CnR = new SqlConnection(conexion);
                                SqlCommand CmdR = new SqlCommand(SQL, CnR);
                                SqlDataAdapter DaR = new SqlDataAdapter(CmdR);
                                DataSet DsR = new DataSet();
                                CnR.Open();
                                DaR.Fill(DsR, "RefEnc");
                                enc.RefTipoDocumento = "01";
                                try
                                {
                                    enc.RefFechaEmision = Convert.ToDateTime(DsR.Tables["RefEnc"].Rows[0]["DocDate"]);
                                }
                                catch (Exception)
                                {


                                }


                                try
                                {
                                    enc.RefNumeroDocumento = Convert.ToString(DsR.Tables["RefEnc"].Rows[0]["ClaveHacienda"]);
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

                                enc.RefCodigo = "01";
                                enc.RefRazon = $"Anula documento electrónico { enc.RefNumeroDocumento}";



                                CnR.Close();
                            }
                            
                        }
                            
                         
                       

                    }



                    db.SaveChanges();




                    Cn.Close();





                    t.Commit();

                    //Crear el xml a enviar
                    MakeXML xml = metodo.RellenarXML(enc, Detalles.ToArray());
                    HttpClient cliente = new HttpClient();

                    var httpContent = new StringContent(JsonConvert.SerializeObject(xml), Encoding.UTF8, "application/json");
                    cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        HttpResponseMessage response = await cliente.PostAsync(parametros.urlCyber, httpContent);

                        if (response.IsSuccessStatusCode)
                        {
                            response.Content.Headers.ContentType.MediaType = "application/json";
                            var resp = await response.Content.ReadAsAsync<respuesta>();

                          

                            db.Entry(enc).State = System.Data.Entity.EntityState.Modified;
                            enc.procesadaHacienda = true;
                            enc.code = resp.code;
                            enc.RespuestaHacienda = resp.hacienda_mensaje == null ? resp.xml_error : resp.hacienda_mensaje; //resp.hacienda_mensaje;
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

                            if(enc.code == 1)
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
                else
                {
                    if (metodo.ObtenerConfig("ValidarNumerico") == "1") //Si NO le importa el consecutivo
                    {
                        switch (tipoDocumento)
                        {
                            case "01":
                                {
                                    db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                                    Sucursal.consecFac--;
                                    db.SaveChanges();
                                    break;
                                }
                            case "02":
                                {
                                    db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                                    Sucursal.consecND--;
                                    db.SaveChanges();
                                    break;
                                }
                            case "03":
                                {
                                    db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                                    Sucursal.consecNC--;
                                    db.SaveChanges();
                                    break;
                                }
                            case "04":
                                {
                                    db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                                    Sucursal.consecTiq--;
                                    db.SaveChanges();
                                    break;
                                }
                        }
                    }
                       
                    //Se vuelve a enviar en el caso de que haya existido un error
                    t.Commit();
                    if( Factura.code != 1)
                    {
                        var DetFactura = db.DetDocumento.Where(a => a.idEncabezado == Factura.id).ToList();

                        MakeXML xml = metodo.RellenarXML(Factura, DetFactura.ToArray());
                        HttpClient cliente = new HttpClient();

                        var httpContent = new StringContent(JsonConvert.SerializeObject(xml), Encoding.UTF8, "application/json");
                        cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        try
                        {
                            HttpResponseMessage response = await cliente.PostAsync(parametros.urlCyber, httpContent);

                            if (response.IsSuccessStatusCode)
                            {
                                response.Content.Headers.ContentType.MediaType = "application/json";
                                var resp = await response.Content.ReadAsAsync<respuesta>();
                                db.Entry(Factura).State = System.Data.Entity.EntityState.Modified;
                                Factura.procesadaHacienda = true;
                                Factura.code = resp.code;
                                Factura.RespuestaHacienda = resp.hacienda_mensaje == null ? resp.xml_error : resp.hacienda_mensaje;// resp.hacienda_mensaje;
                                Factura.XMLFirmado = resp.data;
                                Factura.JSON = JsonConvert.SerializeObject(xml);
                                Factura.ClaveHacienda = resp.clave;
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


        [Route("api/Documentos/Consultar")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetOneAsync([FromUri] string Clave, string CodSucursal = "001")
        {
            try
            {
                var Sucursal = db.Sucursales.Where(a => a.codSuc == CodSucursal).FirstOrDefault();
                var parametros = db.Parametros.FirstOrDefault();
                //REspuesta de hacienda
                cuerpoRespuesta cuerpo = new cuerpoRespuesta();


                cuerpo.api_key = Sucursal.ApiKey;
                cuerpo.clave = Clave;
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

                        return Request.CreateResponse(HttpStatusCode.OK,resp2);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, response2.IsSuccessStatusCode);
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

                }

                //

                 
            }
            catch (Exception ex) 
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [Route("api/Documentos/ConsultarEstado")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetEstadoAsync()
        {
            var error = "";
            try
            {
                var Documentos1 = db.EncDocumento.Where(a =>   a.sincronizadaSAP == false).ToList();
                var Documentos2 = db.EncDocumento.Where(a => a.procesadaHacienda == false && a.code == 0).ToList();


                var parametros = db.Parametros.FirstOrDefault();
                Metodos metodo = new Metodos();

                foreach(var item in Documentos2)
                {
                    try
                    {
                        if (item.code != 1)
                        {
                            var Sucursal = db.Sucursales.Where(a => a.codSuc == item.idSucursal).FirstOrDefault();
                            var DetFactura = db.DetDocumento.Where(a => a.idEncabezado == item.id).ToList();

                            MakeXML xml = metodo.RellenarXML(item, DetFactura.ToArray());
                            HttpClient cliente = new HttpClient();

                            var httpContent = new StringContent(JsonConvert.SerializeObject(xml), Encoding.UTF8, "application/json");
                            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            try
                            {
                                HttpResponseMessage response = await cliente.PostAsync(parametros.urlCyber, httpContent);

                                if (response.IsSuccessStatusCode)
                                {
                                    response.Content.Headers.ContentType.MediaType = "application/json";
                                    var resp = await response.Content.ReadAsAsync<respuesta>();
                                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                    item.procesadaHacienda = true;
                                    item.code = resp.code;
                                    item.RespuestaHacienda = resp.hacienda_mensaje == null ? resp.xml_error : resp.hacienda_mensaje; //resp.hacienda_mensaje;
                                    item.XMLFirmado = resp.data;
                                    item.JSON = JsonConvert.SerializeObject(xml);
                                    item.ClaveHacienda = resp.clave;
                                    if (resp.clave != null)
                                    {
                                        if (resp.clave.Length > 3)
                                        {
                                            item.ConsecutivoHacienda = item.ClaveHacienda.Substring(21, 20);



                                        }


                                    }
                                    item.ErrorCyber = resp.xml_error;

                                    if (item.code == 1)
                                    {
                                        //REspuesta de hacienda
                                        cuerpoRespuesta cuerpo = new cuerpoRespuesta();
                                        cuerpo.api_key = Sucursal.ApiKey;
                                        cuerpo.clave = item.ClaveHacienda;
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
                                                    item.RespuestaHacienda = resp2.data.ind_estado;
                                                    item.XMLFirmado = resp2.data.respuesta_xml;


                                                }
                                                else
                                                {
                                                    item.RespuestaHacienda = resp2.data.ind_estado;

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

                                }else
                                {
                                    BitacoraErrores be = new BitacoraErrores();
                                    be.DocNum = item.DocEntry;
                                    be.Type = item.TipoDocumento;
                                    be.Descripcion = JsonConvert.SerializeObject(response);
                                    be.StackTrace = JsonConvert.SerializeObject(xml);
                                    be.Fecha = DateTime.Now;
                                    db.BitacoraErrores.Add(be);
                                    db.SaveChanges();
                                }

                            }
                            catch (Exception ex)
                            {
                                BitacoraErrores be = new BitacoraErrores();
                                be.DocNum = item.DocEntry;
                                be.Type = item.TipoDocumento;
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
                        be.DocNum = item.consecutivoSAP;
                        be.Type = item.TipoDocumento;
                        be.Descripcion = ex.Message;
                        be.StackTrace = ex.StackTrace;
                        be.Fecha = DateTime.Now;
                        db.BitacoraErrores.Add(be);
                        db.SaveChanges();
                    }
                }

                //Poner el consecutivo en SAP
                foreach(var item in Documentos1)
                {
                    if(!string.IsNullOrEmpty(item.ClaveHacienda))
                    {
                        if (item.ClaveHacienda.Length > 3)
                        {

                            try
                            {
                                if (metodo.ObtenerConfig("HANA") == "0")
                                {
                                    var Cn4 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                    var Cmd4 = new SqlCommand();

                                    Cn4.Open();

                                    Cmd4.Connection = Cn4;
                                    if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                    {

                                        Cmd4.CommandText = " Update OINV set " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                                    }
                                    else if (item.TipoDocumento == "03")
                                    {
                                        Cmd4.CommandText = " Update ORIN set " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                                    }
                                    else if (item.TipoDocumento == "08")
                                    {

                                        Cmd4.CommandText = " Update OPCH set " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                                    }


                                    Cmd4.ExecuteNonQuery();
                                    Cn4.Close();
                                    Cn4.Dispose();
                                }
                                else
                                {
                                    var Cn4 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                    var Cmd4 = new SqlCommand();

                                    Cn4.Open();

                                    Cmd4.Connection = Cn4;
                                    if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                    {

                                        Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARFACTURA(" + item.DocEntry + ", ''P'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";
                                        error = Cmd4.CommandText;
                                    }
                                    else if (item.TipoDocumento == "03")
                                    {
                                        Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARNOTA(" + item.DocEntry + ", ''P'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                    }
                                    else if (item.TipoDocumento == "08")
                                    {
                                        Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARCOMPRA(" + item.DocEntry + ", ''P'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";
 
                                    }


                                    Cmd4.ExecuteNonQuery();
                                    Cn4.Close();
                                    Cn4.Dispose();
                                }
                                   

                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                item.RespuestaHacienda = "procesando";
                                item.sincronizadaSAP = true;
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                BitacoraErrores be = new BitacoraErrores();
                                be.DocNum = item.consecutivoSAP;
                                be.Type = item.TipoDocumento;
                                be.Descripcion = ex.Message + " " + error ;
                                be.StackTrace = ex.StackTrace;
                                be.Fecha = DateTime.Now;
                                db.BitacoraErrores.Add(be);
                                db.SaveChanges();

                            }



                        }
                    }
                    
                }


                var Documentos = db.EncDocumento.Where(a => a.RespuestaHacienda.ToLower().Contains("procesando") && !a.XMLFirmado.ToLower().Contains("Error".ToLower())).ToList();

                if(parametros.urlCyberRespHacienda.ToLower().Contains("consultarespuestahacienda") )
                {
                    //Verificar Estado de hacienda
                    foreach (var item in Documentos)
                    {


                        //REspuesta de hacienda
                        cuerpoRespuesta cuerpo = new cuerpoRespuesta();

                        var Suc = db.Sucursales.Where(a => a.codSuc == item.idSucursal).FirstOrDefault();
                        cuerpo.api_key = Suc.ApiKey;
                        cuerpo.clave = item.ClaveHacienda;
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

                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                if (resp2.data.ind_estado.Contains("aceptado"))
                                {

                                    item.RespuestaHacienda = resp2.data.ind_estado;
                                    item.XMLFirmado = resp2.data.respuesta_xml;
                                    item.code = resp2.code;
                                    db.SaveChanges();
                                    if (metodo.ObtenerConfig("HANA") == "0")
                                    {
                                        var Cn5 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                        var Cmd5 = new SqlCommand();

                                        Cn5.Open();

                                        Cmd5.Connection = Cn5;
                                        if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                        {

                                            Cmd5.CommandText = " Update OINV set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";

                                        }
                                        else if (item.TipoDocumento == "03")
                                        {
                                            Cmd5.CommandText = " Update ORIN set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                                        }
                                        else if (item.TipoDocumento == "08")
                                        {
                                            Cmd5.CommandText = " Update OPCH set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";
                                        }

                                        Cmd5.ExecuteNonQuery();
                                        Cn5.Close();
                                        Cn5.Dispose();
                                    }
                                    else
                                    {
                                        var Cn4 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                        var Cmd4 = new SqlCommand();

                                        Cn4.Open();

                                        Cmd4.Connection = Cn4;
                                        if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                        {

                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARFACTURA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";
                                            error = Cmd4.CommandText;
                                        }
                                        else if (item.TipoDocumento == "03")
                                        {
                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARNOTA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                        }
                                        else if (item.TipoDocumento == "08")
                                        {
                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARCOMPRA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                        }


                                        Cmd4.ExecuteNonQuery();
                                        Cn4.Close();
                                        Cn4.Dispose();
                                    }

                                }
                                else
                                {
                                    item.RespuestaHacienda = resp2.data.ind_estado;
                                    if (metodo.ObtenerConfig("HANA") == "0")
                                    {
                                        var Cn5 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                        var Cmd5 = new SqlCommand();

                                        Cn5.Open();

                                        Cmd5.Connection = Cn5;
                                        if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                        {

                                            Cmd5.CommandText = " Update OINV set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";

                                        }
                                        else if (item.TipoDocumento == "03")
                                        {
                                            Cmd5.CommandText = " Update ORIN set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                                        }
                                        else if (item.TipoDocumento == "08")
                                        {
                                            Cmd5.CommandText = " Update OPCH set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";
                                        }

                                        Cmd5.ExecuteNonQuery();
                                        Cn5.Close();
                                        Cn5.Dispose();
                                    }
                                    else
                                    {
                                        var Cn4 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                        var Cmd4 = new SqlCommand();

                                        Cn4.Open();

                                        Cmd4.Connection = Cn4;


                                        if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                        {

                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARFACTURA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";
                                            error = Cmd4.CommandText;
                                        }
                                        else if (item.TipoDocumento == "03")
                                        {
                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARNOTA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                        }
                                        else if (item.TipoDocumento == "08")
                                        {
                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARCOMPRA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                        }





                                        Cmd4.ExecuteNonQuery();
                                        Cn4.Close();
                                        Cn4.Dispose();
                                    }

                                }
                                db.SaveChanges();
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            BitacoraErrores be = new BitacoraErrores();
                            be.DocNum = "";
                            be.Type = "";
                            be.Descripcion = ex.Message + " " + error;
                            be.StackTrace = ex.StackTrace;
                            be.Fecha = DateTime.Now;
                            db.BitacoraErrores.Add(be);
                            db.SaveChanges();

                        }

                        //
                    }
                }
                else
                {
                    //Verificar Estado de hacienda con otro url
                    foreach (var item in Documentos)
                    {


                        //REspuesta de hacienda
                        cuerpoRespuesta cuerpo = new cuerpoRespuesta();

                        var Suc = db.Sucursales.Where(a => a.codSuc == item.idSucursal).FirstOrDefault();
                        cuerpo.api_key = Suc.ApiKey;
                        cuerpo.clave = item.ClaveHacienda;
                        HttpClient cliente2 = new HttpClient();

                        var httpContent2 = new StringContent(JsonConvert.SerializeObject(cuerpo), Encoding.UTF8, "application/json");
                        cliente2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        try
                        {
                            HttpResponseMessage response2 = await cliente2.PostAsync(parametros.urlCyberRespHacienda, httpContent2);
                            if (response2.IsSuccessStatusCode)
                            {
                                response2.Content.Headers.ContentType.MediaType = "application/json";
                                var resp2 = await response2.Content.ReadAsAsync<HaciendaRespuestaOtrolink>();

                                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                if (resp2.hacienda_result.ind_estado.Contains("aceptado"))
                                {

                                    item.RespuestaHacienda = resp2.hacienda_result.ind_estado;
                                    item.XMLFirmado = resp2.hacienda_result.respuesta_xml;
                                    item.code = resp2.code;
                                    db.SaveChanges();
                                    if (metodo.ObtenerConfig("HANA") == "0")
                                    {
                                        var Cn5 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                        var Cmd5 = new SqlCommand();

                                        Cn5.Open();

                                        Cmd5.Connection = Cn5;
                                        if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                        {

                                            Cmd5.CommandText = " Update OINV set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";

                                        }
                                        else if (item.TipoDocumento == "03")
                                        {
                                            Cmd5.CommandText = " Update ORIN set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                                        }
                                        else if (item.TipoDocumento == "08")
                                        {
                                            Cmd5.CommandText = " Update OPCH set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";
                                        }

                                        Cmd5.ExecuteNonQuery();
                                        Cn5.Close();
                                        Cn5.Dispose();
                                    }
                                    else
                                    {
                                        var Cn4 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                        var Cmd4 = new SqlCommand();

                                        Cn4.Open();

                                        Cmd4.Connection = Cn4;
                                        if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                        {

                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARFACTURA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";
                                            error = Cmd4.CommandText;
                                        }
                                        else if (item.TipoDocumento == "03")
                                        {
                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARNOTA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                        }
                                        else if (item.TipoDocumento == "08")
                                        {
                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARCOMPRA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                        }


                                        Cmd4.ExecuteNonQuery();
                                        Cn4.Close();
                                        Cn4.Dispose();
                                    }

                                }
                                else
                                {
                                    item.RespuestaHacienda = resp2.hacienda_result.ind_estado;
                                    if (metodo.ObtenerConfig("HANA") == "0")
                                    {
                                        var Cn5 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                        var Cmd5 = new SqlCommand();

                                        Cn5.Open();

                                        Cmd5.Connection = Cn5;
                                        if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                        {

                                            Cmd5.CommandText = " Update OINV set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";

                                        }
                                        else if (item.TipoDocumento == "03")
                                        {
                                            Cmd5.CommandText = " Update ORIN set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                                        }
                                        else if (item.TipoDocumento == "08")
                                        {
                                            Cmd5.CommandText = " Update OPCH set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";
                                        }

                                        Cmd5.ExecuteNonQuery();
                                        Cn5.Close();
                                        Cn5.Dispose();
                                    }
                                    else
                                    {
                                        var Cn4 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                                        var Cmd4 = new SqlCommand();

                                        Cn4.Open();

                                        Cmd4.Connection = Cn4;


                                        if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                                        {

                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARFACTURA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";
                                            error = Cmd4.CommandText;
                                        }
                                        else if (item.TipoDocumento == "03")
                                        {
                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARNOTA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                        }
                                        else if (item.TipoDocumento == "08")
                                        {
                                            Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARCOMPRA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                                        }





                                        Cmd4.ExecuteNonQuery();
                                        Cn4.Close();
                                        Cn4.Dispose();
                                    }

                                }
                                db.SaveChanges();
                            }
                            else
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            BitacoraErrores be = new BitacoraErrores();
                            be.DocNum = "";
                            be.Type = "";
                            be.Descripcion = ex.Message + " " + error;
                            be.StackTrace = ex.StackTrace;
                            be.Fecha = DateTime.Now;
                            db.BitacoraErrores.Add(be);
                            db.SaveChanges();

                        }

                        //
                    }
                }
                




                return Request.CreateResponse(HttpStatusCode.OK);

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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }

        [Route("api/Documentos/Respuesta")]
        [HttpPost]
        public async System.Threading.Tasks.Task<HttpResponseMessage> PostAsync([FromBody] data datos)
        {

            try
            {

                var item = db.EncDocumento.Where(a => a.ClaveHacienda == datos.clave).FirstOrDefault();
                var parametros = db.Parametros.FirstOrDefault();
                if(item != null)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    item.RespuestaHacienda = datos.ind_estado;

                    if(datos.ind_estado.Contains("aceptado"))
                    {

                        if(metodo.ObtenerConfig("HANA") == "0")
                        {
                            var Cn5 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                            var Cmd5 = new SqlCommand();

                            Cn5.Open();

                            Cmd5.Connection = Cn5;
                            if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                            {

                                Cmd5.CommandText = " Update OINV set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";

                            }
                            else if (item.TipoDocumento == "03")
                            {
                                Cmd5.CommandText = " Update ORIN set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                            }
                            else if (item.TipoDocumento == "08")
                            {
                                Cmd5.CommandText = " Update OPCH set " + parametros.CampoEstado + " = 'A', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";
                            }

                            Cmd5.ExecuteNonQuery();
                            Cn5.Close();
                            Cn5.Dispose();
                        }
                        else
                        {
                            var Cn4 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                            var Cmd4 = new SqlCommand();

                            Cn4.Open();

                            Cmd4.Connection = Cn4;
                            if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                            {

                                Cmd4.CommandText = " EXEC ('call \""+ metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARFACTURA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";
                         
                            }
                            else if (item.TipoDocumento == "03")
                            {
                                Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARNOTA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                            }
                            else if (item.TipoDocumento == "08")
                            {
                                Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARCOMPRA(" + item.DocEntry + ", ''A'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                            }


                            Cmd4.ExecuteNonQuery();
                            Cn4.Close();
                            Cn4.Dispose();
                        }
                       
                        item.sincronizadaSAP = true;
                    }
                    else
                    {
                        if (metodo.ObtenerConfig("HANA") == "0")
                        {
                            var Cn5 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                            var Cmd5 = new SqlCommand();

                            Cn5.Open();

                            Cmd5.Connection = Cn5;
                            if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                            {

                                Cmd5.CommandText = " Update OINV set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";

                            }
                            else if (item.TipoDocumento == "03")
                            {
                                Cmd5.CommandText = " Update ORIN set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "' where DocEntry = '" + item.DocEntry + "' ";

                            }
                            else if (item.TipoDocumento == "08")
                            {
                                Cmd5.CommandText = " Update OPCH set " + parametros.CampoEstado + " = 'R', " + parametros.CampoClave + " = '" + item.ClaveHacienda + "', " + parametros.CampoConsecutivo + " = '" + item.ConsecutivoHacienda + "'  where DocEntry = '" + item.DocEntry + "' ";
                            }

                            Cmd5.ExecuteNonQuery();
                            Cn5.Close();
                            Cn5.Dispose();
                        }
                        else
                        {
                            var Cn4 = new SqlConnection(metodo.DevuelveCadena(item.idSucursal));
                            var Cmd4 = new SqlCommand();

                            Cn4.Open();

                            Cmd4.Connection = Cn4;
                            if (item.TipoDocumento != "08" && item.TipoDocumento != "03")
                            {

                                Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARFACTURA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";
                                 
                            }
                            else if (item.TipoDocumento == "03")
                            {
                                Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARNOTA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                            }
                            else if (item.TipoDocumento == "08")
                            {
                                Cmd4.CommandText = " EXEC ('call \"" + metodo.ObtenerConfig("TablaIntermedia") + "\".ACTUALIZARCOMPRA(" + item.DocEntry + ", ''R'', " + item.ConsecutivoHacienda + ", " + item.ClaveHacienda + " )') AT SAPHANA";

                            }


                            Cmd4.ExecuteNonQuery();
                            Cn4.Close();
                            Cn4.Dispose();
                        }
                         
                        item.sincronizadaSAP = true;
                    }

                    db.SaveChanges();
                }
                else
                {
                    var ClaveRespuesta = datos.clave.Split('-');
                    string Clave = ClaveRespuesta[0];
                    var bandeja = db.BandejaEntrada.Where(a => a.ClaveReceptor == Clave).FirstOrDefault();

                    if(bandeja != null)
                    {
                        db.Entry(bandeja).State = System.Data.Entity.EntityState.Modified;
                        bandeja.XMLRespuesta = datos.respuesta_xml;
                        bandeja.RespuestaHacienda = datos.ind_estado;
                        bandeja.ConsecutivoReceptor = ClaveRespuesta[1];
                        db.SaveChanges();
                    }
                    else
                    {
                        BitacoraErrores be = new BitacoraErrores();
                        be.DocNum = "";
                        be.Type = "";
                        be.Descripcion = "No se encontro " + JsonConvert.SerializeObject(datos) ;
                        be.StackTrace = "Respuesta";
                        be.Fecha = DateTime.Now;
                        db.BitacoraErrores.Add(be);
                        db.SaveChanges();
                    }
                }

           
                

                return Request.CreateResponse(HttpStatusCode.OK);
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
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);

            }
        }


       

    }




    
}