using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
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

        public Task<IList<CourseOverviewDTO>> GetAvailableCourses(string email)
        {
            WhoAmIRequest systemUserRequest = new WhoAmIRequest();
            WhoAmIResponse systemUserResponse = (WhoAmIResponse)_serviceClient.Execute(systemUserRequest);
            Guid userId = systemUserResponse.UserId;
            throw new NotImplementedException();
        }

        public Task<CourseDetailDTO> GetCourseDetails(Guid courseId)
        {
            throw new NotImplementedException();
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

        public Task<IList<CourseUnitOverviewDTO>> GetCourseUnits(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CourseUnitSubModuleDTO>> GetSubModules(Guid unitId)
        {
            throw new NotImplementedException();
        }
    }
}
