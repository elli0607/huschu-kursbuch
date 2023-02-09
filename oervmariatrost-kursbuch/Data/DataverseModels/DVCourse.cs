namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVCourse : BaseEntity
    {
        private string nameLogical = "cre56_name";
        private string beschreibungLogical = "cre56_beschreibung";
        private string einheitenLogiccal = "cre56_einheiten";

        private string kursStartLogical = "cre56_kursstart";
        private string kursEndeLogical = "cre56_kursende";
        private string kursDauerLogical = "kubu_coursedurationmin";

        private string kursBildLogical = "cre56_kursbild";
        private string enableCourseBookLogical = "kubu_enablecoursebook";
        private string statuscodeLogical = "statuscode";
        private string statecodeLogical = "statecode";
        private string welcomeTextLogical = "kubu_coursebookwelcometext";

        private string kursLeiterLogical = "cre56_kursleiter";

        public DVCourse() : base(DataverseConstants.LN_Course)
        {
            this.AddField(Description);
            this.AddField(NumberOfUnits);
            this.AddField(Kursstart);
            this.AddField(kursBildLogical);
            this.AddField(EnableCourseBook);
            this.AddField(Title);
            this.AddField(Statuscode);

            this.AddField(WelcomeText, false);
            this.AddField(Kursende, false);
            this.AddField(Kursdauer, false);
            this.AddField(Kursleiter, false);
            this.AddField(Statecode, false);
        }

        public string Description
        {
            get
            {
                return beschreibungLogical; 
            }
        }

        public string NumberOfUnits
        {
            get
            {
                return einheitenLogiccal;
            }
        }

        public string Kursstart
        {
            get
            {
                return kursStartLogical;
            }
        }

        public string Kursende
        {
            get
            {
                return kursEndeLogical;
            }
        }

        public string Kursdauer
        {
            get
            {
                return kursDauerLogical;
            }
        }

        public string Kursleiter
        {
            get
            {
                return kursLeiterLogical;
            }
        }

        public string Bild
        {
            get
            {
                return kursBildLogical;
            }
        }

        public string EnableCourseBook
        {
            get
            {
                return enableCourseBookLogical;
            }
        }

        public string Title
        {
            get
            {
                return nameLogical;
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

        public string WelcomeText
        {
            get
            {
                return welcomeTextLogical;
            }
        }
    }
}
