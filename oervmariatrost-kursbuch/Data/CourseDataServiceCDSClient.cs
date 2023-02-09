using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Rest;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using oervmariatrost_kursbuch.Data.DataverseModels;
using oervmariatrost_kursbuch.Data.DTO;
using oervmariatrost_kursbuch.Data.UserDataManagement;
using oervmariatrost_kursbuch.Shared;
using System.Web.Helpers;

namespace oervmariatrost_kursbuch.Data
{
    public class CourseDataServiceCDSClient : ICourseDataService
    {
        private readonly ServiceClient _serviceClient;
        private UserSessionDataService _userSessionData;

        public CourseDataServiceCDSClient(ServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
            _userSessionData = new UserSessionDataService();

        }

        #region Authentication Stuff
        public async Task InitializeUserData(string email)
        {
            if (_userSessionData.IsAuthenticated)
            {
                return;
            }


            this.RequestContactForLoginMail(email);
            this.SetCoursesForLoggedInUser();
        }
        public string GetFullNameForLoginMail(string email)
        {

            this.RequestContactForLoginMail(email);
            return _userSessionData.FullName;
        }
        private void RequestContactForLoginMail(string email)
        {
            DVContact contact = new DVContact();
            var contact_Query = new QueryExpression(contact.LogicalName);
            contact_Query.ColumnSet.AddColumns(contact.GetAllLogicalNamesForEntityFields().ToArray());
            var mailFilter = new FilterExpression(LogicalOperator.Or);
            mailFilter.AddCondition(contact.KubuLoginMail, ConditionOperator.Equal, email);
            mailFilter.AddCondition(contact.StandardEmail, ConditionOperator.Equal, email);

            contact_Query.Criteria.AddFilter(mailFilter);

            var foundContacts = _serviceClient.RetrieveMultiple(contact_Query);
            if (foundContacts.Entities != null && foundContacts.Entities.Count > 0)
            {
                var actualContact = foundContacts.Entities.First();
                _userSessionData.SetInitialContactData(actualContact.Id, actualContact.GetAttributeValue<string>(contact.Fullname), email);
            }
            else
            {
                throw new ContactNotFoundException("Du bist noch nicht bei uns im System! Bitte melde dich unter info(a)hundeschule-mariatrost.at um weitere Informationen zu erhalten");
            }
        }

        private void SetCoursesForLoggedInUser()
        {
            DVCourseMember courseMember = new DVCourseMember();
            var courseMember_Query = new QueryExpression(courseMember.LogicalName);
            courseMember_Query.ColumnSet.AddColumns(courseMember.GetAllLogicalNamesForEntityFields().ToArray());
            courseMember_Query.Criteria.AddCondition(courseMember.Member, ConditionOperator.Equal, _userSessionData.ContactId);

            var allCourses = _serviceClient.RetrieveMultiple(courseMember_Query);

            if (allCourses.Entities != null && allCourses.Entities.Count > 0)
            {
                List<UserCourseDetails> CourseToCoursememberId = new List<UserCourseDetails>();
                foreach (var courseMemberEntity in allCourses.Entities)
                {
                    UserCourseDetails details = new UserCourseDetails()
                    {
                        BoughtCourseBook = courseMemberEntity.GetAttributeValue<bool>(courseMember.PayedForCoursebook),
                        CourseId = courseMemberEntity.GetAttributeValue<EntityReference>(courseMember.Course).Id,
                        CourseMemberId = courseMemberEntity.Id,
                        DogName = courseMemberEntity.GetAttributeValue<EntityReference>(courseMember.Dog)?.Name
                    };

                    CourseToCoursememberId.Add(details);
                }

                _userSessionData.SetCoursesAndCoursememberIds(CourseToCoursememberId);
            }
            else
            {
                throw new NoCoursesForUserFoundException("Für dich sind noch keine Kurse angelegt! Es wird Zeit, dass du dich für den ersten anmeldest!");
            }
        }
        #endregion

