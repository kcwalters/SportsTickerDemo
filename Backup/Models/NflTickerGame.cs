namespace NflTickerDemo.Models;
public class NflTickerGame
{
    public string AwayTeam { get; set; } = """";
    public string HomeTeam { get; set; } = """";
    public int? AwayScore { get; set; }
    public int? HomeScore { get; set; }
    public string StatusText { get; set; } = """";
    public bool IsLive { get; set; }
    public bool IsFinal { get; set; }
    public string? EventId { get; set; }
}
