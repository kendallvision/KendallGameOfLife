using GameOfLife.Core.Interfaces;
using GameOfLife.Core.Models;
using GameOfLife.Infrastructure.Data;

namespace GameOfLife.Infrastructure.Repositories;

public class SqliteBoardRepository : IBoardRepository
{
    private readonly GameDbContext _context;

    public SqliteBoardRepository(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Board?> GetBoard(Guid id)
    {
        return await _context.Boards.FindAsync(id);
    }

    public async Task SaveBoard(Board board)
    {
        var existing = await _context.Boards.FindAsync(board.Id);
        if (existing == null)
        {
            await _context.Boards.AddAsync(board);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(board);
        }
				
        await _context.SaveChangesAsync();
    }
}