        #region List All Courses
        public async Task<IList<CourseOverviewDTO>> GetAvailableCourses(string email)
        {
            if (!_userSessionData.IsAuthenticated)
            {
                await this.InitializeUserData(email);
                if(!_userSessionData.IsAuthenticated)
                {
                    throw new NoCoursesForUserFoundException("Irgendetwas hat da nicht geklappt! Für dich gibt es keine Kurse");
                }
            }

            List<CourseOverviewDTO> CourseList = new List<CourseOverviewDTO>();

            DVCourse course = new DVCourse();
            var courseQuery = new QueryExpression(course.LogicalName);
            courseQuery.ColumnSet.AddColumns(course.GetOverviewFieldsLogicalNames().ToArray());
            var courseIdFilter = new FilterExpression(LogicalOperator.Or);

            foreach (var courseDetails in _userSessionData.CourseToMemberList)
            {
                courseIdFilter.AddCondition(course.Id, ConditionOperator.Equal, courseDetails.CourseId);
            }

            courseQuery.AddOrder(course.Kursstart, OrderType.Descending);
            courseQuery.Criteria.AddFilter(courseIdFilter);

            EntityCollection courses = _serviceClient.RetrieveMultiple(courseQuery);

            foreach (var yourCourse in courses.Entities)
            {
                CourseOverviewDTO courseModel = new CourseOverviewDTO();
                courseModel.CourseId = yourCourse.Id;
                courseModel.BoughtCourseBook = _userSessionData.UserBoughtCoursebookForCourse(yourCourse.Id);
                courseModel.DogName = _userSessionData.GetDogNameForCourse(yourCourse.Id);

                courseModel.CourseStartDate = yourCourse.GetAttributeValue<DateTime>(course.Kursstart);
                courseModel.NumberOfUnits = yourCourse.GetAttributeValue<int>(course.NumberOfUnits);
                courseModel.OverviewDescription = yourCourse.GetAttributeValue<string>(course.Description);
                courseModel.Title = yourCourse.GetAttributeValue<string>(course.Title);
                courseModel.UseCourseBook = yourCourse.GetAttributeValue<bool>(course.EnableCourseBook);

                byte[] pic = yourCourse.GetAttributeValue<byte[]>(course.Bild);

                if (pic != null)
                {
                    //has to be done in some better moon phase
                    courseModel.Picture = Convert.ToBase64String(pic).ToString();
                }

                CourseList.Add(courseModel);
            }

            return CourseList;
        }

        #endregion

        public async Task<CourseDetailDTO> GetCourseDetails(Guid courseId, string email)
        {
            if (!this._userSessionData.IsAuthenticated || !this._userSessionData.UserIsAllowedToCourseBook(courseId))
            {
                throw new StrizziException("Nanana! Da hast du nichts verloren! Bitte bleib bei den Inhalten, die für dich bereitgestellt sind.");
            }

            DVCourse dvHelperCourse = new DVCourse();

            CourseDetailDTO courseDetail = new CourseDetailDTO();
            QueryExpression query = new QueryExpression(dvHelperCourse.LogicalName);
            query.ColumnSet.AddColumns(dvHelperCourse.GetAllLogicalNamesForEntityFields().ToArray());
            query.Criteria.AddCondition(dvHelperCourse.Id, ConditionOperator.Equal, courseId);
            Entity course = _serviceClient.Retrieve(dvHelperCourse.LogicalName, courseId, new ColumnSet(dvHelperCourse.GetAllLogicalNamesForEntityFields()));


            courseDetail.CourseEndDate = course.GetAttributeValue<DateTime>(dvHelperCourse.Kursende);
            courseDetail.CourseId = course.Id;
            courseDetail.CourseStartDate = course.GetAttributeValue<DateTime>(dvHelperCourse.Kursstart);
            courseDetail.Instructor = course.GetAttributeValue<EntityReference>(dvHelperCourse.Kursleiter).Name;

            //TODO: checken - hä?? was soll das da? 
            //courseDetail.NumberOfUnits = course.GetAttributeValue<int>("cre56_einheiten");

            courseDetail.TotalUnits = course.GetAttributeValue<int>(dvHelperCourse.NumberOfUnits);
            courseDetail.WelcomeText = course.GetAttributeValue<string>(dvHelperCourse.WelcomeText);
            courseDetail.Title = course.GetAttributeValue<string>(dvHelperCourse.Title);
            if (course.Attributes.Contains(dvHelperCourse.Bild))
            {
                courseDetail.Picture = Convert.ToBase64String(course.GetAttributeValue<byte[]>(dvHelperCourse.Bild)).ToString();
            }

            var duration = course.GetAttributeValue<int>(dvHelperCourse.Kursdauer);
            courseDetail.CourseEndDate.AddMinutes(duration);

            return courseDetail;
        }

