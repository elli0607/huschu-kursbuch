using oervmariatrost_kursbuch.Data.DTO;

namespace oervmariatrost_kursbuch.Data
{
    public interface ICourseDataService
    {
        Task<IList<CourseOverviewDTO>> GetAvailableCourses(string email);
        Task<CourseDetailDTO> GetCourseDetails(Guid courseId, string email);
        Task<IList<CourseUnitOverviewDTO>> GetCourseUnits(Guid courseId, string email);
        Task<CourseUnitDetailDTO> GetCourseUnit(Guid courseUnit, Guid courseId, string email);
        Task<IList<CourseUnitModuleDTO>> GetCourseUnitModules(Guid courseUnit, Guid courseId, string email);
        Task<CourseUnitModuleDTO> GetCourseUnitModule(Guid moduleId, Guid courseUnitId, Guid courseId, string email);

    }
}
