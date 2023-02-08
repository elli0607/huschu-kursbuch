using Microsoft.AspNetCore.Mvc;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Runtime.CompilerServices;

namespace oervmariatrost_kursbuch.Data
{
    public static class Dataverse_Helper
    {
        public static string CourseMemberAlias = "courseMember"; 
        public static string CourseAlias = "course";

        public static T GetAliasedAttribute<T>(string alias, string property, Entity entity)
        {
            string key = alias + "." + property;
            if (entity.Contains(key))
            {
                return ((T)entity.GetAttributeValue<AliasedValue>(key).Value);
            }

            return default(T);
        }

       

        public static QueryExpression GetCourseMemberQuery(string userMail, string courseMemberAlias)
        {
            var query = new QueryExpression(DataverseConstants.LN_Contact);
            query.ColumnSet.AddColumns("contactid", "fullname");

            // Add filter query.Criteria
            query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, userMail);

            // Add link-entity courseMember
            var courseMember = query.AddLink(DataverseConstants.LN_CourseMember, "contactid", "cre56_hundefuhrer");
            courseMember.EntityAlias = courseMemberAlias;

            // Add columns to courseMember.Columns
            courseMember.Columns.AddColumns(
                "cre56_besuchterkurs",
                "cre56_gefuhrterhund",
                "kubu_payedforcoursebook",
                "statuscode"
            );

            return query;
        }

        public static QueryExpression GetCourseMemberIdQuery(Guid courseId, string email)
        {
            var query = GetCourseMemberQuery(email, CourseMemberAlias);
            query.LinkEntities.Where(le => le.EntityAlias == CourseMemberAlias).First().LinkCriteria.AddCondition("cre56_besuchterkurs", ConditionOperator.Equal, courseId.ToString());
            return query;
        }
    }
}
