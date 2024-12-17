using System.ComponentModel.DataAnnotations;

namespace GameOfLife.Core.Models;

public class Board
{
    [Key]
    public Guid Id { get; set; }
    public bool[,] State { get; set; }
    public int Width => State.GetLength(0);
    public int Height => State.GetLength(1);
    public DateTime CreatedAt { get; set; }

    public Board(bool[,] initialState)
    {
        Id = Guid.NewGuid();
        State = initialState;
        CreatedAt = DateTime.UtcNow;
    }

    // Parameterless constructor for EF Core
    protected Board()
    {
        State = new bool[0, 0];
    }

    public bool NeedsExpansion()
    {
        // Check edges for live cells
        for (int x = 0; x < Width; x++)
        {
            if (State[x, 0] || State[x, Height - 1]) {
							return true;
						}
        }

        for (int y = 0; y < Height; y++)
        {
            if (State[0, y] || State[Width - 1, y]) {
							return true;
						}
        }

        return false;
    }

    public void ExpandGrid()
    {
        var newWidth = Width + 2;
        var newHeight = Height + 2;
        var newState = new bool[newWidth, newHeight];

        // Copy existing state to center of new grid
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                newState[x + 1, y + 1] = State[x, y];
            }
        }

        State = newState;
    }
}
