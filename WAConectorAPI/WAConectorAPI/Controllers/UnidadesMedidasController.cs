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
    public class UnidadesMedidasController: ApiController
    {
        ModelCliente db = new ModelCliente();

        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {

                var unidades = db.UnidadesMedida.ToList();

                if (!string.IsNullOrEmpty(filtro.Texto))
                {
                    unidades = unidades.Where(a => a.Nombre.ToUpper().Contains(filtro.Texto.ToUpper())).ToList();
                }



                return Request.CreateResponse(HttpStatusCode.OK, unidades);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [Route("api/UnidadesMedidas/Consultar")]
        public HttpResponseMessage GetOne([FromUri]string id)
        {
            try
            {



                var unidad = db.UnidadesMedida.Where(a => a.codCyber == id).FirstOrDefault();


                if (unidad == null)
                {
                    throw new Exception("Esta unidad de medida no se encuentra registrado");
                }

                return Request.CreateResponse(HttpStatusCode.OK, unidad);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] UnidadesMedida unidad)
        {
            try
            {


                var Unidad = db.UnidadesMedida.Where(a => a.codCyber == unidad.codCyber).FirstOrDefault();

                if (Unidad == null)
                {
                    Unidad = new UnidadesMedida();
                    Unidad.codCyber = unidad.codCyber;
                    Unidad.codSAP = unidad.codSAP;
                    Unidad.Nombre = unidad.Nombre;
                     


                    db.UnidadesMedida.Add(Unidad);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Esta unidad YA existe");
                }


                return Request.CreateResponse(HttpStatusCode.OK, Unidad);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("api/UnidadesMedidas/Actualizar")]
        public HttpResponseMessage Put([FromBody] UnidadesMedida unidad)
        {
            try
            {


                var Unidad = db.UnidadesMedida.Where(a => a.codCyber == unidad.codCyber).FirstOrDefault();

                if (Unidad != null)
                {
                    db.Entry(Unidad).State = EntityState.Modified;
                    Unidad.codSAP = unidad.codSAP;
                    Unidad.Nombre = unidad.Nombre;

                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Unidad de medida no existe");
                }

                return Request.CreateResponse(HttpStatusCode.OK, Unidad);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("api/UnidadesMedidas/Eliminar")]
        public HttpResponseMessage Delete([FromUri] string id)
        {
            try
            {


                var Unidad = db.UnidadesMedida.Where(a => a.codCyber == id).FirstOrDefault();

                if (Unidad != null)
                {


                    db.UnidadesMedida.Remove(Unidad);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Unidad no existe");
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