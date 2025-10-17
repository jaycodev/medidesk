﻿using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Web.Models.Account;

namespace Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Show(int code = 500)
        {
            string title = "", message = "", homeUrl = Url.Action("Login", "Account") ?? "/";

            var userJson = HttpContext.Session.GetString("UserSession");
            UserSession? sessionUser = null;

            if (!string.IsNullOrEmpty(userJson))
                sessionUser = JsonSerializer.Deserialize<UserSession>(userJson);

            if (sessionUser != null)
            {
                string role = sessionUser.ActiveRole?.ToLower() ?? string.Empty;

                switch (role)
                {
                    case "administrador":
                    case "medico":
                    case "paciente":
                        homeUrl = Url.Action("Home", "Appointments") ?? "/";
                        break;
                }
            }

            switch (code)
            {
                case 400:
                    title = "Solicitud no válida";
                    message = "La solicitud no pudo ser procesada. Revisa los datos e intenta nuevamente.";
                    break;
                case 401:
                    title = "Sesión no iniciada";
                    message = "Debes iniciar sesión para acceder a este recurso. Por favor, autentícate.";
                    break;
                case 403:
                    title = "Acceso no autorizado";
                    message = "No tienes permiso para acceder a esta página o recurso.";
                    break;
                case 404:
                    title = "Página no encontrada";
                    message = "La página que buscas no existe o fue movida.";
                    break;
                case 500:
                    title = "Error interno del servidor";
                    message = "Algo salió mal. Intenta más tarde o contacta a soporte.";
                    break;
                default:
                    title = "Error inesperado";
                    message = "Ocurrió un problema. Intenta más tarde.";
                    break;
            }

            ViewBag.Code = code;
            ViewBag.Title = title;
            ViewBag.Message = message;
            ViewBag.HomeUrl = homeUrl;

            Response.StatusCode = code;
            return View("Show");
        }
    }
}
