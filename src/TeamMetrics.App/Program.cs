namespace TeamMetrics.App
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using CsvHelper;

	public class Program
	{
		public static void Main(string[] args)
		{
			var issues = ReadJiraCsvFile("jira.csv");

			//ByTimePeriod(issues);

			BySprint(issues);

			Console.ReadKey();
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
			var lastSaturday = DateTime.Today.Last(DayOfWeek.Saturday).Date;

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