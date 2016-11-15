namespace TeamMetrics
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
			var lastSaturday = DateTime.Today.Last(DayOfWeek.Saturday).Date;

			var startDate = ConsoleUtil.ReadDateTime("Start date:", lastSaturday.AddDays(-6).Date);
			var endDate = ConsoleUtil.ReadDateTime("End date:", lastSaturday);

			Console.Clear();
			ConsoleUtil.WriteHeader($"## Stats from {startDate:d} to {endDate:d}");

			var issues = ReadJiraCsvFile("jira.csv");
			var stats = MetricCalculator.CalculateStats(issues, startDate, endDate);

			ConsoleUtil.PrintPropertyValues(stats);

			Console.ReadKey();
		}

		private static List<JiraIssue> ReadJiraCsvFile(string filename)
		{
			var filetext = File.ReadAllText(filename);
			var csv = new CsvReader(new StringReader(filetext));
			csv.Configuration.RegisterClassMap<JiraIssueMap>();
			csv.Configuration.WillThrowOnMissingField = false;
			return csv.GetRecords<JiraIssue>().ToList();
		}
	}
}