namespace TeamMetrics.App
{
	using System;
	using System.Collections.Generic;
	using OfficeOpenXml;

	public static class Extensions
	{
		public static DateTime Last(this DateTime now, DayOfWeek dayOfWeek, int weeksAgo = 1)
		{
			DayOfWeek lastDayOfWeek = dayOfWeek;
			var week = 0;
			var previousDay = now.AddDays(-1);

			while (week < weeksAgo)
			{
				week += 1;

				while (previousDay.DayOfWeek != lastDayOfWeek)
				{
					previousDay = previousDay.AddDays(-1);
				}
			}

			return previousDay;
		}

		/// <summary>
		/// Gets the 12:00:00 instance of a DateTime
		/// </summary>
		public static DateTime AbsoluteStart(this DateTime dateTime)
		{
			return dateTime.Date;
		}

		/// <summary>
		/// Gets the 11:59:59 instance of a DateTime
		/// </summary>
		public static DateTime AbsoluteEnd(this DateTime dateTime)
		{
			return AbsoluteStart(dateTime).AddDays(1).AddTicks(-1);
		}

		public static IEnumerable<DateTime> Enumerate(this DateTime start, TimeSpan period, DateTime until)
		{
			if (start >= until)
			{
				throw new ArgumentOutOfRangeException(nameof(until));
			}

			var at = start;
			while (at <= until)
			{
				yield return at;
				at = at.Add(period);
			}
		}

		public static IEnumerable<T> ForEachItem<T>(this IEnumerable<T> list, Action<T> action)
		{
			foreach (var item in list)
			{
				action(item);
				yield return item;
			}
		}
		
		public static void Write<T>(this ExcelWorksheet worksheet, int row, StoryPointStats<T> metric, string sprintName)
		{
			worksheet.Cells[row, 1].Value = sprintName;
			worksheet.Cells[row, 2].Value = metric.Segment;
			worksheet.Cells[row, 3].Value = metric.Open;
			worksheet.Cells[row, 4].Value = metric.InProgress;
			worksheet.Cells[row, 5].Value = metric.InCodeReview;
			worksheet.Cells[row, 6].Value = metric.ReadyForDeploy;
			worksheet.Cells[row, 7].Value = metric.Done;
		}
	}
}