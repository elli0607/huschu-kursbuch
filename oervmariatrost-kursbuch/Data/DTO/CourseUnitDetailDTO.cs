namespace oervmariatrost_kursbuch.Data.DTO
{
    public class CourseUnitDetailDTO : CourseUnitOverviewDTO
    {
        public string SummaryUnit { get; set; }
        public string LearningGoal { get; set; }
        public string ImportantExerciseTips { get; set;}
        public string Lifehacks { get; set;}

        public string Homework { get; set; }

        public string MyPersonalNotes { get; set; }
    }
}
