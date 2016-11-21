namespace TeamMetrics.App
{
	public class StoryPointStats<TKey>
	{
		public TKey Segment { get; set; }
		public decimal Open { get; set; }
		public decimal InProgress { get; set; }
		public decimal InCodeReview { get; set; }
		public decimal ReadyForDeploy { get; set; }
		public decimal Done { get; set; }
	}
}