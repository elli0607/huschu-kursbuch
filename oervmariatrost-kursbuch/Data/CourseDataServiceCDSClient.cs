using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Rest;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using oervmariatrost_kursbuch.Data.DTO;

namespace oervmariatrost_kursbuch.Data
{
    public class CourseDataServiceCDSClient : ICourseDataService
    {
        private readonly ServiceClient _serviceClient;
        public CourseDataServiceCDSClient(ServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }



        public async Task<IList<CourseOverviewDTO>> GetAvailableCourses(string email)
        {
            WhoAmIRequest systemUserRequest = new WhoAmIRequest();
            WhoAmIResponse systemUserResponse = (WhoAmIResponse)_serviceClient.Execute(systemUserRequest);
            Guid userId = systemUserResponse.UserId;
            List<CourseOverviewDTO> CourseList = new List<CourseOverviewDTO>();

            
            var query = new QueryExpression("contact");
            query.ColumnSet.AddColumns("contactid", "fullname");

            // Add filter query.Criteria
            query.Criteria.AddCondition("kubu_loginmail", ConditionOperator.Equal, email);

            // Add link-entity courseMember
            var courseMember = query.AddLink("cre56_kursgeher", "contactid", "cre56_hundefuhrer");
            courseMember.EntityAlias = "courseMember";

            // Add columns to courseMember.Columns
            courseMember.Columns.AddColumns(
                "cre56_name",
                "cre56_besuchterkurs",
                "cre56_gefuhrterhund",
                "kubu_payedforcoursebook",
                "statuscode"
            );

            // Add link-entity course
            var course = courseMember.AddLink("cre56_kurs", "cre56_besuchterkurs", "cre56_kursid");
            course.EntityAlias = "course";

            // Add columns to course.Columns
            course.Columns.AddColumns(
                "cre56_beschreibung",
                "cre56_einheiten",
                "cre56_kursstart",
                "cre56_kursid",
                "kubu_enablecoursebook",
                "cre56_name",
                "cre56_kursbildid",
                "statuscode"
            );


            EntityCollection courses = _serviceClient.RetrieveMultiple(query);

            foreach (var yourCourse in courses.Entities)
            {
                //Todo: Add to List
                CourseOverviewDTO courseModel = new CourseOverviewDTO();
                courseModel.CourseId = Guid.Parse(this.GetAttributeAsString(course.EntityAlias + ".cre56_kursid", yourCourse));
                courseModel.BoughtCourseBook = bool.Parse(this.GetAttributeAsString(courseMember.EntityAlias + ".kubu_payedforcoursebook", yourCourse));
                courseModel.DogName = this.GetAttributeAsString(courseMember.EntityAlias + ".cre56_gefuhrterhund", yourCourse, true);
                courseModel.CourseStartDate = DateTime.Parse(this.GetAttributeAsString(course.EntityAlias + ".cre56_kursstart", yourCourse));
                courseModel.NumberOfUnits = int.Parse(this.GetAttributeAsString(course.EntityAlias + ".cre56_einheiten", yourCourse));
                courseModel.OverviewDescription = this.GetAttributeAsString(course.EntityAlias + ".cre56_beschreibung", yourCourse);
                courseModel.Title = this.GetAttributeAsString(course.EntityAlias + ".cre56_name", yourCourse);
                courseModel.UseCourseBook = bool.Parse(this.GetAttributeAsString(course.EntityAlias + ".kubu_enablecoursebook", yourCourse));

                //has to be done in some better moon phase
                courseModel.Picture = this.GetAttributeAsString(course.EntityAlias + ".cre56_kursbildid", yourCourse);
                CourseList.Add(courseModel);
            }

            return CourseList;
        }

        public async Task<CourseDetailDTO> GetCourseDetails(Guid courseId, string email)
        {
            Guid courseMembId= this.GetCourseMemberId(courseId, email);
            if (courseMembId == Guid.Empty)
            {
                throw new Exception("This is not a nice move");
            }

            CourseDetailDTO courseDetail = new CourseDetailDTO();
            QueryExpression query = new QueryExpression("cre56_kurs");
            query.ColumnSet.AddColumns(
            "cre56_kursid",
            "cre56_name",
            "kubu_coursebookwelcometext",
            "kubu_enablecoursebook",
            "cre56_kursbildid",
            "cre56_kursende",
            "kubu_coursedurationmin",
            "statecode",
            "cre56_kursleiter",
            "cre56_einheiten",
            "cre56_kursstart",
            "statuscode",
            "cre56_trainerteam");
            query.Criteria.AddCondition("cre56_kursid", ConditionOperator.Equal, courseId);

            EntityCollection col = _serviceClient.RetrieveMultiple(query);

            var courseData = col.Entities.FirstOrDefault(); 
            if(courseData == null)
            {
                //ähm.. des wär doof
                return new CourseDetailDTO(); 
            }

            courseDetail.CourseEndDate = courseData.GetAttributeValue<DateTime>("cre56_kursende"); 
            courseDetail.CourseId = courseData.GetAttributeValue<Guid>("cre56_kursid");
            courseDetail.CourseStartDate = courseData.GetAttributeValue<DateTime>("cre56_kursstart");
            courseDetail.Instructor = courseData.GetAttributeValue<EntityReference>("cre56_kursleiter").Name;
            courseDetail.NumberOfUnits = courseData.GetAttributeValue<int>("cre56_einheiten");
            courseDetail.TotalUnits = courseData.GetAttributeValue<int>("cre56_einheiten");
            courseDetail.WelcomeText = courseData.GetAttributeValue<string>("kubu_coursebookwelcometext"); 
            courseDetail.Trainers = courseData.GetAttributeValue<string>("cre56_trainerteam");
            courseDetail.Title = courseData.GetAttributeValue<string>("cre56_name");
            courseDetail.Picture = courseData.GetAttributeValue<string>("cre56_kursbild_url");

            courseDetail.CourseEndDate = courseData.GetAttributeValue<DateTime>("cre56_kursende");
            var duration = courseData.GetAttributeValue<int>("kubu_coursedurationmin");
            courseDetail.CourseEndDate.AddMinutes(duration); 

            return courseDetail;
        }

