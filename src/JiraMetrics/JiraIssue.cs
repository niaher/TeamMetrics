namespace TeamMetrics
{
	using System;

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

		public string FirstReviewer { get; set; }
		public string SecondReviewer { get; set; }

		public bool InProgress(DateTime date) => IsBetweenDates(date, this.InProgressOn, this.CodeReviewOn);
		public bool InCodeReviewOn(DateTime date) => IsBetweenDates(date, this.CodeReviewOn, this.ReadyForDeployOn);
		public bool InReadyForDeploy(DateTime date) => IsBetweenDates(date, this.ReadyForDeployOn, this.ResolvedOn);

		private static bool IsBetweenDates(DateTime date, DateTime? start, DateTime? end)
		{
			var d = date.Date.AbsoluteStart().AddHours(12);
			return d >= start?.AbsoluteStart() && (d < end?.AbsoluteStart() || end == null);
		}
	}
}