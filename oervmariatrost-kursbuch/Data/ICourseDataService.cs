using oervmariatrost_kursbuch.Data.DTO;

namespace oervmariatrost_kursbuch.Data
{
    public interface ICourseDataService
    {
        public IList<CourseDTO> GetAvailableCourses(string email);
        public CourseDetailDTO GetCourseDetails(Guid courseId);
       // public CourseUnitDTO GetCourseUnit(Guid courseUnit);
      //  public UnitModuleDTO GetModuleUnit(Guid moduleId);
    }
}
