using Microsoft.Crm.Sdk;

namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVCourseMember: BaseEntity
    {
        private string statuscodeLogical = "statuscode";
        private string besuchterKursLogical = "cre56_besuchterkurs";
        private string dogLogical = "cre56_gefuhrterhund";
        private string payedForCourseBookLogical = "kubu_payedforcoursebook";
        private string memberLogical = "cre56_hundefuhrer"; 

        public DVCourseMember() : base(DataverseConstants.LN_CourseMember)
        {
            this.AddField(Course);
            this.AddField(Dog);
            this.AddField(PayedForCoursebook);
            this.AddField(Statuscode);
            this.AddField(Member);
        }

        public string Course
        {
            get
            {
                return besuchterKursLogical;
            }
        }

        public string Dog
        {
            get
            {
                return dogLogical;
            }
        }

        public string PayedForCoursebook
        {
            get
            {
                return payedForCourseBookLogical;
            }
        }

        public string Statuscode
        {
            get
            {
                return statuscodeLogical;
            }
        }

        public string Member
        {
            get
            {
                return memberLogical;
            }
        }

    }
}
