using BusinessObjects;
using BusinessObjects.DTO.Syllabus;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class SyllabusService : ISyllabusService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SyllabusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<SyllabusResponse> CreateSyllabus(CreateSyllabusRequest request)
        {
            var subject = await _unitOfWork.GetRepository<Subject>().Entities.FirstOrDefaultAsync(a => a.Id == request.SubjectId);
            if (subject == null)
            {
                throw new Exception("Subject Not Found");
            }
            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
            if (teacher == null)
            {
                throw new Exception("Teacher Not Found");
            }
            var syllabus = new Syllabus
            {
                SyllabusName = request.SyllabusName,
                Description = request.Description,
                GradeLevel = request.GradeLevel,
                //Subject = request.Subject,
                AssessmentMethod = request.AssessmentMethod,
                CourseMaterial = request.CourseMaterial,
                SubjectId = request.SubjectId,
                TeacherProfileId = request.TeacherProfileId,
            };
            await _unitOfWork.GetRepository<Syllabus>().InsertAsync(syllabus);
            await _unitOfWork.SaveAsync();
            var result = new SyllabusResponse
            {
                Id = syllabus.Id,
                SyllabusName = syllabus.SyllabusName,
                Description = syllabus.Description,
                GradeLevel = syllabus.GradeLevel,
                //Subject = syllabus.Subject,
                AssessmentMethod = syllabus.AssessmentMethod,
                CourseMaterial = syllabus.CourseMaterial,
                SubjectId = syllabus.SubjectId,
                TeacherProfileId = syllabus.TeacherProfileId,
            };
            return result;
        }

        public async Task<bool> DeleteSyllabus(Guid id)
        {
            var syllabus = await _unitOfWork.GetRepository<Syllabus>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (syllabus == null)
            {
                throw new Exception("Syllabus Not Found");
            }
            syllabus.IsDeleted = true;
            await _unitOfWork.GetRepository<Syllabus>().UpdateAsync(syllabus);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<SyllabusResponse>> GetAllSyllabus(string? searchTerm, int pageNumber, int pageSize, Guid? subjectId, Guid? TeacherProfileId)
        {
            var syllabus = _unitOfWork.GetRepository<Syllabus>().Entities.Where(a => !a.IsDeleted);
            if (subjectId.HasValue)
            {
                syllabus = syllabus.Where(a => a.SubjectId == subjectId);
            }
            if (TeacherProfileId.HasValue)
            {
                syllabus = syllabus.Where(a => a.TeacherProfileId == TeacherProfileId);
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                syllabus = syllabus.Where(c =>
                    (c.SyllabusName != null && c.SyllabusName.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.GradeLevel != null && c.GradeLevel.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    //(c.Subject != null && c.Subject.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.AssessmentMethod != null && c.AssessmentMethod.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.CourseMaterial != null && c.CourseMaterial.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            var totalCount = await syllabus.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedSylabus = await syllabus
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new SyllabusResponse
                {
                    Id = a.Id,
                    SyllabusName = a.SyllabusName,
                    Description = a.Description,
                    GradeLevel = a.GradeLevel,
                    //Subject = a.Subject,
                    AssessmentMethod = a.AssessmentMethod,
                    CourseMaterial = a.CourseMaterial,
                    SubjectId = a.SubjectId,
                    TeacherProfileId = a.TeacherProfileId,
                }).ToListAsync();
            return paginatedSylabus;
        }

        public async Task<SyllabusResponse> GetSyllabusById(Guid id)
        {
            var syllabus = await _unitOfWork.GetRepository<Syllabus>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (syllabus == null)
            {
                throw new Exception("Syllabus Not Found");
            }
            var result = new SyllabusResponse
            {
                Id = syllabus.Id,
                SyllabusName = syllabus.SyllabusName,
                Description = syllabus.Description,
                GradeLevel = syllabus.GradeLevel,
                //Subject = syllabus.Subject,
                AssessmentMethod = syllabus.AssessmentMethod,
                CourseMaterial = syllabus.CourseMaterial,
                SubjectId = syllabus.SubjectId,
                TeacherProfileId = syllabus.TeacherProfileId,
            };
            return result;
        }

        public async Task<SyllabusResponse> UpdateSyllabus(Guid id, UpdateSyllabusRequest request)
        {
            var syllabus = await _unitOfWork.GetRepository<Syllabus>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (syllabus == null)
            {
                throw new Exception("Syllabus Not Found");
            }
            if (request.SyllabusName != null)
            {
                syllabus.SyllabusName = request.SyllabusName;
            }
            if (request.Description != null)
            {
                syllabus.Description = request.Description;
            }
            if (request.GradeLevel != null)
            {
                syllabus.GradeLevel = request.GradeLevel;
            }
            //if (request.Subject != null)
            //{
            //    syllabus.Subject = request.Subject;
            //}
            if (request.AssessmentMethod != null)
            {
                syllabus.AssessmentMethod = request.AssessmentMethod;
            }
            if (request.CourseMaterial != null)
            {
                syllabus.CourseMaterial = request.CourseMaterial;
            }
            if (request.SubjectId.HasValue)
            {
                var checkSubject = await _unitOfWork.GetRepository<Subject>().Entities.FirstOrDefaultAsync(a => a.Id == request.SubjectId);
                if (checkSubject == null)
                {
                    throw new Exception("Subject Not Found");
                }
                syllabus.SubjectId = request.SubjectId.Value;
            }
            if (request.TeacherProfileId.HasValue)
            {
                var checkTeacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
                if (checkTeacher == null)
                {
                    throw new Exception("Teacher Not Found");
                }
                syllabus.TeacherProfileId = request.TeacherProfileId.Value;
            }
            await _unitOfWork.GetRepository<Syllabus>().UpdateAsync(syllabus);
            await _unitOfWork.SaveAsync();

            var result = new SyllabusResponse
            {
                Id = syllabus.Id,
                SyllabusName = syllabus.SyllabusName,
                Description = syllabus.Description,
                GradeLevel = syllabus.GradeLevel,
                //Subject = syllabus.Subject,
                AssessmentMethod = syllabus.AssessmentMethod,
                CourseMaterial = syllabus.CourseMaterial,
                SubjectId = syllabus.SubjectId,
                TeacherProfileId = syllabus.TeacherProfileId,
            };
            return result;
        }
    }
}
