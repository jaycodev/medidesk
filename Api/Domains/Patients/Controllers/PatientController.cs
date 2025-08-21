using Api.Data.Contract;
using Api.Domains.Patients.DTOs;
using Api.Domains.Patients.Models;
using Api.Domains.Patients.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Globalization;

namespace Api.Domains.Patients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatient patientDATA;

        public PatientController(IPatient patient)
        {
            patientDATA = patient;
        }

        [HttpGet("{id}")]
        public IActionResult FindById(int id)
        {
            Patient patient = new Patient { UserId = id };
            var result = patientDATA.GetById(id);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound(new { message = "Paciente no encontrado" });
        }

        [HttpGet]
        public IActionResult ListPatients()
        {
            Patient patient = new Patient();
            return Ok(patientDATA.GetList());
        }

        [HttpPost]
        public IActionResult RegisterPatient(PatientCreateDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Datos del paciente no válidos" });
            }

            var result = patientDATA.Create(dto);
            if (result > 0)
            {
                return Ok(new { message = "Paciente creado correctamente" });
            }
            return BadRequest(new { message = "No se pudo crear el paciente" });
        }

        [HttpPut]
        public IActionResult UpdatePatient(PatientUpdateDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "Datos del paciente no válidos" });
            }

            var result = patientDATA.Update(dto);
            if (result > 0)
            {
                return Ok(new { message = "Paciente actualizado correctamente" });
            }
            return NotFound(new { message = "Paciente no encontrado" });
        }
        /*
                public ActionResult ExportToExcel()
                {
                    var patients = service.ExecuteRead("GET_ALL", new Patient());

                    var stream = new MemoryStream();

                    using (var workbook = new XLWorkbook())
                    {
                        var sheet = workbook.Worksheets.Add("Pacientes");

                        var now = DateTime.Now;
                        string date = now.ToString("dd MMM yyyy");
                        string time = now.ToString("hh:mm tt", new CultureInfo("es-PE"));

                        sheet.Cell(1, 1).Value = date;
                        sheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        sheet.Cell(1, 1).Style.Font.FontSize = 10;

                        sheet.Range("B1:E1").Merge();
                        sheet.Cell(1, 2).Value = "Lista de pacientes";
                        sheet.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        sheet.Cell(1, 2).Style.Font.Bold = true;
                        sheet.Cell(1, 2).Style.Font.FontSize = 14;

                        sheet.Cell(1, 6).Value = time;
                        sheet.Cell(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        sheet.Cell(1, 6).Style.Font.FontSize = 10;

                        var headers = new[] { "Código", "Nombre(s)", "Apellido(s)", "Correo electrónico", "Fecha nacimiento", "Grupo sanguíneo" };
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
                        foreach (var p in patients)
                        {
                            sheet.Cell(row, 1).Value = p.UserId;
                            sheet.Cell(row, 2).Value = p.FirstName;
                            sheet.Cell(row, 3).Value = p.LastName;
                            sheet.Cell(row, 4).Value = p.Email;
                            sheet.Cell(row, 5).Value = p.BirthDate?.ToString("dd MMM yyyy") ?? "-";
                            sheet.Cell(row, 6).Value = p.BloodType;

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

                    return File(stream,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "Pacientes.xlsx");
                }
        */
    }
}
