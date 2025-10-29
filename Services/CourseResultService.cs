﻿using BusinessObjects;
using BusinessObjects.DTO.CourseResult;
using BusinessObjects.DTO.LessonPlan;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CourseResultService : ICourseResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseResultService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CourseResultResponse> CreateCourseResult(CreateCourseResultRequest request)
        {
            var student = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.StudentId && !a.IsDeleted);
            if (student == null)
            {
                throw new Exception("Student Not Found");
            }
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId && !a.IsDeleted);
            if (course == null)
            {
                throw new Exception("Course Not Found");
            }
            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherId && !a.IsDeleted);
            if (teacher == null)
            {
                throw new Exception("Teacher Not Found");
            }
            var checkCourseResult = await _unitOfWork.GetRepository<CourseResult>().Entities
                .FirstOrDefaultAsync(a => a.CourseId == course.Id && a.StudentProfileId == student.Id && !a.IsDeleted);
            if (checkCourseResult !=null)
            {
                throw new Exception("CourseResult for this student already exist");
            }
            var courseResult = new CourseResult
            {
                Mark = request.Mark,
                Comment = request.Comment,
                StudentProfileId = request.StudentId,
                CourseId = request.CourseId,
                TeacherProfileId = request.TeacherId,
            };
            await _unitOfWork.GetRepository<CourseResult>().InsertAsync(courseResult);
            await _unitOfWork.SaveAsync();
            var result = new CourseResultResponse
            {
                Mark = courseResult.Mark,
                Comment = courseResult.Comment,
                StudentId = courseResult.StudentProfileId,
                CourseId = courseResult.CourseId,
                TeacherId = courseResult.TeacherProfileId,
            };
            return result;
        }

        public async Task<bool> DeleteCourseResult(Guid id)
        {
            var courseResult = await _unitOfWork.GetRepository<CourseResult>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (courseResult == null)
            {
                throw new Exception("CourseResult Not Found");
            }
            courseResult.IsDeleted = true;
            await _unitOfWork.GetRepository<CourseResult>().UpdateAsync(courseResult);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<CourseResultResponse>> GetAllCourseResult(string? searchTerm, int pageNumber, int pageSize, Guid? TeacherProfileId, Guid? CourseId, Guid? StudentProfileId)
        {
            var courseResult = _unitOfWork.GetRepository<CourseResult>().Entities.Where(a => !a.IsDeleted);
            if (TeacherProfileId.HasValue)
            {
                courseResult = courseResult.Where(a => a.TeacherProfileId == TeacherProfileId);
            }
            if (CourseId.HasValue)
            {
                courseResult = courseResult.Where(a => a.CourseId == CourseId);
            }
            if (StudentProfileId.HasValue)
            {
                courseResult = courseResult.Where(a => a.StudentProfileId == StudentProfileId);
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courseResult = courseResult.Where(c =>
                    (c.Comment != null && c.Comment.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            var totalCount = await courseResult.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedCourseResult = await courseResult
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new CourseResultResponse
                {
                    Mark = a.Mark,
                    Comment = a.Comment,
                    StudentId = a.StudentProfileId,
                    CourseId = a.CourseId,
                    TeacherId = a.TeacherProfileId,
                }).ToListAsync();
            return paginatedCourseResult;
        }

        public async Task<CourseResultResponse> GetCourseResultById(Guid id)
        {
            var courseResult = await _unitOfWork.GetRepository<CourseResult>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (courseResult == null)
            {
                throw new Exception("CourseResult Not Found");
            }
            var result = new CourseResultResponse
            {
                Mark = courseResult.Mark,
                Comment = courseResult.Comment,
                StudentId = courseResult.StudentProfileId,
                CourseId = courseResult.CourseId,
                TeacherId = courseResult.TeacherProfileId,
            };
            return result;
        }

        public async Task<CourseResultResponse> UpdateCourseResult(Guid id, UpdateCourseResultRequest request)
        {
            var courseResult = await _unitOfWork.GetRepository<CourseResult>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (courseResult == null)
            {
                throw new Exception("CourseResult Not Found");
            }
            if (request.Mark.HasValue)
            {
                courseResult.Mark = request.Mark.Value;
            }
            if (request.Comment != null)
            {
                courseResult.Comment = request.Comment;
            }
            if (request.CourseId.HasValue)
            {
                var checkCourse = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId && !a.IsDeleted);
                if (checkCourse == null)
                {
                    throw new Exception("Course Not Found");
                }
                courseResult.CourseId = request.CourseId.Value;
            }
            if (request.TeacherId.HasValue)
            {
                var checkTeacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherId && !a.IsDeleted);
                if (checkTeacher == null)
                {
                    throw new Exception("Teacher Not Found");
                }
                courseResult.TeacherProfileId = request.TeacherId.Value;
            }
            if (request.StudentId.HasValue)
            {
                var checkStudent = await _unitOfWork.GetRepository<StudentProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.StudentId && !a.IsDeleted);
                if (checkStudent == null)
                {
                    throw new Exception("Student Not Found");
                }
                courseResult.StudentProfileId = request.StudentId.Value;
            }
            await _unitOfWork.GetRepository<CourseResult>().UpdateAsync(courseResult);
            await _unitOfWork.SaveAsync();

            var result = new CourseResultResponse
            {
                Mark = courseResult.Mark,
                Comment = courseResult.Comment,
                StudentId = courseResult.StudentProfileId,
                CourseId = courseResult.CourseId,
                TeacherId = courseResult.TeacherProfileId,
            };
            return result;
        }
    }
}
