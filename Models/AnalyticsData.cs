namespace ReflektJournal.Models;

public class AnalyticsData
{
    public MoodDistribution MoodDistribution { get; set; } = new();
    public List<MoodFrequency> FrequentMoods { get; set; } = new();
    public List<TagUsage> MostUsedTags { get; set; } = new();
    public List<TagBreakdown> TagBreakdown { get; set; } = new();
    public List<WordCountTrend> WordCountTrends { get; set; } = new();
    public int TotalEntries { get; set; }
    public int TotalWords { get; set; }
    public double AverageWordsPerEntry { get; set; }
}

public class MoodDistribution
{
    public Dictionary<string, int> MoodCounts { get; set; } = new();
    public Dictionary<string, double> MoodPercentages { get; set; } = new();
}

public class MoodFrequency
{
    public string MoodName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class TagUsage
{
    public string TagName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class TagBreakdown
{
    public string TagName { get; set; } = string.Empty;
    public int EntryCount { get; set; }
    public List<string> AssociatedMoods { get; set; } = new();
}

public class WordCountTrend
{
    public DateTime Date { get; set; }
    public int WordCount { get; set; }
    public int EntryCount { get; set; }
}