        public async Task<IList<CourseUnitOverviewDTO>> GetCourseUnits(Guid courseId)
        {
            if (!this._userSessionData.IsAuthenticated || !this._userSessionData.UserIsAllowedToCourseBook(courseId))
            {
                throw new StrizziException("Nanana! Da hast du nichts verloren! Bitte bleib bei den Inhalten, die für dich bereitgestellt sind.");
            }

            DVCourseHour dvCourseHourHelper = new DVCourseHour();
            DVAttendence dvAttendenceHelper = new DVAttendence();
            List<CourseUnitOverviewDTO> unitList = new List<CourseUnitOverviewDTO>();
            var query_statuscode = DataverseConstants.ApprovedCourseHour; //Freigegeben

            var courseHourQuery = new QueryExpression(dvCourseHourHelper.LogicalName);
            courseHourQuery.ColumnSet.AddColumns(dvCourseHourHelper.GetOverviewFieldsLogicalNames());
            courseHourQuery.Criteria.AddCondition(dvCourseHourHelper.Statuscode, ConditionOperator.Equal, query_statuscode);
            courseHourQuery.Criteria.AddCondition(dvCourseHourHelper.AccordingCourse, ConditionOperator.Equal, courseId);

            var attendence = courseHourQuery.AddLink(dvAttendenceHelper.LogicalName, dvCourseHourHelper.Id, dvAttendenceHelper.CourseUnit);
            attendence.EntityAlias = DataverseConstants.AttendenceAlias;
            attendence.Columns.AddColumns(dvAttendenceHelper.GetAllLogicalNamesForEntityFields());
            attendence.LinkCriteria.AddCondition(dvAttendenceHelper.CourseMember, ConditionOperator.Equal, this._userSessionData.GetCourseMemberIdForCourse(courseId));

            EntityCollection result = _serviceClient.RetrieveMultiple(courseHourQuery);

            foreach (var unit in result.Entities)
            {
                CourseUnitOverviewDTO cUnit = new CourseUnitOverviewDTO();
                cUnit.Description = unit.GetAttributeValue<string>(dvCourseHourHelper.Summary);
                cUnit.MyAttendenceState = ((OptionSetValue)unit.GetAttributeValue<AliasedValue>(attendence.EntityAlias + "." + dvAttendenceHelper.AttendenceState).Value).Value;
                cUnit.ExecutionDate = unit.GetAttributeValue<DateTime>(dvCourseHourHelper.CourseUnitDate);
                cUnit.Name = unit.GetAttributeValue<string>(dvCourseHourHelper.UnitTitle);
                cUnit.UnitId = unit.Id;
                unitList.Add(cUnit);
            }

            return unitList;
        }

        public async Task<CourseUnitDetailDTO> GetCourseUnit(Guid courseUnit, Guid courseId)
        {
            if (!this._userSessionData.IsAuthenticated || !this._userSessionData.UserIsAllowedToCourseBook(courseId))
            {
                throw new StrizziException("Nanana! Da hast du nichts verloren! Bitte bleib bei den Inhalten, die für dich bereitgestellt sind.");
            }

            DVCourseHour dvCourseHourHepler = new DVCourseHour();
            Entity course = _serviceClient.Retrieve(dvCourseHourHepler.LogicalName, courseUnit, new ColumnSet(dvCourseHourHepler.GetAllLogicalNamesForEntityFields()));

            CourseUnitDetailDTO courseUnitModel = new CourseUnitDetailDTO();
            courseUnitModel.Homework = course.GetAttributeValue<string>(dvCourseHourHepler.Homework);
            courseUnitModel.LearningGoal = course.GetAttributeValue<string>(dvCourseHourHepler.LearningGoal);
            courseUnitModel.ImportantExerciseTips = course.GetAttributeValue<string>(dvCourseHourHepler.ImportantExTips);
            courseUnitModel.Lifehacks = course.GetAttributeValue<string>(dvCourseHourHepler.LifeHacks);
            courseUnitModel.SummaryUnit = course.GetAttributeValue<string>(dvCourseHourHepler.Summary);
            courseUnitModel.Name = course.GetAttributeValue<string>(dvCourseHourHepler.UnitTitle);

            DVAttendence dvAttHelper = new DVAttendence();
            var attQuery = new QueryExpression(dvAttHelper.LogicalName);
            attQuery.ColumnSet.AddColumns(dvAttHelper.GetAllLogicalNamesForEntityFields());

            // Add filter query.Criteria
            attQuery.Criteria.AddCondition(dvAttHelper.CourseUnit, ConditionOperator.Equal, courseUnit);
            attQuery.Criteria.AddCondition(dvAttHelper.CourseMember, ConditionOperator.Equal, this._userSessionData.GetCourseMemberIdForCourse(courseId));

            EntityCollection att = _serviceClient.RetrieveMultiple(attQuery);
            courseUnitModel.MyAttendenceState = att.Entities.Count > 0 ? att.Entities.First().GetAttributeValue<OptionSetValue>(dvAttHelper.AttendenceState).Value : (int)DataverseConstants.CourseAttencendeState.Abwesend;
            return courseUnitModel;
        }

