namespace TeamMetrics.App
{
	using System.Collections.Generic;

	public class SprintMetrics
	{
		public string Sprint { get; set; }
		public List<StoryPointStats<Priority>> StoryPointsByPriority { get; set; }
		public List<StoryPointStats<string>> StoryPointsByEpic { get; set; }
	}
}