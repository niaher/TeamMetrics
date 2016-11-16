namespace TeamMetrics.App
{
	using System.Collections.Generic;

	public class TeamMetrics
	{
		public int IssueCount { get; set; }
		public int NewBugs { get; set; }
		public decimal StoryPointsDone { get; set; }
		public int ResolvedBugs { get; set; }
		public decimal StoryPointsInCodeReview { get; set; }
		public decimal StoryPointsInProgress { get; set; }
		public decimal StoryPointsReadyForDeploy { get; set; }

		/// <summary>
		/// Average number of days in code review.
		/// </summary>
		public double AverageTimeInCodeReview { get; set; }

		/// <summary>
		/// Average number days to deploy a change.
		/// </summary>
		public double AverageTimeInReadyForDeploy { get; set; }

		/// <summary>
		/// Average number of days from the time a bug reported until the fix is deployed.
		/// </summary>
		public double AverateTimeToFixBug { get; set; }

		public List<PersonStats> PersonStats { get; set; } = new List<PersonStats>();
	}
}