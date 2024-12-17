using System.Text;
using GameOfLife.Core.Interfaces;
using GameOfLife.Core.Models;

namespace GameOfLife.Core.Services;

public class GameService
{
    private readonly IBoardRepository _repository;

    public GameService(IBoardRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateBoard(bool[,] initialState)
    {
        var board = new Board(initialState);
        await _repository.SaveBoard(board);

        return board.Id;
    }

    public async Task<bool[,]> GetNextState(Guid boardId)
    {
        var board = await _repository.GetBoard(boardId);
        if (board == null) {
					throw new KeyNotFoundException("Board not found");
				}

        var nextState = CalculateNextState(board);
        board.State = nextState;
        await _repository.SaveBoard(board);
        
        return nextState;
    }

		public async Task<bool[,]> GetStateAfterSteps(Guid boardId, int steps)
		{
				if (steps < 0)
				{
						throw new ArgumentException("Number of steps cannot be negative", nameof(steps));
				}

				var board = await _repository.GetBoard(boardId);
				if (board == null) throw new KeyNotFoundException("Board not found");

				var currentState = board.State;
				for (int i = 0; i < steps; i++)
				{
						currentState = CalculateNextState(new Board(currentState));
				}
				
				return currentState;
		}

		public async Task<bool[,]> GetFinalState(Guid boardId, int maxGenerations)
		{
				var board = await _repository.GetBoard(boardId);
				if (board == null) {
					throw new KeyNotFoundException("Board not found");
				}

				var currentState = board.State;
				var iterations = 0;
				var stateHistory = new Dictionary<string, int>(); // Track state and the generation it appeared

				while (iterations < maxGenerations)
				{
						var stateHash = GetStateHash(currentState);
						
						if (stateHistory.TryGetValue(stateHash, out int previousGen))
						{
								// Calculate period
								int period = iterations - previousGen;
								
								// If period is 1, we've found a stable state
								if (period == 1)
								{
										return currentState;
								}
								
								// Otherwise it's an oscillator
								throw new Exception($"Pattern oscillates with period {period} - no final stable state exists");
						}

						stateHistory.Add(stateHash, iterations);
						currentState = CalculateNextState(new Board(currentState));
						iterations++;
				}

				throw new Exception($"No stable state or oscillation detected after {maxGenerations} generations");
		}

    private bool[,] CalculateNextState(Board board)
    {
        if (board.NeedsExpansion())
        {
            board.ExpandGrid();
        }

        var nextState = new bool[board.Width, board.Height];

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                var neighbors = CountNeighbors(board.State, x, y);
                nextState[x, y] = ShouldCellLive(board.State[x, y], neighbors);
            }
        }

        return nextState;
    }

    private int CountNeighbors(bool[,] state, int x, int y)
    {
        int count = 0;
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) {
									continue;
								}
                
                int newX = x + i;
                int newY = y + j;

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    if (state[newX, newY]) {
											count++;
										}
                }
            }
        }

        return count;
    }

    private bool ShouldCellLive(bool currentlyAlive, int neighbors)
    {
        if (currentlyAlive)
        {
            return neighbors == 2 || neighbors == 3;
        }

        return neighbors == 3;
    }

    private string GetStateHash(bool[,] state)
    {
        var sb = new StringBuilder();
        for (int x = 0; x < state.GetLength(0); x++)
        {
            for (int y = 0; y < state.GetLength(1); y++)
            {
                sb.Append(state[x, y] ? '1' : '0');
            }
        }

        return sb.ToString();
    }
}
