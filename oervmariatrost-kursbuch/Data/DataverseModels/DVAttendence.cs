using Microsoft.Crm.Sdk;

namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVAttendence : BaseEntity
    {
        private string courseUnitLogical = "kubu_kurseinheit";
        private string attendenceLogical = "kubu_anwesenheitsstatus";
        private string courseMemberLogical = "kubu_courseling";

        public DVAttendence() : base(DataverseConstants.LN_CourseAttendence)
        {
            this.AddField(CourseUnit);
            this.AddField(AttendenceState);
            this.AddField(CourseMember);
        }

        public string CourseUnit
        {
            get
            {
                return courseUnitLogical;
            }
        }

        public string AttendenceState
        {
            get
            {
                return attendenceLogical;
            }
        }

        public string CourseMember
        {
            get
            {
                return courseMemberLogical; 
            }
        }
    }
}
