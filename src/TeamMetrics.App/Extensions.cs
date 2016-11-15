namespace TeamMetrics.App
{
	using System;

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
	}
}