using GameOfLife.Core.Models;
using GameOfLife.Core.Services;
using GameOfLife.Core.Interfaces;
using Moq;

namespace GameOfLife.Tests;

public class GameServiceTests
{
    private readonly Mock<IBoardRepository> _mockRepo;
    private readonly GameService _service;
    private Board _currentBoard;

    public GameServiceTests()
    {
        _mockRepo = new Mock<IBoardRepository>();
        _mockRepo.Setup(r => r.GetBoard(It.IsAny<Guid>()))
            .ReturnsAsync(() => _currentBoard);
        _mockRepo.Setup(r => r.SaveBoard(It.IsAny<Board>()))
            .Callback<Board>(board => _currentBoard = board);
            
        _service = new GameService(_mockRepo.Object);
    }

    [Fact]
    public async Task Blinker_OscillatesForMultipleGenerations()
    {
        // Arrange
        var initialBlinker = new bool[3, 3]
        {
            { false, true, false },
            { false, true, false },
            { false, true, false }
        };

        var expectedHorizontal = new bool[5, 5]
        {
            { false, false, false, false, false },
            { false, false, false, false, false },
            { false, true, true, true, false },
            { false, false, false, false, false },
            { false, false, false, false, false }
        };

        var expectedVertical = new bool[5, 5]
        {
            { false, false, false, false, false },
            { false, false, true, false, false },
            { false, false, true, false, false },
            { false, false, true, false, false },
            { false, false, false, false, false }
        };

        _currentBoard = new Board(initialBlinker);

        // Act & Assert
        var gen1 = await _service.GetNextState(Guid.NewGuid());
        Assert.Equal(expectedHorizontal, gen1);

        var gen2 = await _service.GetNextState(Guid.NewGuid());
        Assert.Equal(expectedVertical, gen2);

        var gen3 = await _service.GetNextState(Guid.NewGuid());
        Assert.Equal(expectedHorizontal, gen3);

        var gen4 = await _service.GetNextState(Guid.NewGuid());
        Assert.Equal(expectedVertical, gen4);
    }

    [Fact]
    public async Task Block_RemainsStableForMultipleGenerations()
    {
        // Arrange
        var block = new bool[4, 4]
        {
            { false, false, false, false },
            { false, true, true, false },
            { false, true, true, false },
            { false, false, false, false }
        };

        _currentBoard = new Board(block);

        // Act & Assert
        // Check that it remains stable for several generations
        for (int i = 0; i < 5; i++)
        {
            var nextGen = await _service.GetNextState(Guid.NewGuid());
            Assert.Equal(block, nextGen);
        }
    }

    [Fact]
    public async Task Glider_MovesCorrectlyForMultipleGenerations()
    {
        // Arrange
        var initialGlider = new bool[6, 6]
        {
            { false, false, false, false, false, false },
            { false, false, true, false, false, false },
            { false, false, false, true, false, false },
            { false, true, true, true, false, false },
            { false, false, false, false, false, false },
            { false, false, false, false, false, false }
        };

        var expectedGen1 = new bool[6, 6]
        {
            { false, false, false, false, false, false },
            { false, false, false, false, false, false },
            { false, true, false, true, false, false },
            { false, false, true, true, false, false },
            { false, false, true, false, false, false },
            { false, false, false, false, false, false }
        };

        var expectedGen2 = new bool[6, 6]
        {
            { false, false, false, false, false, false },
            { false, false, false, false, false, false },
            { false, false, false, true, false, false },
            { false, true, false, true, false, false },
            { false, false, true, true, false, false },
            { false, false, false, false, false, false }
        };

        var expectedGen3 = new bool[6, 6]
        {
            { false, false, false, false, false, false },
            { false, false, false, false, false, false },
            { false, false, true, false, false, false },
            { false, false, false, true, true, false },
            { false, false, true, true, false, false },
            { false, false, false, false, false, false }
        };

        _currentBoard = new Board(initialGlider);

        // Act & Assert
        var gen1 = await _service.GetNextState(Guid.NewGuid());
        Assert.Equal(expectedGen1, gen1);

        var gen2 = await _service.GetNextState(Guid.NewGuid());
        Assert.Equal(expectedGen2, gen2);

        var gen3 = await _service.GetNextState(Guid.NewGuid());
        Assert.Equal(expectedGen3, gen3);
    }

    [Fact]
    public async Task EmptyBoard_RemainsEmptyForMultipleGenerations()
    {
        // Arrange
        var emptyBoard = new bool[3, 3]
        {
            { false, false, false },
            { false, false, false },
            { false, false, false }
        };

        _currentBoard = new Board(emptyBoard);

        // Act & Assert
        // Check that it remains empty for several generations
        for (int i = 0; i < 5; i++)
        {
            var nextGen = await _service.GetNextState(Guid.NewGuid());
            Assert.Equal(emptyBoard, nextGen);
        }
    }
}
