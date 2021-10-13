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
