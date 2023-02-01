namespace oervmariatrost_kursbuch.Data.DTO
{
    public class CourseDetailDTO : CourseOverviewDTO
    {
        public DateTime CourseEndDate { get; set; }
        public string WelcomeText { get; set; }
        public string Instructor { get; set; }
        public string Trainers { get; set; }
        public int MyPresense { get; set; }
        public int MyAbsence { get; set; }
        public int MyExcusedAbsence { get; set; }
        public int TotalUnits { get; set; }
    }
}
