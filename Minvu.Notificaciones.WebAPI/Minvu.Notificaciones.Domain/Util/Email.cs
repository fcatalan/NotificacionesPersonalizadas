using Minvu.Notificaciones.IData.Log;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Domain.Util
{
	public class Email
	{
		static bool mailSent = false;
		public static void EnviarCorreo(string remitente, string nombreRemitente, 
																		string para, string cc, string cco, 
																		Dictionary<string, byte[]> adjuntos, 
																		string asunto, string cuerpo,
																		Dictionary<string, byte[]> cids = null,
																		int idCorreo = 0)
		{
			MailMessage sMensaje = new MailMessage();
			SmtpClient smptClient = new SmtpClient();
			sMensaje.From = new MailAddress(remitente, nombreRemitente);
			sMensaje.To.Add(para);
			if (cc != null && cc != string.Empty) sMensaje.CC.Add(cc);
			if (cco != null && cco != string.Empty) sMensaje.Bcc.Add(cco);
			sMensaje.Subject = asunto;
			sMensaje.IsBodyHtml = true;
			sMensaje.Body = cuerpo;
			sMensaje.Headers.Add("Codigo-Correo-Notificaciones", idCorreo.ToString());
			sMensaje.Headers.Add("Return-Receipt-To", remitente);
			AlternateView view = AlternateView.CreateAlternateViewFromString(cuerpo, Encoding.UTF8, "text/html");
			if (adjuntos != null && adjuntos.Count > 0)
			{
				foreach (string nombreAdjunto in adjuntos.Keys)
				{
					MemoryStream mStream = new MemoryStream(adjuntos[nombreAdjunto]);
					Attachment attachment = new Attachment(mStream, nombreAdjunto);
					sMensaje.Attachments.Add(attachment);
				}
			}
			if (cids != null)
			{
				foreach (string cidName in cids.Keys)
				{
					MemoryStream mStream = new MemoryStream(cids[cidName]);
					LinkedResource cidImage = new LinkedResource(mStream);
					cidImage.ContentId = cidName;
					view.LinkedResources.Add(cidImage);
				}
				sMensaje.AlternateViews.Add(view);
			}
			//LinkedResource img = new LinkedResource()
			using (var smtp = new SmtpClient())
			{
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["UsarSSLCorreo"])) smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["UsarSSLCorreo"]);
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PuertoCorreoSMTP"])) smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["PuertoCorreoSMTP"]);
				if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["UsuarioCorreo"])
						&& !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClaveCorreo"]))
				{
					smtp.UseDefaultCredentials = false;
					smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
					smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["UsuarioCorreo"], ConfigurationManager.AppSettings["ClaveCorreo"]);
				}
				smtp.Host = ConfigurationManager.AppSettings["ServidorCorreoSMTP"];
				Log.RegistrarInfo(string.Format("[EnviarCorreo] Enviando correo a {0}, desde {1} <{2}> con {3} adjuntos y {4} cids. ID Correo {5}",
																para, sMensaje.From.DisplayName, sMensaje.From.Address, adjuntos == null ? "null" : adjuntos.Count.ToString(), cids == null ? "null" : cids.Count.ToString(),idCorreo));

				smtp.Send(sMensaje);

			}
		}
	}
}
