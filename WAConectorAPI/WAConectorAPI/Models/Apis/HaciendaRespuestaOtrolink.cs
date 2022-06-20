using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WAConectorAPI.Models.Apis
{
    public class HaciendaRespuestaOtrolink
    {
        public int code { get; set; }
        public hacienda_result hacienda_result { get; set; }
    }

    public class hacienda_result
    {
        public string clave { get; set; }
        [JsonProperty("ind-estado")]
        public string ind_estado { get; set; }

        [JsonProperty("respuesta-xml")]
        public string respuesta_xml { get; set; }
    }
}