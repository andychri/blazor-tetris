namespace BlazorTetris.Tetris;

public sealed class ScoreEntry
{
    public int Id { get; set; }                 // optional, unused in-memory
    public string Name { get; set; } = "Player";
    public int Points { get; set; }
    public DateTimeOffset WhenUtc { get; set; } = DateTimeOffset.UtcNow;
}