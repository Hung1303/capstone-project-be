using BusinessObjects.DTO.User;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<CreateAdminRequest> CreateAdminRequest(CreateAdminRequest request);
        Task<CreateCenterRequest> CreateCenterRequest(CreateCenterRequest request);
        Task<CreateTeacherRequest> CreateTeacherRequest(CreateTeacherRequest request);
        Task<CreateTeacherRequest> CenterAddTeacherRequest(Guid centerOwnerId, CreateTeacherRequest request);
        Task<CreateParentRequest> CreateParentRequest(CreateParentRequest request);
        Task<CreateStudentRequest> CreateStudentRequest(Guid parentId, CreateStudentRequest request);
    }
}
