using Microsoft.Crm.Sdk;

namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class DVContact : BaseEntity
    {
        private string fullNameLogical = "fullname";
        private string standardEmailLogical = "emailaddress1";
        private string kubuLoginMailLogical = "kubu_loginmail";

        public DVContact() : base(DataverseConstants.LN_Contact)
        {
            this.AddField(fullNameLogical);
            this.AddField(standardEmailLogical);
            this.AddField(kubuLoginMailLogical);
        }

        public string Fullname
        {
            get
            {
                return fullNameLogical;
            }
        }

        public string StandardEmail
        {
            get
            {
                return standardEmailLogical;
            }
        }

        public string KubuLoginMail
        {
            get
            {
                return kubuLoginMailLogical;
            }
        }
    }
}
