using GameOfLife.API.DTO;
using GameOfLife.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

		[HttpPost]
		public async Task<ActionResult<Guid>> CreateBoard([FromBody] BoardStateDto dto)
		{
				// Check for null or empty input
				if (dto?.State == null || dto.State.Length == 0)
				{
						return BadRequest("Board state cannot be empty");
				}

				// Check for jagged array consistency
				if (dto.State.Any(row => row == null || row.Length != dto.State[0].Length))
				{
						return BadRequest("All rows must have the same length");
				}

				var state = ConvertJaggedTo2D(dto.State);
				var boardId = await _gameService.CreateBoard(state);
				
				return Ok(boardId);
		}

    [HttpGet("{id}/next")]
    public async Task<ActionResult<bool[][]>> GetNextState(Guid id)
    {
        try
        {
            var nextState = await _gameService.GetNextState(id);
            return Ok(Convert2DToJagged(nextState));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/steps/{count}")]
    public async Task<ActionResult<bool[][]>> GetStateAfterSteps(Guid id, int count)
    {
        try
        {
            var state = await _gameService.GetStateAfterSteps(id, count);
            return Ok(Convert2DToJagged(state));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

		[HttpGet("{id}/final/{maxGenerations}")]
		public async Task<ActionResult<bool[][]>> GetFinalState(Guid id, int maxGenerations = 100)
		{
				try
				{
						var finalState = await _gameService.GetFinalState(id, maxGenerations);
						return Ok(Convert2DToJagged(finalState));
				}
				catch (KeyNotFoundException)
				{
						return NotFound();
				}
				catch (Exception ex)
				{
						return BadRequest(ex.Message);
				}
		}

    private static bool[,] ConvertJaggedTo2D(bool[][] jaggedArray)
    {
        var rows = jaggedArray.Length;
        var cols = jaggedArray[0].Length;
        var result = new bool[rows, cols];
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = jaggedArray[i][j];
            }
        }
        
        return result;
    }

    private static bool[][] Convert2DToJagged(bool[,] array2D)
    {
        var rows = array2D.GetLength(0);
        var cols = array2D.GetLength(1);
        var result = new bool[rows][];
        
        for (int i = 0; i < rows; i++)
        {
            result[i] = new bool[cols];
            for (int j = 0; j < cols; j++)
            {
                result[i][j] = array2D[i, j];
            }
        }
        
        return result;
    }
}
