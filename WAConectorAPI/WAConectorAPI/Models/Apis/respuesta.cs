using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WAConectorAPI.Models.Apis
{
    public class respuesta
    {
        public string data { get; set; }
        public string clave { get; set; }
        public int code { get; set; }
        public string hacienda_mensaje { get; set; }
        public string xml_error { get; set; }
    }
}