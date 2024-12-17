namespace GameOfLife.API.DTO;

public class BoardStateDto
{
    public bool[][] State { get; set; } = Array.Empty<bool[]>();
}
