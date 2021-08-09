﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using WAConectorAPI.Models.ModelCliente;

namespace WAConectorAPI.Controllers
{
    public class Metodos
    {
        ModelCliente db = new ModelCliente();

        public bool SendV2(string para, string copia, string copiaOculta, string de, string displayName, string asunto,
           string html, string HostServer, int Puerto, bool EnableSSL, string UserName, string Password, List<Attachment> ArchivosAdjuntos = null)
        {
            try
            {

                MailMessage mail = new MailMessage();
                mail.Subject = asunto;
                mail.Body = html;
                mail.IsBodyHtml = true;

                // * mail.From = new MailAddress(WebConfigurationManager.AppSettings["UserName"], displayName);
                mail.From = new MailAddress(de, displayName);

                var paraList = para.Split(';');
                foreach (var p in paraList)
                {
                    if (p.Trim().Length > 0)
                        mail.To.Add(p.Trim());
                }
                var ccList = copia.Split(';');
                foreach (var cc in ccList)
                {
                    if (cc.Trim().Length > 0)
                        mail.CC.Add(cc.Trim());
                }
                var ccoList = copiaOculta.Split(';');
                foreach (var cco in ccoList)
                {
                    if (cco.Trim().Length > 0)
                        mail.Bcc.Add(cco.Trim());
                }



                if (ArchivosAdjuntos != null)
                {
                    foreach (var archivo in ArchivosAdjuntos)
                    {
                        //if (!string.IsNullOrEmpty(archivo))
                        mail.Attachments.Add(archivo);
                    }
                }


                SmtpClient client = new SmtpClient();
                client.Host = HostServer; // WebConfigurationManager.AppSettings["HostName"];
                client.Port = Puerto; // int.Parse(WebConfigurationManager.AppSettings["Port"].ToString());
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = EnableSSL; // bool.Parse(WebConfigurationManager.AppSettings["EnableSsl"]);
                client.Credentials = new NetworkCredential(UserName, Password);

                client.Send(mail);
                client.Dispose();
                mail.Dispose();

                return true;

            }
            catch (Exception ex)
            {


                return false;
            }
        }

        public void EnviarCorreo(string metodo, string motivo, string seguimiento)
        {
            try
            {
                EnvioCorreos correo = db.EnvioCorreos.FirstOrDefault();

                var html = "<!DOCTYPE html> <html> <head><meta charset='utf - 8'>";
                html += " <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css' integrity='sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm' crossorigin='anonymous'> ";
                html += " <script src='https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js' integrity='sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl' crossorigin='anonymous'></script> ";
                html += " </head><body><div class='row' style='margin-left: 30%; margin-top: 7%;'><div class='col-sm-10'> ";
                html += " <h3> Error en el método: "+metodo+" </h3><p> Ha ocurrido un error, a continuación más información:  </p> ";
                html += " <ul><li>Descripción: <b>"+motivo+"</b></li> ";
                html += " <li>Línea de código: <b>" +seguimiento+ " </b></li></ul></div></div></body></html> ";

               var resp = SendV2("dsalazar@dydconsultorescr.com", "larce@dydconsultorescr.com", "", correo.Email, "Error Middleware", "Ha ocurrido un error", html, correo.HostName, correo.Port, correo.UseSSL, correo.Email, correo.Password);

                if(!resp)
                {
                    BitacoraErrores errores = new BitacoraErrores();
                    errores.Descripcion = "No se ha podido enviar el correo";
                    errores.StackTrace = "Envio de correo " + JsonConvert.SerializeObject(correo);
                    errores.Fecha = DateTime.Now;
                    db.BitacoraErrores.Add(errores);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                BitacoraErrores errores = new BitacoraErrores();
                errores.Descripcion = "No se ha podido enviar el correo " + ex.Message;
                errores.StackTrace = ex.StackTrace;
                errores.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(errores);
                db.SaveChanges();

            }
        }


    }
}