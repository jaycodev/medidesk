using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using medical_appointment_system.Models;
using medical_appointment_system.Models.ViewModels;
using medical_appointment_system.Services;

namespace medical_appointment_system.Controllers
{
    public class UsersController : Controller
    {
        UserService userService = new UserService();
        PatientService patientService = new PatientService();
        DoctorService doctorService = new DoctorService();

        private Doctor FindDoctorByUserId(int id)
        {
            Doctor doctor = new Doctor { UserId = id };
            var result = doctorService.ExecuteRead("GET_DETAILS_BY_ID", doctor).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        private Patient FindPatientByUserId(int id)
        {
            Patient patient = new Patient { UserId = id };
            var result = patientService.ExecuteRead("GET_DETAILS_BY_ID", patient).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        private User FindById(int id)
        {
            User user = new Patient { UserId = id };
            User result = userService.ExecuteRead("GET_BY_ID", user).First();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        public ActionResult Index()
        {
            List<User> lista = userService.ExecuteRead("GET_ALL", new User());
            return View(lista);
        }

        public ActionResult Create()
        {
            return View(new User());
        }

        [HttpPost]
        public ActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                user.Role = "administrador";
                userService.ExecuteWrite("INSERT", user);
                TempData["Success"] = "¡Usuario creado correctamente!";
                return RedirectToAction("Index");
            }
            catch (ApplicationException ex)
            {
                ViewBag.Message = ex.Message;
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return View(user);
        }

        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(FindById(id));
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            int process = userService.ExecuteWrite("UPDATE", user);
            if (process >= 0)
            {
                TempData["Success"] = "¡Usuario actualizado correctamente!";
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new UserDetailsViewModel
            {
                User = FindById(id),
                Doctor = FindDoctorByUserId(id),
                Patient = FindPatientByUserId(id)
            };

            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(FindById(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = FindById(id);

            try
            {
                int process = userService.ExecuteWrite("DELETE", user);
                if (process >= 0)
                {
                    TempData["Success"] = "¡Usuario eliminado correctamente!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar el usuario. " + ex.Message;
                ModelState.AddModelError("", "No se pudo eliminar el usuario.");
            }
            return View(user);
        }

        public ActionResult ExportToPDF()
        {
            var users = userService.ExecuteRead("GET_ALL", new User());
            using (var ms = new System.IO.MemoryStream())
            {
                var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("LISTA DE USUARIOS"));
                doc.Add(new iTextSharp.text.Paragraph(" "));

                var tabla = new iTextSharp.text.pdf.PdfPTable(6);
                tabla.WidthPercentage = 100;
                tabla.AddCell("ID");
                tabla.AddCell("Nombre");
                tabla.AddCell("Apellido");
                tabla.AddCell("Correo");
                tabla.AddCell("Teléfono");
                tabla.AddCell("Rol");

                foreach (var u in users)
                {
                    tabla.AddCell(u.UserId.ToString());
                    tabla.AddCell(u.FirstName);
                    tabla.AddCell(u.LastName);
                    tabla.AddCell(u.Email);
                    tabla.AddCell(u.Phone ?? "");
                    tabla.AddCell(u.Role);
                }

                doc.Add(tabla);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "Usuarios.pdf");
            }
        }

        public ActionResult ExportToExcel()
        {
            var users = userService.ExecuteRead("GET_ALL", new User());

            var stream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                var hoja = workbook.Worksheets.Add("Usuarios");

                hoja.Cell(1, 1).Value = "ID";
                hoja.Cell(1, 2).Value = "Nombre";
                hoja.Cell(1, 3).Value = "Apellido";
                hoja.Cell(1, 4).Value = "Correo";
                hoja.Cell(1, 5).Value = "Teléfono";
                hoja.Cell(1, 6).Value = "Rol";

                int fila = 2;
                foreach (var u in users)
                {
                    hoja.Cell(fila, 1).Value = u.UserId;
                    hoja.Cell(fila, 2).Value = u.FirstName;
                    hoja.Cell(fila, 3).Value = u.LastName;
                    hoja.Cell(fila, 4).Value = u.Email;
                    hoja.Cell(fila, 5).Value = u.Phone ?? "";
                    hoja.Cell(fila, 6).Value = u.Role;
                    fila++;
                }

                hoja.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Position = 0;

            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Usuarios.xlsx");
        }
    }
}
