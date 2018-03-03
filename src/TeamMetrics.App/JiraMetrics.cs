namespace TeamMetrics.App
{
	using System.Collections.Generic;
	using global::TeamMetrics.App.Excel;

	public class TeamMetrics
	{
		/// <summary>
		/// Average number of days in code review.
		/// </summary>
		[ExcelColumn(OrderIndex = 30)]
		public double AverageTimeInCodeReview { get; set; }

		/// <summary>
		/// Average number days to deploy a change.
		/// </summary>
		[ExcelColumn(OrderIndex = 31)]
		public double AverageTimeInReadyForDeploy { get; set; }

		/// <summary>
		/// Average number of days from the time a bug reported until the fix is deployed.
		/// </summary>
		[ExcelColumn(OrderIndex = 32)]
		public double AverateTimeToFixBug { get; set; }

		[ExcelColumn(OrderIndex = 10, HeaderBackgroundColor = "#f00")]
		public int NewBugs { get; set; }

		[ExcelColumn(OrderIndex = 15)]
		public int NewIssues { get; set; }

		public List<PersonStats> PersonStats { get; set; } = new List<PersonStats>();

		[ExcelColumn(OrderIndex = 11, HeaderBackgroundColor = "#f00")]
		public int ResolvedBugs { get; set; }

		[ExcelColumn(OrderIndex = 16)]
		public int ResolvedIssues { get; set; }

		/// <summary>
		/// Number of "done" points for which the tasks were picked before the reporting period.
		/// </summary>
		[ExcelColumn(OrderIndex = 24, HeaderBackgroundColor = "#00f")]
		public decimal StoryPointsCarriedOver { get; set; }

		[ExcelColumn(OrderIndex = 23, HeaderBackgroundColor = "#00f")]
		public decimal StoryPointsDone { get; set; }

		[ExcelColumn(OrderIndex = 21, HeaderBackgroundColor = "#00f")]
		public decimal StoryPointsInCodeReview { get; set; }

		[ExcelColumn(OrderIndex = 20, HeaderBackgroundColor = "#00f")]
		public decimal StoryPointsInProgress { get; set; }

		[ExcelColumn(OrderIndex = 22, HeaderBackgroundColor = "#00f")]
		public decimal StoryPointsReadyForDeploy { get; set; }
	}
}