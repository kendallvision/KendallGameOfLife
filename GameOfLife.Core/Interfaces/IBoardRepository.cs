using GameOfLife.Core.Models;

namespace GameOfLife.Core.Interfaces;

public interface IBoardRepository
{
    Task<Board?> GetBoard(Guid id);
		
    Task SaveBoard(Board board);
}