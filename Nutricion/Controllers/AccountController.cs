using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Nutricion.Models;
using System.Web.Security;

namespace Nutricion.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            Datos.Context contexto = new Datos.Context();
            var usuario = contexto.Usuarios.FirstOrDefault(u => u.Nombre.ToLower() == login.UserName.ToLower() && u.Clave == login.Password);
            if (usuario != null)
            {
                FormsAuthentication.SetAuthCookie(login.UserName, false);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Mensaje = "Usuario o contraseña no válidos";

            return View("Login");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return View("Login");
        }

    }
}