using BusinessObjects.DTO.Course;
using BusinessObjects;
using BusinessObjects.DTO.Subject;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Migrations;

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
            var checkTeacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
            var checkCourse = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId);
            if (checkTeacher == null || checkCourse == null)
            {
                throw new Exception("Teacher and Course Not Found");
            }
            if (checkTeacher.Id != checkCourse.TeacherProfileId)
            {
                throw new Exception("Course Techer must be the same");
            }
            var subject = new Subject
            {
                SubjectName = request.SubjectName,
                Description = request.Description,
                TeacherProfileId = request.TeacherProfileId,
                CourseId = request.CourseId,
            };
            await _unitOfWork.GetRepository<Subject>().InsertAsync(subject);
            await _unitOfWork.SaveAsync();
            var result = new SubjectResponse
            {
                SubjectId = subject.Id,
                SubjectName = subject.SubjectName,
                Description = subject.Description,
                TeacherProfileId = subject.TeacherProfileId,
                CourseId = subject.CourseId,
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

        public async Task<IEnumerable<SubjectResponse>> GetAllSubject(string? searchTerm, int pageNumber, int pageSize, Guid? CourseId, Guid? TeacherProfileId)
        {
            var subjects = _unitOfWork.GetRepository<Subject>().Entities.Where(a => !a.IsDeleted);

            if (CourseId.HasValue)
            {
                subjects = subjects.Where(a => a.CourseId == CourseId);
            }
            if (TeacherProfileId.HasValue)
            {
                subjects = subjects.Where(a => a.TeacherProfileId == TeacherProfileId);
            }

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
                    TeacherProfileId = a.TeacherProfileId,
                    CourseId = a.CourseId,
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
                TeacherProfileId = subject.TeacherProfileId,
                CourseId = subject.CourseId,
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
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == subject.CourseId && !a.IsDeleted);
            if (request.TeacherProfileId.HasValue)
            {
                var checkTeacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
                if (checkTeacher == null || checkTeacher.Id != course.TeacherProfileId)
                {
                    throw new Exception("Teacher Not Found or Course teacher not the same");
                }
                subject.TeacherProfileId = request.TeacherProfileId.Value;
            }
            if (request.CourseId.HasValue)
            {
                var checkCourse = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId);
                if (checkCourse == null || checkCourse.TeacherProfileId != subject.TeacherProfileId)
                {
                    throw new Exception("Course Not Found or Course teacher not the same");
                }
                subject.CourseId = request.CourseId.Value;
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
                TeacherProfileId = subject.TeacherProfileId,
                CourseId = subject.CourseId,
            };
            return result;
        }
    }
}
