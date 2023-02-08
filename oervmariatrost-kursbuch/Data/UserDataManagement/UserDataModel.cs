namespace oervmariatrost_kursbuch.Data.UserDataManagement
{
    public class UserDataModel
    {
        public string UserMail { get; set; }

        public string Name { get; set; }

        public Guid ContactId { get; set; }
        
        public List<UserCourseDetails> CoursesOfUser { get; set; }
    }

    public class UserCourseDetails
    {
        public Guid CourseId { get; set; }

        public Guid CourseMemberId { get; set; }

        public bool BoughtCourseBook { get; set; }

        public string DogName { get; set; }
    }
}
