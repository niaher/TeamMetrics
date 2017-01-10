namespace TeamMetrics.App.Excel
{
	public class WorksheetRow<T>
	{
		public WorksheetRow(string key, T row)
		{
			this.Key = key;
			this.Row = row;
		}

		public string Key { get; set; }

		public T Row { get; set; }
	}
}