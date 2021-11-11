using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WAConectorAPI.Models;
using WAConectorAPI.Models.ModelCliente;

namespace WAConectorAPI.Controllers
{
    [Authorize]
    public class CondicionesVentaController: ApiController
    {
        ModelCliente db = new ModelCliente();

        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {

                var cond = db.CondicionesVenta.ToList();

                if (!string.IsNullOrEmpty(filtro.Texto))
                {
                    cond = cond.Where(a => a.Nombre.ToUpper().Contains(filtro.Texto.ToUpper())).ToList();
                }



                return Request.CreateResponse(HttpStatusCode.OK, cond);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [Route("api/CondicionesVenta/Consultar")]
        public HttpResponseMessage GetOne([FromUri]string id)
        {
            try
            {



                var cond = db.CondicionesVenta.Where(a => a.codCyber == id).FirstOrDefault();


                if (cond == null)
                {
                    throw new Exception("Esta condicion de venta no se encuentra registrado");
                }

                return Request.CreateResponse(HttpStatusCode.OK, cond);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] CondicionesVenta cond)
        {
            try
            {


                var Cond = db.CondicionesVenta.Where(a => a.codCyber == cond.codCyber).FirstOrDefault();

                if (Cond == null)
                {
                    Cond = new CondicionesVenta();
                    Cond.codCyber = cond.codCyber;
                    Cond.codSAP = cond.codSAP;
                    Cond.Nombre = cond.Nombre;



                    db.CondicionesVenta.Add(Cond);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Esta condicion de venta YA existe");
                }


                return Request.CreateResponse(HttpStatusCode.OK, Cond);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("api/CondicionesVenta/Actualizar")]
        public HttpResponseMessage Put([FromBody] CondicionesVenta cond)
        {
            try
            {


                var Cond = db.CondicionesVenta.Where(a => a.codCyber == cond.codCyber).FirstOrDefault();

                if (Cond != null)
                {
                    db.Entry(Cond).State = EntityState.Modified;
                    Cond.codSAP = cond.codSAP;
                    Cond.Nombre = cond.Nombre;

                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Condicion de venta no existe");
                }

                return Request.CreateResponse(HttpStatusCode.OK, Cond);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("api/CondicionesVenta/Eliminar")]
        public HttpResponseMessage Delete([FromUri] string id)
        {
            try
            {


                var Cond = db.CondicionesVenta.Where(a => a.codCyber == id).FirstOrDefault();

                if (Cond != null)
                {


                    db.CondicionesVenta.Remove(Cond);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Condicion de venta no existe");
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}