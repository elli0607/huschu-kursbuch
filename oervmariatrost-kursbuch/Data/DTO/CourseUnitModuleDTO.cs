namespace oervmariatrost_kursbuch.Data.DTO
{
    public class CourseUnitModuleDTO
    {
        public Guid ModuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SubModuleId { get; set; }
        public string Picture { get; set; }
        public bool IncludeTheory { get; set; }
        public List<CourseUnitSubModuleDTO> SubModules { get; set; }
    }
}
