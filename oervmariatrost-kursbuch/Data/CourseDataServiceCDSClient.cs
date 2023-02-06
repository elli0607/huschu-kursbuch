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
                "cre56_kursbildid", 
                "cre56_kursbild",
                "kubu_enablecoursebook",
                "cre56_name",
                "statuscode"
            );


            EntityCollection courses = _serviceClient.RetrieveMultiple(query);

            foreach (var yourCourse in courses.Entities)
            {
                //Todo: Add to List
                CourseOverviewDTO courseModel = new CourseOverviewDTO();
                courseModel.CourseId = Guid.Parse(this.GetAliasedAttributeAsString(course.EntityAlias + ".cre56_kursid", yourCourse));
                courseModel.BoughtCourseBook = bool.Parse(this.GetAliasedAttributeAsString(courseMember.EntityAlias + ".kubu_payedforcoursebook", yourCourse));
                courseModel.DogName = this.GetAliasedAttributeAsString(courseMember.EntityAlias + ".cre56_gefuhrterhund", yourCourse, true);
                courseModel.CourseStartDate = DateTime.Parse(this.GetAliasedAttributeAsString(course.EntityAlias + ".cre56_kursstart", yourCourse));
                courseModel.NumberOfUnits = int.Parse(this.GetAliasedAttributeAsString(course.EntityAlias + ".cre56_einheiten", yourCourse));
                courseModel.OverviewDescription = this.GetAliasedAttributeAsString(course.EntityAlias + ".cre56_beschreibung", yourCourse);
                courseModel.Title = this.GetAliasedAttributeAsString(course.EntityAlias + ".cre56_name", yourCourse);
                courseModel.UseCourseBook = bool.Parse(this.GetAliasedAttributeAsString(course.EntityAlias + ".kubu_enablecoursebook", yourCourse));

                //has to be done in some better moon phase
                if(yourCourse.Attributes.Contains(course.EntityAlias + ".cre56_kursbildid_cre56_kursbild"))
                {
                    courseModel.Picture = Convert.ToBase64String((byte[])yourCourse.GetAttributeValue<AliasedValue>(course.EntityAlias + ".cre56_kursbildid_cre56_kursbild").Value).ToString();
                }

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
            "cre56_kursbild",
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
            if(courseData.Attributes.Contains("cre56_kursbild")) { 
                courseDetail.Picture = Convert.ToBase64String(courseData.GetAttributeValue<byte[]>("cre56_kursbild")).ToString();
            }

            courseDetail.CourseEndDate = courseData.GetAttributeValue<DateTime>("cre56_kursende");
            var duration = courseData.GetAttributeValue<int>("kubu_coursedurationmin");
            courseDetail.CourseEndDate.AddMinutes(duration); 

            return courseDetail;
        }

        public async Task<CourseUnitDetailDTO> GetCourseUnit(Guid courseUnit, string email)
        {
            Entity course = _serviceClient.Retrieve("cre56_coursehour", courseUnit, new ColumnSet(true));
            CourseUnitDetailDTO courseUnitModel = new CourseUnitDetailDTO();
            courseUnitModel.LearningGoal = course.GetAttributeValue<string>("cre56_homework");
            courseUnitModel.ImportantExerciseTips = course.GetAttributeValue<string>("cre56_importantexercisetips");
            courseUnitModel.Lifehacks = course.GetAttributeValue<string>("cre56_lifehacks");
            courseUnitModel.SummaryUnit = course.GetAttributeValue<string>("cre56_summarycourseunit");
            courseUnitModel.Name = course.GetAttributeValue<string>("cre56_unittitle");

            Guid courseMemberId = this.GetCourseMemberId(course.GetAttributeValue<EntityReference>("cre56_accordingcourse").Id, email);

            return courseUnitModel;
        }

        public async Task<CourseUnitModuleDTO> GetCourseUnitModule(Guid moduleId, Guid courseUnitId)
        {
            CourseUnitModuleDTO unitModuleModel = new CourseUnitModuleDTO(); 

            var query = new QueryExpression("kubu_practicedexercise")
            {
                TopCount = 50
            };

            // Add columns to query.ColumnSet
            query.ColumnSet.AddColumns("kubu_incltheory", "kubu_submodule");

            // Add filter query.Criteria
            query.Criteria.AddCondition("kubu_mainmodule", ConditionOperator.Equal, moduleId);
            query.Criteria.AddCondition("kubu_trainingslesson", ConditionOperator.Equal, courseUnitId);

            // Add link-entity mainmodule
            var mainmodule = query.AddLink("kubu_modul", "kubu_mainmodule", "kubu_modulid");
            mainmodule.EntityAlias = "mainmodule";

            // Add columns to mainmodule.Columns
            mainmodule.Columns.AddColumns("kubu_name", "kubu_goal", "cre56_moduleimage", "kubu_modulid");
            EntityCollection practicedEx = _serviceClient.RetrieveMultiple(query);
            Entity mainModul = practicedEx.Entities.First();

            unitModuleModel.Description = this.GetAliasedAttributeAsString(mainmodule.EntityAlias + ".kubu_goal", mainModul);
            unitModuleModel.ModuleId = mainModul.Id;
            unitModuleModel.Name = this.GetAliasedAttributeAsString(mainmodule.EntityAlias + ".kubu_name", mainModul);
            unitModuleModel.IncludeTheory = mainModul.GetAttributeValue<bool>("kubu_incltheory");
            if(mainModul.Attributes.Contains("kubu_submodule"))
            {
                unitModuleModel.SubModuleId = mainModul.GetAttributeValue<EntityReference>("kubu_submodule").Id;
            }
            if (mainModul.Attributes.Contains(mainmodule.EntityAlias + ".cre56_moduleimage"))
            {
                unitModuleModel.Picture = Convert.ToBase64String((byte[])mainModul.GetAttributeValue<AliasedValue>(mainmodule.EntityAlias + ".cre56_moduleimage").Value).ToString();
            }

            var query2 = new QueryExpression("kubu_submodul")
            {
                TopCount = 50
            };

            // Add columns to query.ColumnSet
            query2.ColumnSet.AddColumns("kubu_name", "kubu_description", "kubu_order", "kubu_link", "kubu_criteriafornextstep");

            // Add filter query.Criteria
            query2.Criteria.AddCondition("kubu_mainmodul", ConditionOperator.Equal, this.GetAliasedAttributeAsString(mainmodule.EntityAlias + ".kubu_modulid", mainModul));

            // Add orders
            query2.AddOrder("kubu_order", OrderType.Ascending);

            EntityCollection subModules = _serviceClient.RetrieveMultiple(query2);
            unitModuleModel.SubModules = new List<CourseUnitSubModuleDTO>(); 

            foreach(var subModule in subModules.Entities)
            {
                CourseUnitSubModuleDTO subModel = new CourseUnitSubModuleDTO();
                subModel.Link = subModule.GetAttributeValue<string>("kubu_link");
                if(unitModuleModel.IncludeTheory || unitModuleModel.SubModuleId == subModule.Id)
                {
                    subModel.Description = subModule.GetAttributeValue<string>("kubu_description");
                    subModel.Name = subModule.GetAttributeValue<string>("kubu_name");
                }

                unitModuleModel.SubModules.Add(subModel);
            }

            return unitModuleModel; 
        }

        public async Task<IList<CourseUnitModuleDTO>> GetCourseUnitModules(Guid courseUnit)
        {
            var query_kubu_nurgeplant = false;
            List<CourseUnitModuleDTO> moduleModels = new List<CourseUnitModuleDTO>();
            // Instantiate QueryExpression query
            var query = new QueryExpression("kubu_practicedexercise")
            {
                TopCount = 50
            };

            // Add columns to query.ColumnSet
            query.ColumnSet.AddColumns("kubu_incltheory", "kubu_mainmodule", "kubu_submodule");

            // Add filter query.Criteria
            query.Criteria.AddCondition("kubu_trainingslesson", ConditionOperator.Equal, courseUnit);
            query.Criteria.AddCondition("kubu_nurgeplant", ConditionOperator.Equal, query_kubu_nurgeplant);

            // Add link-entity mainmodule
            var mainmodule = query.AddLink("kubu_modul", "kubu_mainmodule", "kubu_modulid");
            mainmodule.EntityAlias = "mainmodule";

            // Add columns to mainmodule.Columns
            mainmodule.Columns.AddColumns("kubu_name", "kubu_whatsitfor", "kubu_modulid");
            EntityCollection modules = _serviceClient.RetrieveMultiple(query); 

            foreach(var mod in modules.Entities)
            {
                CourseUnitModuleDTO model = new CourseUnitModuleDTO();
                EntityReference mModule = mod.GetAttributeValue<EntityReference>("kubu_mainmodule");
                model.ModuleId = mModule.Id;
                model.Name = mModule.Name;
                model.Description = this.GetAliasedAttributeAsString(mainmodule.EntityAlias + ".kubu_whatsitfor", mod);
                model.IncludeTheory = mod.GetAttributeValue<bool>("kubu_incltheory");

                moduleModels.Add(model);
            }

            return moduleModels;
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

        protected string GetAliasedAttributeAsString(string key, Entity entity, bool isEntityReference = false)
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
