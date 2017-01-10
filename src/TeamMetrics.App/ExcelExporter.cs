namespace TeamMetrics.App
{
	using System.Collections.Generic;
	using global::TeamMetrics.App.Excel;
	using OfficeOpenXml;

	internal static class ExcelExporter
	{
		public static void Export(this ExcelWorksheet worksheet, IEnumerable<SprintMetrics> sprints)
		{
			var results = new List<Metric>();

			foreach (var sprint in sprints)
			{
				foreach (var metric in sprint.StoryPointsByPriority)
				{
					results.Add(Metric.New(sprint, metric));
				}

				foreach (var metric in sprint.StoryPointsByEpic)
				{
					results.Add(Metric.New(sprint, metric));
				}
			}

			worksheet.Write(results);
		}

		private class Metric
		{
			public string Sprint { get; set; }
			public string Segment { get; set; }
			public decimal Open { get; set; }
			public decimal InProgress { get; set; }
			public decimal InCodeReview { get; set; }
			public decimal ReadyForDeploy { get; set; }
			public decimal Done { get; set; }

			public static Metric New<T>(SprintMetrics sprint, StoryPointStats<T> metric)
			{
				return new Metric
				{
					Sprint = sprint.Sprint,
					InCodeReview = metric.InCodeReview,
					ReadyForDeploy = metric.ReadyForDeploy,
					Segment = metric.Segment.ToString(),
					InProgress = metric.InProgress,
					Open = metric.Open,
					Done = metric.Done
				};
			}
		}
	}
}