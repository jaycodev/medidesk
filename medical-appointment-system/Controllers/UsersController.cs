using System;
using System.Transactions;
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
        SpecialtyService specialtyService = new SpecialtyService();

        private Doctor FindDoctorById(int id)
        {
            Doctor doctor = new Doctor { UserId = id };
            var result = doctorService.ExecuteRead("GET_DETAILS_BY_ID", doctor).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        private Patient FindPatientById(int id)
        {
            Patient patient = new Patient { UserId = id };
            var result = patientService.ExecuteRead("GET_DETAILS_BY_ID", patient).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            return null;
        }

        private User FindUserById(int id)
        {
            User user = new Patient { UserId = id };
            User result = userService.ExecuteRead("GET_BY_ID", user).First();

            if (result != null)
            {
                result.SelectedRoleCombo = string.Join(",", result.Roles);
                System.Diagnostics.Debug.WriteLine("SelectedRoleCombo: " + result.SelectedRoleCombo);

                return result;
            }

            return null;
        }

        private void LoadSpecialties(int? selectedId = null)
        {
            ViewBag.Specialties = new SelectList(
                specialtyService.ExecuteRead("GET_ALL", new Specialty()),
                "SpecialtyId",
                "Name",
                selectedId
            );
        }

        public JsonResult GetSpecialties()
        {
            var specialties = specialtyService.ExecuteRead("GET_ALL", new Specialty())
                .Select(s => new { s.SpecialtyId, s.Name });

            return Json(specialties, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var currentUser = Session["user"] as User;

            List<User> list = userService.ExecuteRead("GET_ALL", new User
            {
                UserId = currentUser.UserId
            });

            return View(list);
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
                user.Roles = new List<string> { "administrador" };
                int affectedRows = userService.ExecuteWrite("INSERT", user);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Usuario creado correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "No se pudo crear el usuario. Intenta nuevamente.";
                }
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

            var user = FindUserById(id);

            if (user.Roles.Contains("medico"))
            {
                var doctor = FindDoctorById(id);
                if (doctor != null)
                {
                    user.SpecialtyId = doctor.SpecialtyId;
                    user.Status = doctor.Status;

                    System.Diagnostics.Debug.WriteLine($"[DEBUG] SpecialtyId: {doctor.SpecialtyId}");
                }

                LoadSpecialties(user.SpecialtyId);
            }

            if (user.Roles.Contains("paciente"))
            {
                var patient = FindPatientById(id);
                if (patient != null)
                {
                    user.BirthDate = patient.BirthDate;
                    user.BloodType = patient.BloodType;
                }
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.SelectedRoleCombo))
                {
                    user.Roles = user.SelectedRoleCombo
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => r.Trim())
                        .ToList();
                }

                using (var scope = new TransactionScope())
                {
                    int affectedRows = userService.ExecuteWrite("UPDATE", user);

                    if (user.Roles.Contains("medico"))
                    {
                        if (user.SpecialtyId == null || user.Status == null)
                        {
                            throw new ApplicationException("Faltan campos requeridos para el rol médico.");
                        }

                        var doctor = new Doctor
                        {
                            UserId = user.UserId,
                            SpecialtyId = user.SpecialtyId.Value,
                            Status = user.Status.Value
                        };

                        doctorService.ExecuteWrite("UPDATE", doctor);
                    }

                    if (user.Roles.Contains("paciente"))
                    {
                        if (user.BirthDate == null || string.IsNullOrEmpty(user.BloodType))
                        {
                            throw new ApplicationException("Faltan campos requeridos para el rol paciente.");
                        }

                        var patient = new Patient
                        {
                            UserId = user.UserId,
                            BirthDate = user.BirthDate.Value,
                            BloodType = user.BloodType
                        };

                        patientService.ExecuteWrite("UPDATE", patient);
                    }

                    scope.Complete();

                    TempData["Success"] = "¡Usuario actualizado correctamente!";
                    return RedirectToAction("Index");
                }
            }
            catch (ApplicationException ex)
            {
                ViewBag.Message = ex.Message;
            }
            catch (Exception)
            {
                ViewBag.Message = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            LoadSpecialties(user.SpecialtyId);

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
                User = FindUserById(id),
                Doctor = FindDoctorById(id),
                Patient = FindPatientById(id)
            };

            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            return View(FindUserById(id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = FindUserById(id);

            try
            {
                int affectedRows = userService.ExecuteWrite("DELETE", user);

                if (affectedRows > 0)
                {
                    TempData["Success"] = "¡Usuario eliminado correctamente!";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el usuario. Intenta nuevamente.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado. Intenta más tarde.";
            }

            return RedirectToAction("Index");
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
                    tabla.AddCell(string.Join(", ", u.Roles));
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
                    hoja.Cell(fila, 6).Value = string.Join(", ", u.Roles);
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
