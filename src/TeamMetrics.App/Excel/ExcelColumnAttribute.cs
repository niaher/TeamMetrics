namespace TeamMetrics.App.Excel
{
	using System;

	public class ExcelColumnAttribute : Attribute
	{
		public string HeaderBackgroundColor { get; set; } = "#000";
		public string HeaderFontColor { get; set; } = "#fff";
		public int OrderIndex { get; set; }
	}
}