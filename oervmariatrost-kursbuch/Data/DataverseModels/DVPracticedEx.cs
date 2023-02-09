using Microsoft.Crm.Sdk;

namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVPracticedEx : BaseEntity
    {
        private string mainModuleLogical = "kubu_mainmodule";
        private string trainingsLessonLogical = "kubu_trainingslesson";
        private string inclTheoryLogical = "kubu_incltheory";
        private string subModuleLogical = "kubu_submodule";

        public DVPracticedEx() : base(DataverseConstants.LN_PracticedEx)
        {
            this.AddField(MainModule);
            this.AddField(TrainingsLesson);
            this.AddField(InclTheory);
            this.AddField(SubModule);
        }

        public string MainModule
        {
            get
            {
                return mainModuleLogical;
            }
        }

        public string TrainingsLesson
        {
            get
            {
                return trainingsLessonLogical;
            }
        }

        public string InclTheory
        {
            get
            {
                return inclTheoryLogical;
            }
        }

        public string SubModule
        {
            get
            {
                return subModuleLogical;
            }
        }
    }
}
