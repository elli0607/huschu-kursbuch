using Microsoft.Xrm.Sdk;

namespace oervmariatrost_kursbuch.Data.DataverseModels
{
    public class BaseEntity
    {
        public string LogicalName { get; private set; }
        private Dictionary<string, EntityFieldAddition> FieldMapping;
        private string idLogical; 

        public BaseEntity(string logicalName)
        {
            this.LogicalName = logicalName;
            this.FieldMapping = new Dictionary<string, EntityFieldAddition>();
            this.idLogical = logicalName + "id";
            this.FieldMapping.Add(idLogical, new EntityFieldAddition());
        }

        public string Id
        {
            get
            {
                return this.idLogical;
            }
        }

        public string[] GetOverviewFieldsLogicalNames()
        {
            
            var overviewFields = this.FieldMapping.Where(kvp => kvp.Value.ShowInOverview == true);
            var fieldList = new string[overviewFields.Count()];
            for (int i = 0; i < overviewFields.Count(); i++)
            {
                fieldList[i] = overviewFields.ElementAt(i).Key; 
            }

            return fieldList; 
        }

        public string[] GetAllLogicalNamesForEntityFields()
        {
            return FieldMapping.Keys.ToArray();
        }

        public void AddField(string logicalName, bool isForOverview = true)
        {
            this.FieldMapping.Add(logicalName, new EntityFieldAddition(string.Empty, isForOverview));
        }

        public void AddFields(List<string> logicalNames, bool areForOverview = true)
        {
            foreach(var field in logicalNames)
            {
                this.FieldMapping.Add(field, new EntityFieldAddition(string.Empty, areForOverview));
            }
        }
    }

    public class EntityFieldAddition
    {
        public bool ShowInOverview;
        public string fieldValue; 

        public EntityFieldAddition(string setValue = "", bool ShowInOverview = true)
        {
            this.fieldValue = setValue;
            this.ShowInOverview = ShowInOverview;
        }
    }
}
