namespace OnlineJobs.Application.Visitors
{
    public class AnalyticsResult
    {
        public int TotalViews { get; set; }
        public int TotalApplications { get; set; }
        public int InterviewsScheduled { get; set; }
        public int Hires { get; set; }
        public double ConversionRate { get; set; }
        public double TimeToHireAverage { get; set; }
        public Dictionary<string, int> StatusBreakdown { get; set; }

        public AnalyticsResult()
        {
            StatusBreakdown = new Dictionary<string, int>();
        }

        public double GetApplicationRate()
        {
            return TotalViews > 0 ? (double)TotalApplications / TotalViews * 100 : 0;
        }

        public double GetInterviewRate()
        {
            return TotalApplications > 0 ? (double)InterviewsScheduled / TotalApplications * 100 : 0;
        }

        public double GetHireRate()
        {
            return InterviewsScheduled > 0 ? (double)Hires / InterviewsScheduled * 100 : 0;
        }
    }
}
