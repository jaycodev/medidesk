using Shared.DTOs.Users.Requests;
using Shared.DTOs.Users.Responses;
using Web.Models.Users;

namespace Web.Mappers
{
    public static class UserMapper
    {
        // ViewModel -> Create Request
        public static CreateUserRequest ToCreateRequest(this UserCreateViewModel model)
            => new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Phone = model.Phone
            };

        // ViewModel -> Update Request
        public static UpdateUserRequest ToUpdateRequest(this UserEditViewModel model)
            => new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Roles = (model.SelectedRoleCombo ?? string.Empty)
                            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(r => r.Trim())
                            .ToList()
            };

        // Response -> List ViewModel
        public static UserListViewModel ToListViewModel(this UserListResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Phone = response.Phone,
                Roles = response.Roles,
                ProfilePicture = response.ProfilePicture,
                CanDelete = response.CanDelete
            };

        // Response -> Detail ViewModel
        public static UserDetailViewModel ToDetailViewModel(this UserResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Phone = response.Phone,
                Roles = response.Roles,
                ProfilePicture = response.ProfilePicture,
                SpecialtyName = response.SpecialtyName,
                Status = response.Status,
                BirthDate = response.BirthDate,
                BloodType = response.BloodType
            };

        // Response -> Edit ViewModel
        public static UserEditViewModel ToEditViewModel(this UserResponse response)
            => new()
            {
                UserId = response.UserId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                Email = response.Email,
                Phone = response.Phone,
                SelectedRoleCombo = response.Roles != null ? string.Join(",", response.Roles) : string.Empty,
                Roles = response.Roles ?? new List<string>()
            };
    }
}
