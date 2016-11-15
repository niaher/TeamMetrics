namespace TeamMetrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MetricCalculator
	{
		public static TeamMetrics CalculateStats(List<JiraIssue> issues, DateTime start, DateTime end)
		{
			var absoluteEnd = end.AbsoluteEnd();
			var absoluteStart = start.AbsoluteStart();

			var createdDuring = issues.Where(t => t.CreatedOn >= absoluteStart && t.CreatedOn <= absoluteEnd).ToList();
			var resolvedDuring = issues.Where(t => t.ResolvedOn >= absoluteStart && t.ResolvedOn <= absoluteEnd).ToList();
			var monthAgo = absoluteEnd.AddDays(-30).AbsoluteEnd();

			var stats = new TeamMetrics
			{
				IssueCount = issues.Count,
				NewBugs = createdDuring.Count(t => t.Type == IssueType.Bug),
				ResolvedBugs = resolvedDuring.Count(t => t.Type == IssueType.Bug),
				StoryPointsDone = resolvedDuring.Sum(t => t.StoryPoints ?? 0),
				StoryPointsInCodeReview = issues.Where(t => t.InCodeReviewOn(absoluteEnd)).Sum(t => t.StoryPoints ?? 0),
				StoryPointsInProgress = issues.Where(t => t.InProgress(absoluteEnd)).Sum(t => t.StoryPoints ?? 0),
				StoryPointsReadyForDeploy = issues.Where(t => t.InReadyForDeploy(absoluteEnd)).Sum(t => t.StoryPoints ?? 0),
				AverageTimeInCodeReview = issues
					.Where(t => t.ReadyForDeployOn <= absoluteEnd && t.ReadyForDeployOn >= monthAgo)
					.Where(t => t.CodeReviewOn != null)
					.Average(t => t.ReadyForDeployOn.Value.Subtract(t.CodeReviewOn.Value).Days),
				AverageTimeInReadyForDeploy = issues
					.Where(t => t.ResolvedOn <= absoluteEnd && t.ResolvedOn >= monthAgo)
					.Where(t => t.ReadyForDeployOn != null)
					.Average(t => t.ResolvedOn.Value.Subtract(t.ReadyForDeployOn.Value).Days)
			};

			// Get all people participating in the scrum.
			var people = issues.Select(t => t.Assignee).Distinct().Union(issues.Select(t => t.Reporter).Distinct()).Distinct().ToList();

			foreach (var person in people)
			{
				var resolved = resolvedDuring
					.Where(t => t.Assignee == person)
					.ToList();
				
				var personStats = new PersonStats
				{
					Name = person,
					BugsFixed = resolved.Count(t => t.Type == IssueType.Bug),
					StoryPointsDone = resolved.Sum(t => t.StoryPoints ?? 0),
					BugsReported = createdDuring.Count(t => t.Reporter == person),
					StoryPointsInProgress = issues.Where(t => t.Assignee == person && t.InProgress(end)).Sum(t => t.StoryPoints ?? 0),

					// Code reviews will count only once the story was done.
					StoryPointsReviewed = resolvedDuring
						.Where(t => t.FirstReviewer == person || t.SecondReviewer == person)
						.Sum(t => t.StoryPoints ?? 0)
				};

				stats.PersonStats.Add(personStats);
			}

			stats.PersonStats = stats.PersonStats.OrderByDescending(t => t.Score).ToList();

			return stats;
		}
	}
}