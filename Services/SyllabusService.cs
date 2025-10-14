using BusinessObjects.DTO.Course;
using BusinessObjects;
using BusinessObjects.DTO.Syllabus;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId);
            if (course == null)
            {
                throw new Exception("Course Not Found");
            }
            var syllabus = new Syllabus
            {
                SyllabusName = request.SyllabusName,
                Description = request.Description,
                GradeLevel = request.GradeLevel,
                Subject = request.Subject,
                AssessmentMethod = request.AssessmentMethod,
                CourseMaterial = request.CourseMaterial,
                CourseId = request.CourseId,
            };
            await _unitOfWork.GetRepository<Syllabus>().InsertAsync(syllabus);
            await _unitOfWork.SaveAsync();
            var result = new SyllabusResponse
            {
                Id = syllabus.Id,
                SyllabusName = syllabus.SyllabusName,
                Description = syllabus.Description,
                GradeLevel = syllabus.GradeLevel,
                Subject = syllabus.Subject,
                AssessmentMethod = syllabus.AssessmentMethod,
                CourseMaterial = syllabus.CourseMaterial,
                CourseId = syllabus.CourseId,
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

        public async Task<IEnumerable<SyllabusResponse>> GetAllSyllabus(string? searchTerm, int pageNumber, int pageSize)
        {
            var syllabus = _unitOfWork.GetRepository<Syllabus>().Entities.Where(a => !a.IsDeleted);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                syllabus = syllabus.Where(c =>
                    (c.SyllabusName != null && c.SyllabusName.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.GradeLevel != null && c.GradeLevel.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Subject != null && c.Subject.ToLower().Contains(searchTerm.Trim().ToLower())) ||
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
                    Subject = a.Subject,
                    AssessmentMethod = a.AssessmentMethod,
                    CourseMaterial = a.CourseMaterial,
                    CourseId = a.CourseId,
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
                Subject = syllabus.Subject,
                AssessmentMethod = syllabus.AssessmentMethod,
                CourseMaterial = syllabus.CourseMaterial,
                CourseId = syllabus.CourseId,
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
            if (request.Subject != null)
            {
                syllabus.Subject = request.Subject;
            }
            if (request.AssessmentMethod != null)
            {
                syllabus.AssessmentMethod = request.AssessmentMethod;
            }
            if (request.CourseMaterial != null)
            {
                syllabus.CourseMaterial = request.CourseMaterial;
            }
            if (request.CourseId.HasValue)
            {
                var checkCourse = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId);
                if (checkCourse == null)
                {
                    throw new Exception("Course Not Found");
                }
                syllabus.CourseId = request.CourseId.Value;
            }
            await _unitOfWork.GetRepository<Syllabus>().UpdateAsync(syllabus);
            await _unitOfWork.SaveAsync();

            var result = new SyllabusResponse
            {
                Id = syllabus.Id,
                SyllabusName = syllabus.SyllabusName,
                Description = syllabus.Description,
                GradeLevel = syllabus.GradeLevel,
                Subject = syllabus.Subject,
                AssessmentMethod = syllabus.AssessmentMethod,
                CourseMaterial = syllabus.CourseMaterial,
                CourseId = syllabus.CourseId,
            };
            return result;
        }
    }
}
