using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorTetris.Tetris;

public interface IScoreService
{
    Task<IReadOnlyList<ScoreEntry>> GetTopAsync(int take = 10);
    Task AddAsync(ScoreEntry entry);
    Task ClearAsync();
}
