using System.Collections.Generic;

namespace SchulIT.UntisExport.Supervisions
{
    public class Supervision
    {
        public string Teacher { get; set; }

        public string Location { get; set; }

        /// <summary>
        /// The number of the weekday, starting with 1 (=Monday)
        /// </summary>
        public int WeekDay { get; set; }

        /// <summary>
        /// The lesson which the supervision is before.
        /// </summary>
        public int Lesson { get; set; }

        /// <summary>
        /// Duration of the supervision in minutes.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The list of weeks the supervision is planned. 
        /// Important: This field is null in case the supervision takes place every week.
        /// </summary>
        public ICollection<int> Weeks { get; set; }
    }
}
