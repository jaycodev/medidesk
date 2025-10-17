using Shared.DTOs.Patients.Requests;
using Shared.DTOs.Patients.Responses;
using Web.Models.Patients;

namespace Web.Mappers
{
    public static class PatientMapper
    {
        // ViewModel -> Create Request
        public static CreatePatientRequest ToCreateRequest(this PatientCreateViewModel model)
            => new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Phone = model.Phone,
                BirthDate = model.BirthDate ?? DateOnly.MinValue,
                BloodType = model.BloodType
            };

        // ViewModel -> Update Request
        public static UpdatePatientRequest ToUpdateRequest(this PatientEditViewModel model)
            => new()
            {
                BirthDate = model.BirthDate,
                BloodType = model.BloodType
            };

        // Response -> List ViewModel
        public static PatientListViewModel ToListViewModel(this PatientListResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                ProfilePicture = response.ProfilePicture,
                BirthDate = response.BirthDate,
                BloodType = response.BloodType
            };

        // Response -> Detail ViewModel
        public static PatientDetailViewModel ToDetailViewModel(this PatientResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Phone = response.Phone,
                ProfilePicture = response.ProfilePicture,
                BirthDate = response.BirthDate,
                BloodType = response.BloodType
            };

        // Response -> Edit ViewModel
        public static PatientEditViewModel ToEditViewModel(this PatientResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Phone = response.Phone,
                BirthDate = response.BirthDate,
                BloodType = response.BloodType
            };
    }
}
