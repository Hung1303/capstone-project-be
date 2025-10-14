using BusinessObjects;
using BusinessObjects.DTO.ClassSchedule;
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
    public class ClassScheduleService : IClassScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ClassScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ClassScheduleResponse> CreateClassSchedule(CreateClassScheduleRequest request)
        {
            var course = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId);
            if (course == null)
            {
                throw new Exception("Course Not Found");
            }
            //if (request.StartDate.HasValue && request.EndDate.HasValue)
            //{
            //    if (request.StartDate > request.EndDate)
            //    {
            //        throw new Exception("start date must be before end date");
            //    }
            //}
            //if (request.StartTime > request.EndTime)
            //{
            //    throw new Exception("start time must be before end time");
            //}
            var classSchedule = new ClassSchedule
            {
                CourseId = request.CourseId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RoomOrLink = request.RoomOrLink,
            };
            await _unitOfWork.GetRepository<ClassSchedule>().InsertAsync(classSchedule);
            await _unitOfWork.SaveAsync();
            var result = new ClassScheduleResponse
            {
                Id = classSchedule.Id,
                CourseId = classSchedule.CourseId,
                DayOfWeek = classSchedule.DayOfWeek,
                StartTime = classSchedule.StartTime,
                EndTime = classSchedule.EndTime,
                StartDate = classSchedule.StartDate,
                EndDate = classSchedule.EndDate,
                RoomOrLink = classSchedule.RoomOrLink,
            };
            return result;
        }

        public async Task<bool> DeleteCLassSchedule(Guid id)
        {
            var classSchedule = await _unitOfWork.GetRepository<ClassSchedule>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (classSchedule == null)
            {
                throw new Exception("class schedule Not Found");
            }
            classSchedule.IsDeleted = true;
            await _unitOfWork.GetRepository<ClassSchedule>().UpdateAsync(classSchedule);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<ClassScheduleResponse>> GetAllClassSchedule(DayOfWeek? dayOfWeek, TimeOnly? startTime, TimeOnly? endTime, DateOnly? startDate, DateOnly? endDate, int pageNumber, int pageSize)
        {
            var classSchedule = _unitOfWork.GetRepository<ClassSchedule>().Entities.Where(a => !a.IsDeleted);
            if (dayOfWeek.HasValue)
            {
                classSchedule = classSchedule.Where(s => s.DayOfWeek == dayOfWeek.Value);
            }
            if (startTime.HasValue)
            {
                classSchedule = classSchedule.Where(s => s.StartTime >= startTime.Value);
            }
            if (endTime.HasValue)
            {
                classSchedule = classSchedule.Where(s => s.EndTime <= endTime.Value);
            }
            if (startDate.HasValue)
            {
                classSchedule = classSchedule.Where(s => s.StartDate.HasValue && s.StartDate.Value >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                classSchedule = classSchedule.Where(s => s.EndDate.HasValue && s.EndDate.Value <= endDate.Value);
            }
            var totalCount = await classSchedule.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedClassSchedule = await classSchedule
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new ClassScheduleResponse
                {
                    Id = a.Id,
                    CourseId = a.CourseId,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    RoomOrLink = a.RoomOrLink,
                }).ToListAsync();
            return paginatedClassSchedule;
        }

        public async Task<ClassScheduleResponse> GetClassScheduleById(Guid id)
        {
            var classSchedule = await _unitOfWork.GetRepository<ClassSchedule>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (classSchedule == null)
            {
                throw new Exception("Class Schedule Not Found");
            }
            var result = new ClassScheduleResponse
            {
                Id = classSchedule.Id,
                CourseId = classSchedule.CourseId,
                DayOfWeek = classSchedule.DayOfWeek,
                StartTime = classSchedule.StartTime,
                EndTime = classSchedule.EndTime,
                StartDate = classSchedule.StartDate,
                EndDate = classSchedule.EndDate,
                RoomOrLink = classSchedule.RoomOrLink,
            };
            return result;
        }

        public async Task<ClassScheduleResponse> UpdateClassSchedule(Guid id, UpdateClassScheduleRequest request)
        {
            var classSchedule = await _unitOfWork.GetRepository<ClassSchedule>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (classSchedule == null)
            {
                throw new Exception("Class Schedule Not Found");
            }
            if (request.DayOfWeek.HasValue)
            {
                classSchedule.DayOfWeek = request.DayOfWeek.Value;
            }
            if (request.StartTime.HasValue)
            {
                classSchedule.StartTime = request.StartTime.Value;
            }
            if (request.EndTime.HasValue)
            {
                classSchedule.EndTime = request.EndTime.Value;
            }
            if (request.StartDate.HasValue)
            {
                classSchedule.StartDate = request.StartDate.Value;
            }
            if (request.EndDate.HasValue)
            {
                classSchedule.EndDate = request.EndDate.Value;
            }
            if (request.RoomOrLink != null)
            {
                classSchedule.RoomOrLink = request.RoomOrLink;
            }
            if (request.CourseId.HasValue)
            {
                var checkCourse = await _unitOfWork.GetRepository<Course>().Entities.FirstOrDefaultAsync(a => a.Id == request.CourseId);
                if (checkCourse == null)
                {
                    throw new Exception("Course Not Found");
                }
                classSchedule.CourseId = request.CourseId.Value;
            }
            await _unitOfWork.GetRepository<ClassSchedule>().UpdateAsync(classSchedule);
            await _unitOfWork.SaveAsync();

            var result = new ClassScheduleResponse
            {
                Id = classSchedule.Id,
                CourseId = classSchedule.CourseId,
                DayOfWeek = classSchedule.DayOfWeek,
                StartTime = classSchedule.StartTime,
                EndTime = classSchedule.EndTime,
                StartDate = classSchedule.StartDate,
                EndDate = classSchedule.EndDate,
                RoomOrLink = classSchedule.RoomOrLink,
            };
            return result;
        }
    }
}
