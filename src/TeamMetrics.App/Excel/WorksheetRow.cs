namespace TeamMetrics.App.Excel
{
	public class WorksheetRow<T>
	{
		public WorksheetRow(string keyValue, T row)
		{
			this.KeyValue = keyValue;
			this.Row = row;
		}

		public string KeyValue { get; set; }

		public T Row { get; set; }
	}
}