using Nutricion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nutricion.Controllers
{
    [Authorize]
    public class PacienteController : Controller
    {
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
            if (paciente.Id == 0)
            {
                Datos.Paciente pacienteNuevo = new Datos.Paciente();
                pacienteNuevo.FechaNacimiento = paciente.FechaNacimiento.Value;
                pacienteNuevo.IdTipoDocumento = paciente.TipoDocumento.Id;
                pacienteNuevo.NumeroDocumento = paciente.NumeroDocumento;
                pacienteNuevo.PrimerApellido = paciente.PrimerApellido;
                pacienteNuevo.PrimerNombre = paciente.PrimerNombre;
                pacienteNuevo.SegundoApellido = paciente.SegundoApellido;
                pacienteNuevo.SegundoNombre = paciente.SegundoNombre;
                pacienteNuevo.Sexo = paciente.Sexo;
                contexto.Pacientes.Add(pacienteNuevo);
            }
            else
            {
                Datos.Paciente pacienteActual = contexto.Pacientes.FirstOrDefault(p => p.Id == paciente.Id);
                if (pacienteActual != null)
                {
                    pacienteActual.FechaNacimiento = paciente.FechaNacimiento.Value;
                    pacienteActual.IdTipoDocumento = paciente.TipoDocumento.Id;
                    pacienteActual.NumeroDocumento = paciente.NumeroDocumento;
                    pacienteActual.PrimerApellido = paciente.PrimerApellido;
                    pacienteActual.PrimerNombre = paciente.PrimerNombre;
                    pacienteActual.SegundoApellido = paciente.SegundoApellido;
                    pacienteActual.SegundoNombre = paciente.SegundoNombre;
                    pacienteActual.Sexo = paciente.Sexo;
                }
            }
            contexto.SaveChanges();

            EstablecerTiposDocumento();

            List<Paciente> pacientes = contexto.Pacientes.OrderBy(p => p.Id).Select(p => new Paciente()
            {
                Id = p.Id,
                PrimerApellido = p.PrimerApellido,
                SegundoApellido = p.SegundoApellido,
                PrimerNombre = p.PrimerNombre,
                SegundoNombre = p.SegundoNombre,
                NumeroDocumento = p.NumeroDocumento,
                FechaNacimiento = p.FechaNacimiento,
                TipoDocumento = new TipoDocumento() { Nombre = p.TipoDocumento.Nombre }
            }).ToList();

            return View("Index", pacientes);
        }

        public ActionResult Editar(int id)
        {
            EstablecerTiposDocumento();

            var pacienteActual = contexto.Pacientes.FirstOrDefault(p => p.Id == id);
            Paciente paciente = new Paciente();
            if (pacienteActual != null)
            {
                paciente.Id = pacienteActual.Id;
                paciente.FechaNacimiento = pacienteActual.FechaNacimiento;
                paciente.TipoDocumento = new TipoDocumento() { Id = pacienteActual.IdTipoDocumento };
                paciente.NumeroDocumento = pacienteActual.NumeroDocumento;
                paciente.PrimerApellido = pacienteActual.PrimerApellido;
                paciente.PrimerNombre = pacienteActual.PrimerNombre;
                paciente.SegundoApellido = pacienteActual.SegundoApellido;
                paciente.SegundoNombre = pacienteActual.SegundoNombre;
                paciente.Sexo = pacienteActual.Sexo;
            }

            return View("Crear", paciente);
        }

        public ActionResult Eliminar(int id)
        {
            EstablecerTiposDocumento();

            var pacienteActual = contexto.Pacientes.FirstOrDefault(p => p.Id == id);
            contexto.Pacientes.Remove(pacienteActual);
            contexto.SaveChanges();

            List<Paciente> pacientes = contexto.Pacientes.OrderBy(p => p.Id).Select(p => new Paciente()
            {
                Id = p.Id,
                PrimerApellido = p.PrimerApellido,
                SegundoApellido = p.SegundoApellido,
                PrimerNombre = p.PrimerNombre,
                SegundoNombre = p.SegundoNombre,
                NumeroDocumento = p.NumeroDocumento,
                FechaNacimiento = p.FechaNacimiento,
                TipoDocumento = new TipoDocumento() { Nombre = p.TipoDocumento.Nombre }
            }).ToList();

            return View("Index", pacientes);
        }

        private void EstablecerTiposDocumento()
        {
            var tiposDocumento = contexto.TiposDocumento.OrderBy(t => t.Nombre).Select(t => new TipoDocumento()
            {
                Id = t.Id,
                Nombre = t.Nombre
            }).ToList();

            ViewBag.TiposDocumento = tiposDocumento.Select(x => new SelectListItem
            {
                Text = x.Nombre,
                Value = x.Id.ToString()
            });
        }
    }
}