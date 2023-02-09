namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVCourseHour : BaseEntity
    {
        private string statuscodeLogical = "statuscode";
        private string unitTitleLogical = "cre56_unittitle";
        private string summaryLogical = "cre56_summarycourseunit";
        private string einheitenLogical = "cre56_courseunitdate";
        private string accordingCourseLogical = "cre56_accordingcourse";

        private string homeworkLogical = "cre56_homework";
        private string learningGoalLogical = "kubu_learninggoal";
        private string importantExTipsLogical = "cre56_importantexercisetips";
        private string lifeHacksLogical = "cre56_lifehacks";
        
        private string statecodeLogical = "statecode";
        private string welcomeTextLogical = "kubu_coursebookwelcometext";

        private string kursLeiterLogical = "cre56_kursleiter";

        public DVCourseHour() : base(DataverseConstants.LN_CourseHour)
        {
            this.AddField(Statuscode);
            this.AddField(Summary);
            this.AddField(UnitTitle);
            this.AddField(CourseUnitDate);
            this.AddField(AccordingCourse);

            this.AddField(Homework, false);
            this.AddField(LearningGoal, false);
            this.AddField(ImportantExTips, false);
            this.AddField(LifeHacks, false);
        }

        public string Summary
        {
            get
            {
                return summaryLogical;
            }
        }

        public string CourseUnitDate
        {
            get
            {
                return einheitenLogical;
            }
        }

        public string Homework
        {
            get
            {
                return homeworkLogical;
            }
        }

        public string LearningGoal
        {
            get
            {
                return learningGoalLogical;
            }
        }

        public string Kursleiter
        {
            get
            {
                return kursLeiterLogical;
            }
        }

        public string ImportantExTips
        {
            get
            {
                return importantExTipsLogical;
            }
        }

        public string LifeHacks
        {
            get
            {
                return lifeHacksLogical;
            }
        }

        public string UnitTitle
        {
            get
            {
                return unitTitleLogical;
            }
        }

        public string Statuscode
        {
            get
            {
                return statuscodeLogical;
            }
        }

        public string Statecode
        {
            get
            {
                return statecodeLogical;
            }
        }

        public string AccordingCourse
        {
            get
            {
                return accordingCourseLogical;
            }
        }
    }
}
