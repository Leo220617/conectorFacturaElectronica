﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WAConectorAPI.Models;
using WAConectorAPI.Models.Apis;
using WAConectorAPI.Models.ModelCliente;
using clave = WAConectorAPI.Models.Apis.clave;
using emisor = WAConectorAPI.Models.Apis.emisor;

namespace WAConectorAPI.Controllers
{
    [Authorize]
    public class AceptacionController : ApiController
    {
        ModelCliente db = new ModelCliente();
        Metodos metodo = new Metodos();
        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {
                DateTime time = new DateTime();
                //if (filtro.FechaFinal != time)
                //{
                //    filtro.FechaFinal = filtro.FechaFinal.AddHours(23).AddMinutes(58) ;

                //}

                var Compras = db.BandejaEntrada.Select(a => new {
                    a.Id,
                    a.NumeroConsecutivo,
                    a.TipoDocumento,
                    a.IdEmisor,
                    a.NombreEmisor,
                    a.FechaIngreso,
                    a.FechaEmision,
                    a.CodigoMoneda,
                    a.TotalComprobante,
                    a.Impuesto,
                    a.tipo,
                    a.DetalleMensaje,
                    a.RespuestaHacienda,
                    a.idAceptador,
                    a.Procesado,
                    a.XMLRespuesta,
                    a.XmlConfirmacion ,
                    a.IVA1,
                    a.IVA2,
                    a.IVA4,
                    a.IVA8,
                    a.IVA13
                }

                    ).Where(a => (filtro.FechaInicial != time ? a.FechaIngreso >= filtro.FechaInicial : true) 
                && (filtro.FechaFinal != time ? a.FechaIngreso <= filtro.FechaFinal : true) 
                && a.XmlConfirmacion != null 
                && (!string.IsNullOrEmpty(filtro.Estado) ? (filtro.Estado != "NULL" ? a.Procesado == filtro.Estado  : true) : true)
                && (filtro.Codigo1 > 0 ? a.idAceptador == filtro.Codigo1  : true)
                && (filtro.CodMoneda != "NULL" && !string.IsNullOrEmpty(filtro.CodMoneda) ?  a.CodigoMoneda.Contains(filtro.CodMoneda) : true)
                && (!string.IsNullOrEmpty(filtro.Texto) ? a.TipoDocumento == filtro.Texto : true)
                ).ToList();


                
                

                return Request.CreateResponse(HttpStatusCode.OK, Compras);

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

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [Route("api/Aceptacion/Consultar")]
        public HttpResponseMessage GetOne([FromUri]int id)
        {
            try
            {



                var Documentos = db.BandejaEntrada.Where(a => a.Id == id).FirstOrDefault();


                if (Documentos == null)
                {
                    throw new Exception("Este documento no se encuentra registrado");
                }

                return Request.CreateResponse(HttpStatusCode.OK, Documentos);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync([FromBody] BandejaEntrada bandeja)
        {
            var t = db.Database.BeginTransaction();
            try
            {

                if(string.IsNullOrEmpty(bandeja.tipo) || bandeja.tipo == "0" || bandeja.CondicionImpuesto == "0" || string.IsNullOrEmpty(bandeja.CondicionImpuesto) || string.IsNullOrEmpty(bandeja.CodigoActividad))
                {
                    throw new Exception("Faltan datos por rellenar");
                }
                Parametros parametros = db.Parametros.FirstOrDefault();

                var Bandeja = db.BandejaEntrada.Where(a => a.Id == bandeja.Id).FirstOrDefault();

                MakeXMLAceptacionFactura xml = new MakeXMLAceptacionFactura();
                var Sucursal = db.Sucursales.FirstOrDefault();

                xml.api_key = Sucursal.ApiKey;
                //Generacion del nodo clave
                xml.clave = new clave();
                xml.clave.tipo = bandeja.tipo;
                xml.clave.sucursal = Sucursal.codSuc;
                xml.clave.terminal = Sucursal.Terminal;
                xml.clave.numero_documento = Bandeja.NumeroConsecutivo;
                xml.clave.numero_cedula_emisor = Bandeja.IdEmisor;
                xml.clave.fecha_emision_doc = Bandeja.FechaEmision.Substring(6, 4) + "-" + Bandeja.FechaEmision.Substring(3, 2) + "-" + Bandeja.FechaEmision.Substring(0, 2) + "T12:00:00-06:00";// DateTime.Parse(Bandeja.FechaEmision).ToString("yyyy-MM-ddThh:mm:ss-06:00");
                xml.clave.mensaje = bandeja.Mensaje;
                xml.clave.detalle_mensaje = bandeja.DetalleMensaje;
                xml.clave.codigo_actividad = bandeja.CodigoActividad;
                xml.clave.condicion_impuesto = bandeja.CondicionImpuesto;
                xml.clave.impuesto_acreditar = Math.Round(bandeja.impuestoAcreditar).ToString().Replace(",", ".");  
                xml.clave.gasto_aplicable = Math.Round(bandeja.gastoAplicable).ToString().Replace(",", ".");
                xml.clave.monto_total_impuesto = Math.Round(Bandeja.Impuesto).ToString().Replace(",", ".");
                xml.clave.total_factura = Math.Round(Bandeja.TotalComprobante.Value).ToString().Replace(",", ".");
                xml.clave.numero_cedula_receptor = Sucursal.Cedula;
                xml.clave.num_consecutivo_receptor = Sucursal.consecAFC.ToString().Replace(",", ".");

                db.Entry(Sucursal).State = System.Data.Entity.EntityState.Modified;
                Sucursal.consecAFC += 1;
                db.SaveChanges();

                xml.clave.situacion_presentacion = (DateTime.Now.Date - DateTime.Parse( xml.clave.fecha_emision_doc)).TotalDays == 0 ? "1" : "3";
                xml.clave.codigo_seguridad = metodo.GeneraNumero();

                //Generacion del nodo emisor

                xml.emisor = new emisor();
                xml.emisor.identificacion = new Models.Apis.identificacion();
                xml.emisor.identificacion.tipo = Bandeja.tipoIdentificacionEmisor;
                xml.emisor.identificacion.numero = Bandeja.IdEmisor;

                xml.parametros = new parametros();
                xml.parametros.enviodgt = "A";


                db.Entry(Bandeja).State = System.Data.Entity.EntityState.Modified;
                Bandeja.idAceptador = bandeja.idAceptador;
                Bandeja.tipo = bandeja.tipo;
                Bandeja.Mensaje = bandeja.Mensaje;
                Bandeja.DetalleMensaje = bandeja.DetalleMensaje;
                Bandeja.CodigoActividad = bandeja.CodigoActividad;
                Bandeja.CondicionImpuesto = bandeja.CondicionImpuesto;
                Bandeja.impuestoAcreditar = bandeja.impuestoAcreditar;
                Bandeja.gastoAplicable = bandeja.gastoAplicable;
                Bandeja.situacionPresentacion = xml.clave.situacion_presentacion;
                db.SaveChanges();

                HttpClient cliente = new HttpClient();

                var httpContent = new StringContent(JsonConvert.SerializeObject(xml), Encoding.UTF8, "application/json");
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    HttpResponseMessage response = await cliente.PutAsync(parametros.urlCyberAceptacion, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        response.Content.Headers.ContentType.MediaType = "application/json";
                        var resp = await response.Content.ReadAsAsync<respuesta>();



                        db.Entry(Bandeja).State = System.Data.Entity.EntityState.Modified;
                        Bandeja.Procesado = "1";
                        Bandeja.RespuestaHacienda = resp.code.ToString();

                        Bandeja.XMLRespuesta = resp.data;

                        Bandeja.JSON = JsonConvert.SerializeObject(xml);

                        Bandeja.ClaveReceptor = resp.clave;




                        db.SaveChanges();


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

                t.Commit();
                return Request.CreateResponse(HttpStatusCode.OK, Bandeja);
            }
            catch (Exception ex)
            {
                t.Rollback();
                BitacoraErrores be = new BitacoraErrores();
                be.DocNum = "";
                be.Type = "";
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}