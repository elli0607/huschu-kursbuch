using Microsoft.AspNetCore.Components;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using System.Runtime.CompilerServices;
using System.Web.Helpers;

namespace oervmariatrost_kursbuch.Data.UserDataManagement
{
    public class UserSessionDataService
    {
        private UserDataModel userData;
        private bool isAuthenticated = false;

        public UserSessionDataService()
        {
            userData = new UserDataModel();
        }

        internal void SetInitialContactData(Guid id, string fullname, string email)
        {
            this.userData.Name = fullname;
            this.userData.ContactId = id;
            this.userData.UserMail = email;
            
        }

        public bool IsAuthenticated
        {
            get
            {
                return this.isAuthenticated;
            }
        }

        public string FullName
        {
            get
            {
                return this.userData.Name;
            }
        }

        public Guid ContactId
        {
            get
            {
                return this.userData.ContactId;
            }
        }

        public List<UserCourseDetails> CourseToMemberList
        {
            get
            {
                return this.userData.CoursesOfUser;
            }
        }

        public void SetCoursesAndCoursememberIds(List<UserCourseDetails> coursesForMember)
        {
            this.userData.CoursesOfUser = coursesForMember;
            if (this.userData.ContactId != Guid.Empty && this.userData.Name != string.Empty)
            {
                this.isAuthenticated = true;
            }
        }

        public bool UserBoughtCoursebookForCourse(Guid courseId)
        {
            var courseData = this.userData.CoursesOfUser.Where(course => course.CourseId == courseId).FirstOrDefault(); 

            if(courseData != null)
            {
                return courseData.BoughtCourseBook;
            }

            return false; 
        }

        public bool UserIsAllowedToCourseBook(Guid courseId)
        {
            var course = this.userData.CoursesOfUser.Where(course => course.CourseId == courseId).FirstOrDefault(); 
            return  course != null && course.BoughtCourseBook ? true : false;
        }

        public Guid GetCourseMemberIdForCourse(Guid courseId)
        {
            var course = this.userData.CoursesOfUser.Where(course => course.CourseId == courseId).FirstOrDefault();

            return course != null ? course.CourseMemberId : Guid.Empty;
        }

        public string GetDogNameForCourse(Guid courseId)
        {
            var courseData = this.userData.CoursesOfUser.Where(course => course.CourseId == courseId).FirstOrDefault();

            if (courseData != null)
            {
                return courseData.DogName;
            }

            return string.Empty;
        }
    }
}