        public async Task<CourseUnitModuleDTO> GetCourseUnitModule(Guid moduleId, Guid courseUnitId, Guid courseId, string email)
        {
            if (!this._userSessionData.IsAuthenticated || !this._userSessionData.UserIsAllowedToCourseBook(courseId))
            {
                throw new StrizziException("Nanana! Da hast du nichts verloren! Bitte bleib bei den Inhalten, die für dich bereitgestellt sind.");
            }

            CourseUnitModuleDTO unitModuleModel = new CourseUnitModuleDTO();

            DVPracticedEx dvPracticedExHelper = new DVPracticedEx();
            DVMainModule dvMainModuleHelper = new DVMainModule();

            var practicedEx_Query = new QueryExpression(dvPracticedExHelper.LogicalName);
            practicedEx_Query.ColumnSet.AddColumns(dvPracticedExHelper.GetAllLogicalNamesForEntityFields());
            practicedEx_Query.Criteria.AddCondition(dvPracticedExHelper.MainModule, ConditionOperator.Equal, moduleId);
            practicedEx_Query.Criteria.AddCondition(dvPracticedExHelper.TrainingsLesson, ConditionOperator.Equal, courseUnitId);

            // Add link-entity mainmodule
            var mainmodule = practicedEx_Query.AddLink(dvMainModuleHelper.LogicalName, dvPracticedExHelper.MainModule, dvMainModuleHelper.Id);
            mainmodule.EntityAlias = DataverseConstants.MainModuleAlias;
            mainmodule.Columns.AddColumns(dvMainModuleHelper.GetOverviewFieldsLogicalNames());

            EntityCollection practicedEx = _serviceClient.RetrieveMultiple(practicedEx_Query);

            Entity mainModul = practicedEx.Entities.First();

            unitModuleModel.Description = this.GetAliasedAttributeAsString(mainmodule.EntityAlias + "." + dvMainModuleHelper.Goal, mainModul);
            unitModuleModel.ModuleId = mainModul.Id;
            unitModuleModel.Name = this.GetAliasedAttributeAsString(mainmodule.EntityAlias + "." + dvMainModuleHelper.Name, mainModul);
            unitModuleModel.IncludeTheory = mainModul.GetAttributeValue<bool>(dvPracticedExHelper.InclTheory);

            if (mainModul.Attributes.Contains(dvPracticedExHelper.SubModule))
            {
                unitModuleModel.SubModuleId = mainModul.GetAttributeValue<EntityReference>(dvPracticedExHelper.SubModule).Id;
            }
            if (mainModul.Attributes.Contains(mainmodule.EntityAlias + "." + dvMainModuleHelper.Image))
            {
                unitModuleModel.Picture = Convert.ToBase64String((byte[])mainModul.GetAttributeValue<AliasedValue>(mainmodule.EntityAlias + "." + dvMainModuleHelper.Image).Value).ToString();
            }

            DVSubModule dVSubModuleHelper = new DVSubModule(); 
            var query2 = new QueryExpression(dVSubModuleHelper.LogicalName);
            query2.ColumnSet.AddColumns(dVSubModuleHelper.GetAllLogicalNamesForEntityFields());

            query2.Criteria.AddCondition(dVSubModuleHelper.MainModule, ConditionOperator.Equal, moduleId);

            // Add orders
            query2.AddOrder(dVSubModuleHelper.Order, OrderType.Ascending);

            EntityCollection subModules = _serviceClient.RetrieveMultiple(query2);
            unitModuleModel.SubModules = new List<CourseUnitSubModuleDTO>();

            foreach (var subModule in subModules.Entities)
            {
                CourseUnitSubModuleDTO subModel = new CourseUnitSubModuleDTO();
                subModel.Name = subModule.GetAttributeValue<string>(dVSubModuleHelper.Name);

                if (unitModuleModel.IncludeTheory || unitModuleModel.SubModuleId == subModule.Id)
                {
                    subModel.Link = subModule.GetAttributeValue<string>(dVSubModuleHelper.Link);
                    subModel.Description = subModule.GetAttributeValue<string>(dVSubModuleHelper.Description);
                }

                unitModuleModel.SubModules.Add(subModel);
            }

            return unitModuleModel;
        }

        public async Task<IList<CourseUnitModuleDTO>> GetCourseUnitModules(Guid courseUnit, Guid courseId, string email)
        {
            if (!this._userSessionData.IsAuthenticated || !this._userSessionData.UserIsAllowedToCourseBook(courseId))
            {
                throw new StrizziException("Nanana! Da hast du nichts verloren! Bitte bleib bei den Inhalten, die für dich bereitgestellt sind.");
            }

            var query_kubu_nurgeplant = false;
            List<CourseUnitModuleDTO> moduleModels = new List<CourseUnitModuleDTO>();
            // Instantiate QueryExpression query
            var query = new QueryExpression(DataverseConstants.LN_PracticedEx);

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

            foreach (var mod in modules.Entities)
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

    }
}
