namespace oervmariatrost_kursbuch.Data.DTO
{
    public class CourseUnitOverviewDTO
    {
        public Guid UnitId { get; set; }
        public string Name{ get; set; }
        public string Description { get; set; }
        public DateTime ExecutionDate { get; set; }
    }
}
