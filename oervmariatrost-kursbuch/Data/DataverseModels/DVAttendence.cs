using Microsoft.Crm.Sdk;

namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVAttendence : BaseEntity
    {
        private string courseUnitLogical = "kubu_kurseinheit";
        private string attendenceLogical = "kubu_anwesenheitsstatus";
        private string courseMemberLogical = "kubu_courseling";
        private string personalNotesLogical = "kubu_anmerkungzureinheit";


        public DVAttendence() : base(DataverseConstants.LN_CourseAttendence)
        {
            this.AddField(CourseUnit);
            this.AddField(AttendenceState);
            this.AddField(CourseMember);
            this.AddField(PersonalNotes);

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
        public string PersonalNotes
        {
            get
            {
                return personalNotesLogical;
            }
        }
    }
}
