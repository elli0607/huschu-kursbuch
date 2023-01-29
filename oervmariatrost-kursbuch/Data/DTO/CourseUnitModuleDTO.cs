namespace oervmariatrost_kursbuch.Data.DTO
{
    public class CourseUnitModuleDTO
    {
        public Guid ModuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public string Picture { get; set; }
    }
}
