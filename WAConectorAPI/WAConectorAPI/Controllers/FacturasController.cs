using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WAConectorAPI.Models;
using WAConectorAPI.Models.ModelCliente;

namespace WAConectorAPI.Controllers
{
    [Authorize]
    public class FacturasController: ApiController
    {
        ModelCliente db = new ModelCliente();

        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {
                DateTime time = new DateTime();
                var Documentos = db.EncDocumento.Where( a =>(filtro.FechaInicial != time ? a.Fecha >= filtro.FechaInicial : true) && (filtro.FechaFinal != time ? a.Fecha <= filtro.FechaFinal : true)).ToList();

                if (!string.IsNullOrEmpty(filtro.Texto))
                {
                    Documentos = Documentos.Where(a => a.consecutivoSAP.ToUpper().Contains(filtro.Texto.ToUpper())).ToList();
                }

                if(!string.IsNullOrEmpty(filtro.Estado))
                {
                    if(filtro.Estado != "NULL")
                    {
                        Documentos = Documentos.Where(a => a.RespuestaHacienda.ToUpper().Contains(filtro.Estado.ToUpper())).ToList();

                    }
                }
                if (!string.IsNullOrEmpty(filtro.CodMoneda))
                {
                    if (filtro.CodMoneda != "NULL")
                    {
                        Documentos = Documentos.Where(a => a.TipoDocumento.ToUpper().Contains(filtro.CodMoneda.ToUpper())).ToList();

                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, Documentos);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [Route("api/Facturas/Consultar")]
        public HttpResponseMessage GetOne([FromUri]int id)
        {
            try
            {



                var Documentos = db.EncDocumento.Where(a => a.id == id).FirstOrDefault();


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


        [Route("api/Detalles/Consultar")]
        public HttpResponseMessage GetOneDetalle([FromUri]int id)
        {
            try
            {



                var Documentos = db.DetDocumento.Where(a => a.idEncabezado == id).ToList();


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
    }
}