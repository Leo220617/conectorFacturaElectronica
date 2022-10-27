using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WAConectorAPI.Models.ModelCliente;

namespace WAConectorAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        Metodos metodo = new Metodos();
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetAsync([FromUri] string CodSucursal = "001")
        {
            try
            {
                var conexion = metodo.DevuelveCadena(CodSucursal);
                ModelCliente db = new ModelCliente();
                
                return Request.CreateResponse(HttpStatusCode.OK, conexion);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,"ERror"  + ex);
            }
        }
        [Route("api/Values/PDF")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetAsync([FromUri] DateTime FechaInicial, DateTime FechaFinal)
        {
            try
            {
                G G = new G();
                ModelCliente db = new ModelCliente();

                var Bandeja = db.BandejaEntrada.Where(a => a.FechaIngreso >= FechaInicial && a.FechaIngreso <= FechaFinal).ToList();

                foreach(var item in Bandeja)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    item.XmlConfirmacion = G.GuardarPDF(item.Pdf, item.NumeroConsecutivo + "_" + item.NombreEmisor);
                    db.SaveChanges();
                }

                return Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, "Error" + ex);
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
