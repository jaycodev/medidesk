using Api.Domains.Patients.DTOs;
using Api.Domains.Patients.Models;
using Api.Domains.Users.DTOs;
using Api.Domains.Users.Models;

namespace Api.Domains.Patients.Repositories
{
    public interface IPatient
    {
        List<Patient> GetList();
        Patient GetById(int id);
        int Create(PatientCreateDTO dto);
        int Update(PatientUpdateDTO dto);
    }
}
