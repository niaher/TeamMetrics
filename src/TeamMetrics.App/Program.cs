namespace TeamMetrics.App
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using CsvHelper;
	using global::TeamMetrics.App.Excel;
	using OfficeOpenXml;

	public class Program
	{
		public static void Main(string[] args)
		{
			var issues = ReadJiraCsvFile("jira.csv");

			ByTimePeriod(issues);

			//BySprint(issues);

			ExportMetrics(issues);

			Console.WriteLine("Enter any key to exit...");
			Console.ReadKey();
		}

		private static void ExportMetrics(List<JiraIssue> issues)
		{
			using (var workbook = new ExcelPackage())
			{
				// By priority.
				var byPriority = issues
					.SelectMany(t => t.Sprints)
					.Distinct()
					.Select(t => MetricCalculator.CalculateSprintMetrics(issues, t))
					.Select(sprint => sprint.StoryPointsByPriority.Select(stat => new WorksheetRow<StoryPointStats<Priority>>(sprint.Sprint, stat)))
					.SelectMany(t => t.ToList())
					.ToList();

				workbook.Workbook.Worksheets.Add("Sprints by priority").Write(byPriority, "Sprint");
				
				// By epics.
				var byEpics = issues
					.SelectMany(t => t.Sprints)
					.Distinct()
					.Select(t => MetricCalculator.CalculateSprintMetrics(issues, t))
					.Select(sprint => sprint.StoryPointsByEpic.Select(stat => new WorksheetRow<StoryPointStats<string>>(sprint.Sprint, stat)))
					.SelectMany(t => t.ToList())
					.ToList();

				workbook.Workbook.Worksheets.Add("Sprints by epics").Write(byEpics, "Sprint");

				// Calculate weekly metrics.
				var week1 = issues.Min(t => t.CreatedOn).AddDays(1).Previous(DayOfWeek.Sunday);
				var week = new TimeSpan(7, 0, 0, 0);
				var weeks = week1.Enumerate(week, DateTime.Today).ToList();
				var weekMetrics = weeks
					.Select(t => new WorksheetRow<TeamMetrics>(t.ToString("yyyy-MM-dd"), MetricCalculator.CalculateStats(issues, t, t.Add(week))))
					.ToList();

				workbook.Workbook.Worksheets.Add("Weekly").Write(weekMetrics, "Week");

				// Person stats.
				var personMetrics = weekMetrics
					.SelectMany(t => t.Row.PersonStats.Select(a => new WorksheetRow<PersonStats>(t.KeyValue, a)))
					.ToList();

				workbook.Workbook.Worksheets.Add("Weekly by dev").Write(personMetrics, "Week");

				workbook.SaveAs(new FileInfo("metrics.xlsx"));
			}
		}

		private static void BySprint(List<JiraIssue> issues)
		{
			var previousSprint = issues
				.SelectMany(t => t.Sprints)
				.Where(t => !string.IsNullOrWhiteSpace(t))
				.OrderByDescending(t => t)
				.First();

			var sprint = ConsoleUtil.ReadString("Sprint:", previousSprint);
			var byPriority = MetricCalculator.CalculateSprintMetrics(issues, sprint);

			Console.Clear();
			ConsoleUtil.WriteHeader($"## Stats for sprint {sprint}");

			ConsoleUtil.PrintObject(byPriority);
		}

		private static void ByTimePeriod(List<JiraIssue> issues)
		{
			var lastSaturday = DateTime.Today.Previous(DayOfWeek.Saturday).Date;

			var startDate = ConsoleUtil.ReadDateTime("Start date:", lastSaturday.AddDays(-6).Date);
			var endDate = ConsoleUtil.ReadDateTime("End date:", lastSaturday);

			Console.Clear();
			ConsoleUtil.WriteHeader($"## Stats from {startDate:d} to {endDate:d}");

			var stats = MetricCalculator.CalculateStats(issues, startDate, endDate);

			ConsoleUtil.PrintObject(stats);
		}

		private static List<JiraIssue> ReadJiraCsvFile(string filename)
		{
			var filetext = File.ReadAllText(filename);
			var csv = new CsvReader(new StringReader(filetext));
			csv.Configuration.RegisterClassMap<JiraIssueMap>();
			csv.Configuration.WillThrowOnMissingField = false;
			var result = csv.GetRecords<JiraIssue>().ToList();

			var epics = result.Where(t => t.Type == IssueType.Epic).ToDictionary(t => t.Key);

			foreach (var issue in result)
			{
				if (!string.IsNullOrWhiteSpace(issue.EpicKey))
				{
					JiraIssue epic;
					if (epics.TryGetValue(issue.EpicKey, out epic))
					{
						issue.Epic = epic;
					}
				}
			}

			return result;
		}
	}
}