using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.DTO.ClassSchedule;
using Services.Interfaces;

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
            var teacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
            if (teacher == null)
            {
                throw new Exception("teacher Not Found");
            }
            var classSchedule = new ClassSchedule
            {
                ClassName = request.ClassName,
                ClassDescription = request.ClassDescription,
                TeacherProfileId = request.TeacherProfileId,
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
                ClassName = classSchedule.ClassName,
                ClassDescription = classSchedule.ClassDescription,
                TeacherProfileId = classSchedule.TeacherProfileId,
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

        public async Task<IEnumerable<ClassScheduleResponse>> GetAllClassSchedule(Guid? teacherProfileId, DayOfWeek? dayOfWeek, TimeOnly? startTime, TimeOnly? endTime, DateOnly? startDate, DateOnly? endDate, int pageNumber, int pageSize)
        {
            var classSchedule = _unitOfWork.GetRepository<ClassSchedule>().Entities.Where(a => !a.IsDeleted);
            if (teacherProfileId.HasValue)
            {
                classSchedule = classSchedule.Where(a => a.TeacherProfileId == teacherProfileId);
            }
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
                    ClassName = a.ClassName,
                    ClassDescription = a.ClassDescription,
                    TeacherProfileId = a.TeacherProfileId,
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
                ClassName = classSchedule.ClassName,
                ClassDescription = classSchedule.ClassDescription,
                TeacherProfileId = classSchedule.TeacherProfileId,
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
            if (request.ClassName != null)
            {
                classSchedule.ClassName = request.ClassName;
            }
            if (request.ClassDescription != null)
            {
                classSchedule.ClassDescription = request.ClassDescription;
            }
            if (request.TeacherProfileId.HasValue)
            {
                var checkTeacher = await _unitOfWork.GetRepository<TeacherProfile>().Entities.FirstOrDefaultAsync(a => a.Id == request.TeacherProfileId);
                if (checkTeacher == null)
                {
                    throw new Exception("Teacher Not Found");
                }
                classSchedule.TeacherProfileId = request.TeacherProfileId.Value;
            }
            await _unitOfWork.GetRepository<ClassSchedule>().UpdateAsync(classSchedule);
            await _unitOfWork.SaveAsync();

            var result = new ClassScheduleResponse
            {
                Id = classSchedule.Id,
                ClassName = classSchedule.ClassName,
                ClassDescription = classSchedule.ClassDescription,
                TeacherProfileId = classSchedule.TeacherProfileId,
                DayOfWeek = classSchedule.DayOfWeek,
                StartTime = classSchedule.StartTime,
                EndTime = classSchedule.EndTime,
                StartDate = classSchedule.StartDate,
                EndDate = classSchedule.EndDate,
                RoomOrLink = classSchedule.RoomOrLink,
            };
            return result;
        }

        public async Task<IEnumerable<ClassScheduleResponse>> GetClassScheduleByCourse(Guid courseId, DayOfWeek? dayOfWeek, TimeOnly? startTime, TimeOnly? endTime, DateOnly? startDate, DateOnly? endDate, int pageNumber, int pageSize)
        {
            var classSchedule = _unitOfWork.GetRepository<SubjectBuilder>().Entities
                .Where(sb => sb.CourseId == courseId && !sb.ClassSchedule.IsDeleted)
                .Select(sb => sb.ClassSchedule)
                .Distinct();

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
                    ClassName = a.ClassName,
                    ClassDescription = a.ClassDescription,
                    TeacherProfileId = a.TeacherProfileId,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    RoomOrLink = a.RoomOrLink,
                }).ToListAsync();
            return paginatedClassSchedule;        
        }
    }
}
