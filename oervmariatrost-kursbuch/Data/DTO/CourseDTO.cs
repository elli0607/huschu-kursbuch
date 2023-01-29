namespace oervmariatrost_kursbuch.Data.DTO
{
    public class CourseDTO
    {
        public Guid CourseId { get; set; }
        public string Picture { get; set; }
        public string Title { get; set; }
        public string OverviewDescription { get; set; }
        public bool UseCourseBook { get; set; }

    }
}
