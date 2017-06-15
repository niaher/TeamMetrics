namespace TeamMetrics.App
{
	using CsvHelper.Configuration;
	using CsvHelper.TypeConversion;

	public sealed class JiraIssueMap : CsvClassMap<JiraIssue>
	{
		public JiraIssueMap()
		{
			this.Map(m => m.Key).Name("Issue key");
			this.Map(m => m.Type).Name("Issue Type").TypeConverter(new EnumConverter(typeof(IssueType)));
			this.Map(m => m.Status).Name("Status");
			this.Map(m => m.Resolution).Name("Resolution");
			this.Map(m => m.CreatedOn).Name("Created");
			this.Map(m => m.InProgressOn).Name("Custom field (In Progress On)");
			this.Map(m => m.CodeReviewOn).Name("Custom field (Code review/Testing On)");
			this.Map(m => m.ReadyForDeployOn).Name("Custom field (Ready For Deploy On)");
			this.Map(m => m.ResolvedOn).Name("Resolved");
			this.Map(m => m.StoryPoints).Name("Custom field (Story Points)").Default(0);
			this.Map(m => m.Assignee).Name("Assignee");
			this.Map(m => m.Reporter).Name("Reporter");
			this.Map(m => m.Priority).Name("Priority");
			this.Map(m => m.EpicKey).Name("Custom field (Epic Link)");
			this.Map(m => m.EpicName).Name("Custom field (Epic Name)");
			this.Map(m => m.Sprint1).Name("Sprint").NameIndex(0);
			this.Map(m => m.Sprint2).Name("Sprint").NameIndex(1);
			this.Map(m => m.Sprint3).Name("Sprint").NameIndex(2);
			this.Map(m => m.Sprint4).Name("Sprint").NameIndex(3);
			this.Map(m => m.Sprint5).Name("Sprint").NameIndex(4);
			this.Map(m => m.Sprint6).Name("Sprint").NameIndex(5);

			this.Map(m => m.FirstReviewer).Name("Custom field (Reviewed By)").NameIndex(0);
			this.Map(m => m.SecondReviewer).Name("Custom field (Reviewed By)").NameIndex(1);
		}
	}
}