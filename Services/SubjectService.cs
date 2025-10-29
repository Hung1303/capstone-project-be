using BusinessObjects;
using Services.DTO.Subject;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<SubjectResponse> CreateSubject(CreateSubjectRequest request)
        {
            var subject = new Subject
            {
                SubjectName = request.SubjectName,
                Description = request.Description,
            };
            await _unitOfWork.GetRepository<Subject>().InsertAsync(subject);
            await _unitOfWork.SaveAsync();
            var result = new SubjectResponse
            {
                SubjectId = subject.Id,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
            };
            return result;
        }

        public async Task<bool> DeleteSubject(Guid id)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (subject == null)
            {
                throw new Exception("Subject Not Found");
            }
            subject.IsDeleted = true;
            await _unitOfWork.GetRepository<Subject>().UpdateAsync(subject);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<SubjectResponse>> GetAllSubject(string? searchTerm, int pageNumber, int pageSize)
        {
            var subjects = _unitOfWork.GetRepository<Subject>().Entities.Where(a => !a.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                subjects = subjects.Where(c =>
                    (c.SubjectName != null && c.SubjectName.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            var totalCount = await subjects.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedSubjects = await subjects
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new SubjectResponse
                {
                    SubjectId = a.Id,
                    SubjectName = a.SubjectName,
                    Description = a.Description,
                }).ToListAsync();
            return paginatedSubjects;
        }

        public async Task<SubjectResponse> GetSubjectById(Guid id)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (subject == null)
            {
                throw new Exception("Subject Not Found");
            }
            var result = new SubjectResponse
            {
                SubjectId = subject.Id,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
            };
            return result;
        }

        public async Task<SubjectResponse> UpdateSubject(Guid id, UpdateSubjectRequest request)
        {

            var subject = await _unitOfWork.GetRepository<Subject>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (subject == null)
            {
                throw new Exception("subject Not Found");
            }
            if (request.SubjectName != null)
            {
                subject.SubjectName = request.SubjectName;
            }
            if (request.Description != null)
            {
                subject.Description = request.Description;
            }
            await _unitOfWork.GetRepository<Subject>().UpdateAsync(subject);
            await _unitOfWork.SaveAsync();

            var result = new SubjectResponse
            {
                SubjectId = subject.Id,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
            };
            return result;
        }
    }
}
