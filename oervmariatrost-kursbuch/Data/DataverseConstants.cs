﻿namespace oervmariatrost_kursbuch.Data
{
    public class DataverseConstants
    {
        public static string LN_Contact = "contact";
        public static string LN_Course = "cre56_kurs";
        public static string LN_CourseMember = "cre56_kursgeher";
        public static string LN_CourseHour = "cre56_coursehour";
        public static string LN_PracticedEx = "kubu_practicedexercise";
        public static string LN_CourseAttendence = "kubu_coursattendence";

        public enum CourseAttencendeState
        {
            Anwesend = 781410000,
            Entschuldigt = 781410001, 
            Abwesend = 781410002
        }
    }
}
