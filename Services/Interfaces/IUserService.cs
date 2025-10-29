using BusinessObjects;
using Services.DTO.User;
using Core.Base;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<CreateAdminRequest> CreateAdminRequest(CreateAdminRequest request);
        Task<CreateAdminRequest> CreateInspectorRequest(CreateAdminRequest request);
        Task<CreateCenterRequest> CreateCenterRequest(CreateCenterRequest request);
        Task<CreateTeacherRequest> CreateTeacherRequest(CreateTeacherRequest request);
        Task<CreateTeacherRequest> CenterAddTeacherRequest(Guid centerOwnerId, CreateTeacherRequest request);
        Task<CreateParentRequest> CreateParentRequest(CreateParentRequest request);
        Task<CreateStudentRequest> CreateStudentRequest(Guid parentId, CreateStudentRequest request);
        Task<CenterDetailRespone?> UpdateCenterAsync(Guid userId, CenterUpdateRequest request);
        Task<TeacherDetailResponse?> UpdateTeacherAsynce(Guid userId, TeacherUpdateRequest request);
        Task<ParentDetailResponse?> UpdateParentAsynce(Guid userId, ParentUpdateRequest request);
        Task<StudentDetailResponse?> UpdateStudentAsynce(Guid userId, StudentUpdateRequest request);
        Task<(IEnumerable<CenterListResponse> Centers, int TotalCount)> GetAllCentersAsync(int pageNumber, int pageSize, string? centerName = null);
        Task<(IEnumerable<TeacherListResponse> Teachers, int TotalCount)> GetAllTeachersAsync(int pageNumber, int pageSize, string? fullName = null);
        Task<(IEnumerable<ParentListResponse> Parents, int TotalCount)> GetAllParentsAsync(int pageNumber, int pageSize, string? fullName = null);
        Task<(IEnumerable<StudentListResponse> Students, int TotalCount)> GetAllStudentsAsync(int pageNumber, int pageSize, string? fullName = null);
        Task<(IEnumerable<UserSummaryDto> Users, int TotalCount)> GetAllUsersAsync(int pageNumber, int pageSize, string? fullName = null);
        Task<UserDetailResponse?> GetUserByIdAsync(Guid userId);
        Task<CenterDetailRespone?> GetCenterById(Guid userId);
        Task<(IEnumerable<TeacherListResponse> Teachers, int TotalCount)> GetTeachersByCenterIdAsync(
            Guid centerId, int pageNumber, int pageSize, string? fullName = null);
        Task<TeacherDetailResponse?> GetTeacherById(Guid userId);
        Task<ParentDetailResponse?> GetParentById(Guid userId);
        Task<StudentDetailResponse?> GetStudentById(Guid userId);
        Task<bool> DeleteUser(Guid userId);
        Task<bool> UpdateUserStatus(Guid userId, int status);
        Task<bool> ChangePassword(Guid userId, string currentPassword, string newPassword);
        Task<(IEnumerable<CenterListResponse> Centers, int TotalCount)> GetCentersByStatusAsync(CenterStatus status, int pageNumber, int pageSize, string? centerName = null);
        Task<bool> UpdateCenterStatusAsync(Guid centerId, CenterStatus status, string? reason = null);
    }
}
