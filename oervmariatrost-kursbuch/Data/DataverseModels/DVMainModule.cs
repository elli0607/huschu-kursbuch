using Microsoft.Crm.Sdk;

namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVMainModule : BaseEntity
    {
        private string nameLogical = "kubu_name";
        private string goalLogical = "kubu_goal";
        private string imageLogical = "cre56_moduleimage";

        public DVMainModule() : base(DataverseConstants.LN_MainModule)
        {
            this.AddField(Name);
            this.AddField(Goal);
            this.AddField(Image);
        }

        public string Name
        {
            get
            {
                return nameLogical;
            }
        }

        public string Goal
        {
            get
            {
                return goalLogical;
            }
        }

        public string Image
        {
            get
            {
                return imageLogical;
            }
        }
    }
}
