using oervmariatrost_kursbuch.Data.DTO;

namespace oervmariatrost_kursbuch.Data
{
    public interface ICourseDataService
    {
        Task<IList<CourseOverviewDTO>> GetAvailableCourses(string email);
        Task<CourseDetailDTO> GetCourseDetails(Guid courseId);
        Task<IList<CourseUnitOverviewDTO>> GetCourseUnits(Guid courseId);
        Task<CourseUnitDetailDTO> GetCourseUnit(Guid courseUnit);
        Task<IList<CourseUnitModuleDTO>> GetCourseUnitModules(Guid courseUnit);
        Task<CourseUnitModuleDTO> GetCourseUnitModule(Guid moduleId);

        Task<IList<CourseUnitSubModuleDTO>> GetSubModules(Guid unitId);
    }
}
