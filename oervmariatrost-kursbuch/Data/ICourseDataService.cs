using oervmariatrost_kursbuch.Data.DTO;

namespace oervmariatrost_kursbuch.Data
{
    public interface ICourseDataService
    {
        public Task<IList<CourseOverviewDTO>> GetAvailableCourses(string email);
        public Task<CourseDetailDTO> GetCourseDetails(Guid courseId);
        public Task<IList<CourseUnitOverviewDTO>> GetCourseUnit(Guid courseId);
      //  public UnitModuleDTO GetModuleUnit(Guid moduleId);
    }
}
