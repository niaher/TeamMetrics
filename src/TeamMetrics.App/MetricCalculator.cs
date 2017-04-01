namespace TeamMetrics.App
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
			var resolvedDuring = issues.Where(t => t.ResolvedOn >= absoluteStart && t.ResolvedOn <= absoluteEnd && t.Status == "Done").ToList();
			var monthAgo = absoluteEnd.AddDays(-30).AbsoluteEnd();

			var stats = new TeamMetrics
			{
				IssueCount = issues.Count,
				NewBugs = createdDuring.Count(t => t.Type == IssueType.Bug),
				ResolvedBugs = resolvedDuring.Count(t => t.Type == IssueType.Bug),
				StoryPointsDone = resolvedDuring.Sum(t => t.StoryPoints ?? 0),
				StoryPointsCarriedOver = resolvedDuring.Where(t => t.InProgressOn < start).Sum(t => t.StoryPoints ?? 0),
				StoryPointsInCodeReview = issues.Where(t => t.InCodeReviewOn(absoluteEnd)).Sum(t => t.StoryPoints ?? 0),
				StoryPointsInProgress = issues.Where(t => t.InProgress(absoluteEnd)).Sum(t => t.StoryPoints ?? 0),
				StoryPointsReadyForDeploy = issues.Where(t => t.InReadyForDeploy(absoluteEnd)).Sum(t => t.StoryPoints ?? 0),
				AverateTimeToFixBug = issues
					.Where(t => t.ResolvedOn <= absoluteEnd && t.ResolvedOn >= monthAgo)
					.Where(t => t.Type == IssueType.Bug)
					.Select(t => t.TimeToComplete)
					.Where(t => t != null)
					.DefaultIfEmpty(TimeSpan.FromDays(0))
					.Average(t => t.Value.TotalDays),
				AverageTimeInCodeReview = issues
					.Where(t => t.ReadyForDeployOn <= absoluteEnd && t.ReadyForDeployOn >= monthAgo)
					.Select(t => t.TimeToReview)
					.Where(t => t != null)
					.DefaultIfEmpty(TimeSpan.FromDays(0))
					.Average(t => t.Value.TotalDays),
				AverageTimeInReadyForDeploy = issues
					.Where(t => t.ResolvedOn <= absoluteEnd && t.ResolvedOn >= monthAgo)
					.Select(t => t.TimeToDeploy)
					.Where(t => t != null)
					.DefaultIfEmpty(TimeSpan.FromDays(0))
					.Average(t => t.Value.TotalDays)
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
					StoryPointsInProgress =
						issues.Where(t => t.Assignee == person && (t.InProgress(end) || t.InCodeReviewOn(end) || t.InReadyForDeploy(end))).Sum(t => t.StoryPoints ?? 0),

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

		public static SprintMetrics CalculateSprintMetrics(List<JiraIssue> issues, string sprint)
		{
			var storyIssues = issues.Where(t => t.Sprints.Any(s => s.Equals(sprint, StringComparison.OrdinalIgnoreCase))).ToList();

			return new SprintMetrics
			{
				Sprint = sprint,
				StoryPointsByPriority = GetStoryPointStatsBySegment(storyIssues, t => t.Priority).OrderByDescending(t => t.Segment).ToList(),
				StoryPointsByEpic = GetStoryPointStatsBySegment(storyIssues, t => t.EpicKey)
					.ForEachItem(a => a.Segment = issues.FirstOrDefault(t => t.Key == a.Segment)?.EpicName ?? a.Segment)
					.OrderByDescending(t => t.Done)
					.ThenByDescending(t => t.ReadyForDeploy)
					.ThenByDescending(t => t.InCodeReview)
					.ToList()
			};
		}

		private static List<StoryPointStats<T>> GetStoryPointStatsBySegment<T>(
			List<JiraIssue> jiraIssues,
			Func<JiraIssue, T> getSegment)
		{
			var result = new List<StoryPointStats<T>>();

			var segments = jiraIssues.Select(getSegment).Distinct();

			foreach (T segmentKey in segments)
			{
				var bySegment = jiraIssues
					.Where(t => getSegment(t).Equals(segmentKey))
					.ToList();

				result.Add(new StoryPointStats<T>
				{
					Segment = segmentKey,
					Open = bySegment.Where(t => t.Status == "Open").Sum(t => t.StoryPoints ?? 0),
					InProgress = bySegment.Where(t => t.Status == "In Progress").Sum(t => t.StoryPoints ?? 0),
					InCodeReview = bySegment.Where(t => t.Status == "Code review/Testing").Sum(t => t.StoryPoints ?? 0),
					ReadyForDeploy = bySegment.Where(t => t.Status == "Ready for deploy").Sum(t => t.StoryPoints ?? 0),
					Done = bySegment.Where(t => t.Status == "Done").Sum(t => t.StoryPoints ?? 0)
				});
			}

			return result.ToList();
		}
	}
}