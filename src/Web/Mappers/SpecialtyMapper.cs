using Shared.DTOs.Specialties.Requests;
using Shared.DTOs.Specialties.Responses;
using Web.Models.Specialties;

namespace Web.Mappers
{
    public static class SpecialtyMapper
    {
        // ViewModel -> Create Request
        public static SpecialtyRequest ToRequest(this SpecialtyCreateViewModel model)
            => new()
            {
                Name = model.Name,
                Description = model.Description
            };

        // ViewModel -> Update Request
        public static SpecialtyRequest ToRequest(this SpecialtyEditViewModel model)
            => new()
            {
                Name = model.Name,
                Description = model.Description
            };

        // Response -> ViewModel
        public static SpecialtyViewModel ToViewModel(this SpecialtyResponse response)
            => new()
            {
                SpecialtyId = response.SpecialtyId,
                Name = response.Name,
                Description = response.Description
            };

        // Response -> Edit ViewModel
        public static SpecialtyEditViewModel ToEditViewModel(this SpecialtyResponse response)
            => new()
            {
                SpecialtyId = response.SpecialtyId,
                Name = response.Name,
                Description = response.Description
            };
    }
}
