using Nutricion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nutricion.Controllers
{
    public class PacienteController : Controller
    {
        static List<Paciente> pacientes = new List<Paciente>();
        private static Datos.Context contexto = 
            new Datos.Context();
        //
        // GET: /Paciente/
        public ActionResult Index()
        {
            List<Paciente> pacientes = contexto.Pacientes.OrderBy(p => p.Id).Select(p => new Paciente()
            {
                Id = p.Id,
                PrimerApellido = p.PrimerApellido,
                SegundoApellido = p.SegundoApellido,
                PrimerNombre = p.PrimerNombre,
                SegundoNombre = p.SegundoNombre,
                NumeroDocumento = p.NumeroDocumento,
                FechaNacimiento = p.FechaNacimiento
            }).ToList();

            return View(pacientes);
        }

        public ActionResult Grafico()
        {
            var pacientes = contexto.Pacientes.GroupBy(p => p.FechaNacimiento.Year).Select(p => new { Year = p.Key, Cantidad = p.Count() });
            var datosGrafico = "[";
            if (pacientes.Count() > 0)
            {
                foreach (var valor in pacientes)
                {
                    datosGrafico += "['" + valor.Year + "', " + valor.Cantidad + "],";
                }
                datosGrafico = datosGrafico.Substring(0, datosGrafico.Length - 1);
                datosGrafico += "]";
            }
            else
            {
                datosGrafico = "[]";
            }

            ViewBag.DatosGrafico = datosGrafico;
            return View();
        }

        //
        // GET: /Paciente/Crear
        public ActionResult Crear()
        {
            EstablecerTiposDocumento();

            return View();
        }

        //
        // POST: /Paciente/Crear
        [HttpPost]
        public ActionResult Crear(Paciente paciente)
        {
            Datos.Paciente pacienteNuevo = new Datos.Paciente();
            pacienteNuevo.FechaNacimiento = paciente.FechaNacimiento.Value;
            pacienteNuevo.IdTipoDocumento = paciente.TipoDocumento;
            pacienteNuevo.NumeroDocumento = paciente.NumeroDocumento;
            pacienteNuevo.PrimerNombre = paciente.PrimerNombre;
            pacienteNuevo.SegundoNombre = paciente.SegundoNombre;
            pacienteNuevo.PrimerApellido = paciente.PrimerApellido;
            pacienteNuevo.SegundoApellido = paciente.SegundoApellido;
            pacienteNuevo.Sexo = paciente.Sexo;
            contexto.Pacientes.Add(pacienteNuevo);
            contexto.SaveChanges();

            EstablecerTiposDocumento();

            return View("Index", pacientes);
        }

        public ActionResult Editar(int id)
        {
            EstablecerTiposDocumento();

            var persona = pacientes.FirstOrDefault(p => p.Id == id);

            return View("Crear", persona);
        }

        public ActionResult Eliminar(int id)
        {
            EstablecerTiposDocumento();

            var persona = pacientes.FirstOrDefault(p => p.Id == id);
            pacientes.Remove(persona);

            return View("Index", pacientes);
        }

        private void EstablecerTiposDocumento()
        {
            var tiposDocumento = new List<TipoDocumento>();
            tiposDocumento.Add(new TipoDocumento()
            {
                Id = 1,
                Nombre = "Cédula de Ciudadanía"
            });
            tiposDocumento.Add(new TipoDocumento()
            {
                Id = 2,
                Nombre = "Tarjeta de Identidad"
            });
            ViewBag.TiposDocumento = tiposDocumento.Select(
                x => new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.Id.ToString()
                });
        }
    }
}