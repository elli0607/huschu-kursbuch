using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace oervmariatrost_kursbuch.Data.DTO
{
    public class CourseOverviewDTO
    {
        //Course
        public Guid CourseId { get; set; }
        public string Picture { get; set; }
        public string Title { get; set; }
        public string OverviewDescription { get; set; }
        public bool UseCourseBook { get; set; }
        public DateTime CourseStartDate { get; set; }
        public int NumberOfUnits { get; set; }

        //Coursemember 
        public bool BoughtCourseBook { get; set; }
        public string DogName { get; set; }

    }
}
