using Microsoft.Identity.Client;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.DataAnnotations;

namespace oervmariatrost_kursbuch.Data.DTO
{
    public class CourseOverviewDTO
    {
        //Course
        [AttributeLogicalName("cre56_kursid")]
        public Guid CourseId { get; set; }

        public string Picture { get; set; }
        public string Title { get; set; }
        public string OverviewDescription { get; set; }
        public bool UseCourseBook { get; set; }
        public DateTime CourseStartDate { get; set; }
        public int NumberOfUnits { get; set; }

        //Coursemember 
        [AttributeLogicalName("kubu_payedforcoursebook")]
        public bool BoughtCourseBook { get; set; }
        public string DogName { get; set; }

    }
}