        public Task<CourseUnitDetailDTO> GetCourseUnit(Guid courseUnit)
        {

            throw new NotImplementedException();
        }

        public Task<CourseUnitModuleDTO> GetCourseUnitModule(Guid moduleId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CourseUnitModuleDTO>> GetCourseUnitModules(Guid courseUnit)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<CourseUnitOverviewDTO>> GetCourseUnits(Guid courseId, string email)
        {
            var query_statuscode = 781410002; //Freigegeben
            var query_cre56_accordingcourse = courseId;
            var attendence_kubu_courseling = this.GetCourseMemberId(courseId, email);
            List<CourseUnitOverviewDTO> unitList = new List<CourseUnitOverviewDTO>(); 
           

            // Instantiate QueryExpression query
            var query = new QueryExpression("cre56_coursehour")
            {
                TopCount = 50
            };

            // Add columns to query.ColumnSet
            query.ColumnSet.AddColumns("statuscode", "cre56_unittitle", "cre56_summarycourseunit","cre56_courseunitdate");

            // Add filter query.Criteria
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, query_statuscode);
            query.Criteria.AddCondition("cre56_accordingcourse", ConditionOperator.Equal, query_cre56_accordingcourse);

            // Add link-entity attendence
            var attendence = query.AddLink("kubu_coursattendence", "cre56_coursehourid", "kubu_kurseinheit");
            attendence.EntityAlias = "attendence";

            // Add columns to attendence.Columns
            attendence.Columns.AddColumn("kubu_anwesenheitsstatus");

            // Add filter attendence.LinkCriteria
            attendence.LinkCriteria.AddCondition("kubu_courseling", ConditionOperator.Equal, attendence_kubu_courseling);

            EntityCollection result = _serviceClient.RetrieveMultiple(query);

            foreach (var unit in result.Entities)
            {
                CourseUnitOverviewDTO cUnit = new CourseUnitOverviewDTO();
                cUnit.Description = unit.GetAttributeValue<string>("cre56_summarycourseunit");
                cUnit.MyAttendenceState = ((OptionSetValue)unit.GetAttributeValue<AliasedValue>(attendence.EntityAlias + ".kubu_anwesenheitsstatus").Value).Value;
                cUnit.ExecutionDate = unit.GetAttributeValue<DateTime>("cre56_courseunitdate");
                cUnit.Name = unit.GetAttributeValue<string>("cre56_unittitle");
                cUnit.UnitId = unit.Id;
                unitList.Add(cUnit);
            }

            return unitList; 
        }

        public Task<IList<CourseUnitSubModuleDTO>> GetSubModules(Guid unitId)
        {
            throw new NotImplementedException();
        }

        public Guid GetMemberIdOfLoggedInUser(string email)
        {
            ConditionExpression memberMail = new ConditionExpression();
            memberMail.AttributeName = "kubu_loginmail";
            memberMail.Operator = ConditionOperator.Equal;
            memberMail.Values.Add(email);

            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet.AddColumns("fullname", "statuscode");
            query.Criteria.AddCondition(memberMail);

            EntityCollection result = _serviceClient.RetrieveMultiple(query);
            if (result.Entities != null && result.Entities.Count > 0)
            {
                return result.Entities.First().Id;
            }

            return Guid.Empty;
        }

        protected string GetAttributeAsString(string key, Entity entity, bool isEntityReference = false)
        {
            if (entity.Contains(key))
            {
                if (isEntityReference)
                {
                    return ((EntityReference)entity.GetAttributeValue<AliasedValue>(key).Value).Name;
                }

                return entity.GetAttributeValue<AliasedValue>(key).Value.ToString();

            }

            return "";
        }
        protected Guid GetCourseMemberId(Guid courseId, string email)
        {
            // Instantiate QueryExpression query
            var query = new QueryExpression("contact")
            {
                TopCount = 50
            };

            // Add columns to query.ColumnSet
            query.ColumnSet.AddColumn("kubu_loginmail");

            // Add filter query.Criteria
            query.Criteria.AddCondition("kubu_loginmail", ConditionOperator.Equal, email);

            // Add link-entity kursgeher
            var kursgeher = query.AddLink("cre56_kursgeher", "contactid", "cre56_hundefuhrer");
            kursgeher.EntityAlias = "kursgeher";

            // Add columns to kursgeher.Columns
            kursgeher.Columns.AddColumns("cre56_hundefuhrer", "cre56_name", "cre56_kursgeherid");

            // Add filter kursgeher.LinkCriteria
            kursgeher.LinkCriteria.AddCondition("cre56_besuchterkurs", ConditionOperator.Equal, courseId.ToString());

            var result = _serviceClient.RetrieveMultiple(query);
            if (result.Entities != null && result.Entities.Count > 0)
            {
                return (Guid)result.Entities.First().GetAttributeValue<AliasedValue>(kursgeher.EntityAlias+".cre56_kursgeherid").Value;
            }

            return Guid.Empty;
        }
    }
}
