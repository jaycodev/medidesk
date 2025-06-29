using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using medical_appointment_system.Models;
using medical_appointment_system.Models.ViewModels;
using medical_appointment_system.Services;
using System.Globalization;

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

            if (user == null)
            {
                TempData["Error"] = "El usuario no existe.";
                return RedirectToAction("Index");
            }

            if (!user.CanDelete)
            {
                TempData["Error"] = "Este usuario no puede ser eliminado.";
                return RedirectToAction("Index");
            }

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
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var smallFont = FontFactory.GetFont("Arial", 9, Font.NORMAL);
                var boldFont = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.WHITE);
                var titleFont = FontFactory.GetFont("Arial", 14, Font.BOLD);

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                PdfPTable headerTable = new PdfPTable(3);
                headerTable.WidthPercentage = 100;
                headerTable.SpacingBefore = 5f;
                headerTable.SpacingAfter = 10f;
                headerTable.SetWidths(new float[] { 2f, 6f, 2f });

                PdfPCell dateCell = new PdfPCell(new Phrase($"{date}", smallFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    PaddingTop = 4,
                    PaddingBottom = 4
                };
                PdfPCell titleCell = new PdfPCell(new Phrase("Lista de usuarios", titleFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 4,
                    PaddingBottom = 4
                };
                PdfPCell timeCell = new PdfPCell(new Phrase($"{time}", smallFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    PaddingTop = 4,
                    PaddingBottom = 4
                };

                headerTable.AddCell(dateCell);
                headerTable.AddCell(titleCell);
                headerTable.AddCell(timeCell);
                doc.Add(headerTable);

                float[] columnWidths = { 2f, 2f, 2f, 3f, 2f, 3f };
                var table = new PdfPTable(6)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 5f
                };
                table.SetWidths(columnWidths);

                var headerColor = new BaseColor(0x0a, 0x76, 0xd8);
                string[] headers = { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Teléfono", "Rol(es)" };
                foreach (var h in headers)
                {
                    var cell = new PdfPCell(new Phrase(h, boldFont))
                    {
                        BackgroundColor = headerColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                foreach (var u in users)
                {
                    table.AddCell(new PdfPCell(new Phrase(u.UserId.ToString(), smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(u.FirstName, smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(u.LastName, smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(u.Email, smallFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(u.Phone ?? "-", smallFont)) { Padding = 4 });

                    var roles = string.Join(", ", u.Roles.Select(r =>
                        System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())
                    ));
                    table.AddCell(new PdfPCell(new Phrase(roles, smallFont)) { Padding = 4 });
                }

                doc.Add(table);
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
                var sheet = workbook.Worksheets.Add("Usuarios");

                var now = DateTime.Now;
                string date = now.ToString("dd MMM yyyy");
                string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                sheet.Cell(1, 1).Value = date;
                sheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                sheet.Cell(1, 1).Style.Font.FontSize = 10;

                sheet.Range("B1:E1").Merge();
                sheet.Cell(1, 2).Value = "Lista de usuarios";
                sheet.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cell(1, 2).Style.Font.Bold = true;
                sheet.Cell(1, 2).Style.Font.FontSize = 14;

                sheet.Cell(1, 6).Value = time;
                sheet.Cell(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                sheet.Cell(1, 6).Style.Font.FontSize = 10;

                var headers = new[] { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Teléfono", "Rol(es)" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = sheet.Cell(3, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0a76d8");
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Font.FontSize = 10;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.Black;
                }

                int row = 4;
                foreach (var u in users)
                {
                    sheet.Cell(row, 1).Value = u.UserId;
                    sheet.Cell(row, 2).Value = u.FirstName;
                    sheet.Cell(row, 3).Value = u.LastName;
                    sheet.Cell(row, 4).Value = u.Email;
                    sheet.Cell(row, 5).Value = u.Phone ?? "";

                    var roles = string.Join(", ", u.Roles.Select(r =>
                        System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(r.ToLower())
                    ));
                    sheet.Cell(row, 6).Value = roles;

                    for (int c = 1; c <= 6; c++)
                    {
                        var cell = sheet.Cell(row, c);
                        cell.Style.Font.FontSize = 9;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.OutsideBorderColor = XLColor.Black;
                    }

                    row++;
                }

                sheet.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Position = 0;

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Usuarios.xlsx"
            );
        }
    }
}