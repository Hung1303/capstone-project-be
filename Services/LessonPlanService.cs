using BusinessObjects;
using BusinessObjects.DTO.LessonPlan;
using BusinessObjects.DTO.Syllabus;
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
    public class LessonPlanService : ILessonPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LessonPlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<LessonPlanResponse> CreateLessonPlan(CreateLessonPlanRequest request)
        {
            var syllabus = await _unitOfWork.GetRepository<Syllabus>().Entities.FirstOrDefaultAsync(a => a.Id == request.SyllabusId);
            if (syllabus == null)
            {
                throw new Exception("Syllabus Not Found");
            }
            var lessonPlan = new LessonPlan
            {
                SyllabusId = request.SyllabusId,
                Topic = request.Topic,
                StudentTask = request.StudentTask,
                MaterialsUsed = request.MaterialsUsed,
                Notes = request.Notes,
            };
            await _unitOfWork.GetRepository<LessonPlan>().InsertAsync(lessonPlan);
            await _unitOfWork.SaveAsync();
            var result = new LessonPlanResponse
            {
                Id = lessonPlan.Id,
                SyllabusId = lessonPlan.SyllabusId,
                Topic = lessonPlan.Topic,
                StudentTask = lessonPlan.StudentTask,
                MaterialsUsed = lessonPlan.MaterialsUsed,
                Notes = lessonPlan.Notes,
            };
            return result;
        }

        public async Task<bool> DeleteLessonPlan(Guid id)
        {
            var lessonPlan = await _unitOfWork.GetRepository<LessonPlan>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (lessonPlan == null)
            {
                throw new Exception("lessonPlan Not Found");
            }
            lessonPlan.IsDeleted = true;
            await _unitOfWork.GetRepository<LessonPlan>().UpdateAsync(lessonPlan);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<LessonPlanResponse>> GetAllLessonPlan(string? searchTerm, int pageNumber, int pageSize)
        {
            var lessnPlan = _unitOfWork.GetRepository<LessonPlan>().Entities.Where(a => !a.IsDeleted);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                lessnPlan = lessnPlan.Where(c =>
                    (c.Topic != null && c.Topic.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.StudentTask != null && c.StudentTask.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.MaterialsUsed != null && c.MaterialsUsed.ToLower().Contains(searchTerm.Trim().ToLower())) ||
                    (c.Notes != null && c.Notes.ToLower().Contains(searchTerm.Trim().ToLower()))
                );
            }

            var totalCount = await lessnPlan.CountAsync();
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);
            var skipAmount = (pageNumber - 1) * pageSize;
            var paginatedLessonPlan = await lessnPlan
                .OrderByDescending(c => c.CreatedAt)
                .Skip(skipAmount)
                .Take(pageSize)
                .Select(a => new LessonPlanResponse
                {
                    Id = a.Id,
                    SyllabusId = a.SyllabusId,
                    Topic = a.Topic,
                    StudentTask = a.StudentTask,
                    MaterialsUsed = a.MaterialsUsed,
                    Notes = a.Notes,
                }).ToListAsync();
            return paginatedLessonPlan;
        }

        public async Task<LessonPlanResponse> GetLessonPlanById(Guid id)
        {
            var lessonPlan = await _unitOfWork.GetRepository<LessonPlan>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (lessonPlan == null)
            {
                throw new Exception("LessonPlan Not Found");
            }
            var result = new LessonPlanResponse
            {
                Id = lessonPlan.Id,
                SyllabusId = lessonPlan.SyllabusId,
                Topic = lessonPlan.Topic,
                StudentTask = lessonPlan.StudentTask,
                MaterialsUsed = lessonPlan.MaterialsUsed,
                Notes = lessonPlan.Notes,
            };
            return result;
        }

        public async Task<LessonPlanResponse> UpdateLessonPlan(Guid id, UpdateLessonPlanRequest request)
        {
            var lessonPlan = await _unitOfWork.GetRepository<LessonPlan>().Entities.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (lessonPlan == null)
            {
                throw new Exception("LessonPlan Not Found");
            }
            if (request.Topic != null)
            {
                lessonPlan.Topic = request.Topic;
            }
            if (request.StudentTask != null)
            {
                lessonPlan.StudentTask = request.StudentTask;
            }
            if (request.MaterialsUsed != null)
            {
                lessonPlan.MaterialsUsed = request.MaterialsUsed;
            }
            if (request.Notes != null)
            {
                lessonPlan.Notes = request.Notes;
            }
            if (request.SyllabusId.HasValue)
            {
                var checkSyllabus = await _unitOfWork.GetRepository<Syllabus>().Entities.FirstOrDefaultAsync(a => a.Id == request.SyllabusId);
                if (checkSyllabus == null)
                {
                    throw new Exception("Syllabus Not Found");
                }
                lessonPlan.SyllabusId = request.SyllabusId.Value;
            }
            await _unitOfWork.GetRepository<LessonPlan>().UpdateAsync(lessonPlan);
            await _unitOfWork.SaveAsync();

            var result = new LessonPlanResponse
            {
                Id = lessonPlan.Id,
                SyllabusId = lessonPlan.SyllabusId,
                Topic = lessonPlan.Topic,
                StudentTask = lessonPlan.StudentTask,
                MaterialsUsed = lessonPlan.MaterialsUsed,
                Notes = lessonPlan.Notes,
            };
            return result;
        }
    }
}
