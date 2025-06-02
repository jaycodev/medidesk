using sistema_citas_medicas.Models;
using sistema_citas_medicas.Servicio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sistema_citas_medicas.Controllers
{
    public class HorarioDisponibleController : Controller
    {
        ServicioHorarioDisponible servicio = new ServicioHorarioDisponible();

        // GET: HorarioDisponible

        [HttpGet]
        public ActionResult CrearOModificarHorario(int codigo=2)
        {
            //Extraer la lista de horarios disponibles para el médico con el código proporcionado
            HorarioDisponible horarioIdMedico = new HorarioDisponible { IdMedico = codigo };
            List<HorarioDisponible> listaDisponibleActual = servicio.operacionesLectura("CONSULTAR_X_MEDICO", horarioIdMedico);

            var diasSemana = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

            var horariosCompletos = diasSemana.Select(dia =>
            {
                // Buscar si en listaDisponibleActual hay un objeto con este día
                var existente = listaDisponibleActual.FirstOrDefault(h => h.DiaSemana.Equals(dia, StringComparison.OrdinalIgnoreCase));

                if (existente != null)
                {
                    // Si existe, devolver ese objeto (con sus datos)
                    return existente;
                }
                else
                {
                    // Si no existe, crear un objeto con el día, IdMedico y valores por defecto
                    return new HorarioDisponible
                    {
                        DiaSemana = dia,
                        IdMedico = codigo,
                        Habilita = false,
                        HoraInicio = TimeSpan.Zero,   // 00:00
                        HoraFin = TimeSpan.Zero       // 00:00
                    };
                }
            }).ToList();

            return View(horariosCompletos);
        }


        [HttpPost]
        public ActionResult CrearOModificarHorario(List<HorarioDisponible> horarios)
        {
            bool exito = true;

            foreach (var horario in horarios)
            {
                int procesar = servicio.operacionesEscritura("INSERTAR_O_ACTUALIZAR", horario);

                if (procesar < 0)
                {
                    exito = false;
                }
            }

            if (exito)
            {
                return RedirectToAction("Index");
            }
            return View(horarios);
        }
    }
}