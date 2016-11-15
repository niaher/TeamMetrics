namespace TeamMetrics.App
{
	public class PersonStats
	{
		public string Name { get; set; }
		public decimal StoryPointsDone { get; set; }
		public decimal StoryPointsReviewed { get; set; }
		public decimal StoryPointsInProgress { get; set; }
		public int BugsFixed { get; set; }
		public int BugsReported { get; set; }
		
		/// <summary>
		/// Gets overall performance score.
		/// </summary>
		public decimal Score =>
			this.StoryPointsDone +
			this.BugsFixed * 0.2m +
			this.StoryPointsReviewed * 0.1m +
			this.BugsReported * 0.1m +
			this.StoryPointsInProgress * 0.5m;
	}
}