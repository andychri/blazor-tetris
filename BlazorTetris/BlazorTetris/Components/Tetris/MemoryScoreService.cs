using System.Collections.Generic;

namespace BlazorTetris.Tetris;

public sealed class MemoryScoreService : IScoreService
{
    private readonly List<ScoreEntry> _scores = new();
    private readonly object _lock = new();

    public Task<IReadOnlyList<ScoreEntry>> GetTopAsync(int take = 10)
    {
        lock (_lock)
        {
            var top = _scores
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.WhenUtc)
                .Take(take)
                .ToList()
                .AsReadOnly();
            return Task.FromResult((IReadOnlyList<ScoreEntry>)top);
        }
    }

    public Task AddAsync(ScoreEntry entry)
    {
        lock (_lock) _scores.Add(entry);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        lock (_lock) _scores.Clear();
        return Task.CompletedTask;
    }
}
