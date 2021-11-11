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
    public class ParametrosController: ApiController
    {
        ModelCliente db = new ModelCliente();

   

        [Route("api/Parametros/Consultar")]
        public HttpResponseMessage GetOne([FromUri]int id)
        {
            try
            {



                var Parametros = db.Parametros.FirstOrDefault();


                if (Parametros == null)
                {
                    throw new Exception("Este parametro no se encuentra registrado");
                }

                return Request.CreateResponse(HttpStatusCode.OK, Parametros);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("api/Parametros/Actualizar")]
        public HttpResponseMessage Put([FromBody] Parametros param)
        {
            try
            {


                var Parametros = db.Parametros.FirstOrDefault();

                if (Parametros != null)
                {
                    db.Entry(Parametros).State = EntityState.Modified;
                    Parametros.SerieFE = param.SerieFE;
                    Parametros.SerieFEC = param.SerieFEC;
                    Parametros.SerieFEE = param.SerieFEE;
                    Parametros.SerieNC = param.SerieNC;
                    Parametros.SerieND = param.SerieND;
                    Parametros.SerieTE = param.SerieTE;

                    Parametros.urlCyber = param.urlCyber;
                    Parametros.urlCyberRespHacienda = param.urlCyberRespHacienda;
                    Parametros.urlCyberAceptacion = param.urlCyberAceptacion;


                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Parametros no existe");
                }

                return Request.CreateResponse(HttpStatusCode.OK, Parametros);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


    }
}