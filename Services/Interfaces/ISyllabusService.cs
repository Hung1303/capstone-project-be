using BusinessObjects.DTO.Course;
using BusinessObjects.DTO.Syllabus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISyllabusService
    {
        Task<SyllabusResponse> CreateSyllabus(CreateSyllabusRequest request);
        Task<SyllabusResponse> UpdateSyllabus(Guid id, UpdateSyllabusRequest request);
        Task<IEnumerable<SyllabusResponse>> GetAllSyllabus(string? searchTerm, int pageNumber, int pageSize);
        Task<SyllabusResponse> GetSyllabusById(Guid id);
        Task<bool> DeleteSyllabus(Guid id);
    }
}
