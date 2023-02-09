using Microsoft.Crm.Sdk;

namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVSubModule : BaseEntity
    {
        private string nameLogical = "kubu_name";
        private string descriptionLogical = "kubu_description";
        private string orderLogical = "kubu_order";
        private string mainModuleLogical = "kubu_mainmodul";
        private string linkLogical = "kubu_link";
        private string nextStepLogical = "kubu_criteriafornextstep"; 

        public DVSubModule() : base(DataverseConstants.LN_SubModule)
        {
            this.AddField(Name);
            this.AddField(Description);
            this.AddField(Order);
            this.AddField(Link);
            this.AddField(NextStep);
            this.AddField(MainModule);
        }

        public string MainModule
        {
            get
            {
                return mainModuleLogical;
            }
        }

        public string Name
        {
            get
            {
                return nameLogical;
            }
        }

        public string Description
        {
            get
            {
                return descriptionLogical;
            }
        }

        public string Order
        {
            get
            {
                return orderLogical;
            }
        }
        public string NextStep
        {
            get
            {
                return nextStepLogical;
            }
        }
        public string Link
        {
            get
            {
                return linkLogical;
            }
        }
    }
}
