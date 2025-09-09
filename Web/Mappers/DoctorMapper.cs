using Shared.DTOs.Doctors.Requests;
using Shared.DTOs.Doctors.Responses;
using Web.Models.Doctors;

namespace Web.Mappers
{
    public static class DoctorMapper
    {
        // ViewModel -> Create Request
        public static CreateDoctorRequest ToCreateRequest(this DoctorCreateViewModel model)
            => new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Phone = model.Phone,
                SpecialtyId = model.SpecialtyId,
                Status = model.Status ?? false
            };

        // ViewModel -> Update Request
        public static UpdateDoctorRequest ToUpdateRequest(this DoctorEditViewModel model)
            => new()
            {
                SpecialtyId = model.SpecialtyId,
                Status = model.Status
            };

        // Response -> List ViewModel
        public static DoctorListViewModel ToListViewModel(this DoctorListResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                SpecialtyName = response.SpecialtyName,
                ProfilePicture = response.ProfilePicture,
                Status = response.Status
            };

        // Response -> Detail ViewModel
        public static DoctorDetailViewModel ToDetailViewModel(this DoctorResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Phone = response.Phone,
                SpecialtyName = response.SpecialtyName,
                ProfilePicture = response.ProfilePicture,
                Status = response.Status
            };

        // Response -> Edit ViewModel
        public static DoctorEditViewModel ToEditViewModel(this DoctorResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Phone = response.Phone,
                SpecialtyId = response.SpecialtyId,
                Status = response.Status
            };
    }
}
