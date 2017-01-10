namespace TeamMetrics.App
{
	internal class Segment
	{
		public Segment(string key, object row)
		{
			this.Key = key;
			this.Row = row;
		}

		public string Key { get; set; }

		public object Row { get; set; }
	}
}