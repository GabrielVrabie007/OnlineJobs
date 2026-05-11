using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;

namespace OnlineJobs.Application.Visitors
{
    public class AnalyticsVisitor : IApplicationVisitor<AnalyticsResult>
    {
        public AnalyticsResult VisitJobPosting(JobPosting jobPosting)
        {
            var result = new AnalyticsResult();

            if (jobPosting.Applications != null && jobPosting.Applications.Any())
            {
                result.TotalApplications = jobPosting.Applications.Count;
                result.InterviewsScheduled = jobPosting.Applications.Count(a => a.Status == ApplicationStatus.Interviewing);
                result.Hires = jobPosting.Applications.Count(a => a.Status == ApplicationStatus.Accepted);

                result.StatusBreakdown = jobPosting.Applications
                    .GroupBy(a => a.Status.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());

                var acceptedApps = jobPosting.Applications.Where(a => a.Status == ApplicationStatus.Accepted);
                if (acceptedApps.Any())
                {
                    result.TimeToHireAverage = acceptedApps
                        .Where(a => a.ReviewedDate.HasValue)
                        .Average(a => (a.ReviewedDate!.Value - a.AppliedDate).TotalDays);
                }

                result.ConversionRate = result.TotalApplications > 0
                    ? (double)result.Hires / result.TotalApplications * 100
                    : 0;
            }

            return result;
        }

        public AnalyticsResult VisitJobApplication(JobApplication application)
        {
            var result = new AnalyticsResult
            {
                TotalApplications = 1,
                StatusBreakdown = new Dictionary<string, int>
                {
                    { application.Status.ToString(), 1 }
                }
            };

            if (application.Status == ApplicationStatus.Interviewing)
            {
                result.InterviewsScheduled = 1;
            }

            if (application.Status == ApplicationStatus.Accepted)
            {
                result.Hires = 1;

                if (application.ReviewedDate.HasValue)
                {
                    result.TimeToHireAverage = (application.ReviewedDate.Value - application.AppliedDate).TotalDays;
                }
            }

            return result;
        }
    }
}
