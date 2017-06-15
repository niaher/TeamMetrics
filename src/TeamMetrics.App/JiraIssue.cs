namespace TeamMetrics.App
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class JiraIssue
	{
		public string Key { get; set; }
		public IssueType Type { get; set; }
		public string Status { get; set; }
		public string Resolution { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime? InProgressOn { get; set; }
		public DateTime? CodeReviewOn { get; set; }
		public DateTime? ReadyForDeployOn { get; set; }
		public DateTime? ResolvedOn { get; set; }
		public decimal? StoryPoints { get; set; }
		public string Assignee { get; set; }
		public string Reporter { get; set; }
		public Priority Priority { get; set; }
		public string EpicKey { get; set; }
		public string EpicName { get; set; }

		public IEnumerable<string> Sprints
			=>
			new[] { this.Sprint1, this.Sprint2, this.Sprint3, this.Sprint4, this.Sprint5, this.Sprint6 }.Where(t => !string.IsNullOrWhiteSpace(t))
			;

		public string Sprint1 { get; set; }
		public string Sprint2 { get; set; }
		public string Sprint3 { get; set; }
		public string Sprint4 { get; set; }
		public string Sprint5 { get; set; }
		public string Sprint6 { get; set; }
		
		public string FirstReviewer { get; set; }
		public string SecondReviewer { get; set; }

		public TimeSpan? TimeToDeploy => this.Status == "Done" && this.ReadyForDeployOn != null && this.ResolvedOn != null
			? this.ResolvedOn.Value.Subtract(this.ReadyForDeployOn.Value)
			: (TimeSpan?)null;

		public TimeSpan? TimeToReview => this.CodeReviewOn != null && this.ReadyForDeployOn != null
			? this.ReadyForDeployOn.Value.Subtract(this.CodeReviewOn.Value)
			: (TimeSpan?)null;

		public TimeSpan? TimeToComplete => this.Status == "Done" && this.ResolvedOn != null
			? this.ResolvedOn.Value.Subtract(this.CreatedOn)
			: (TimeSpan?)null;

		public JiraIssue Epic { get; set; }

		private static bool IsBetweenDates(DateTime date, DateTime? start, DateTime? end)
		{
			var d = date.Date.AbsoluteStart().AddHours(12);
			return d >= start?.AbsoluteStart() && (d < end?.AbsoluteStart() || end == null);
		}

		public bool InProgress(DateTime date) => IsBetweenDates(date, this.InProgressOn, this.CodeReviewOn);
		public bool InCodeReviewOn(DateTime date) => IsBetweenDates(date, this.CodeReviewOn, this.ReadyForDeployOn);
		public bool InReadyForDeploy(DateTime date) => IsBetweenDates(date, this.ReadyForDeployOn, this.ResolvedOn);
	}
}