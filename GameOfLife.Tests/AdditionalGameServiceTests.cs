using GameOfLife.Core.Models;
using GameOfLife.Core.Services;
using GameOfLife.Core.Interfaces;
using Moq;

namespace GameOfLife.Tests;

public class AdditionalGameServiceTests
{
    private readonly Mock<IBoardRepository> _mockRepo;
    private readonly GameService _service;
    private Board _currentBoard;

    public AdditionalGameServiceTests()
    {
        _mockRepo = new Mock<IBoardRepository>();
        _mockRepo.Setup(r => r.GetBoard(It.IsAny<Guid>()))
            .ReturnsAsync(() => _currentBoard);
        _mockRepo.Setup(r => r.SaveBoard(It.IsAny<Board>()))
            .Callback<Board>(board => _currentBoard = board);
            
        _service = new GameService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetFinalState_StablePattern_ReturnsCorrectState()
    {
        // Test that a pattern that becomes stable is identified correctly
        var almostBlock = new bool[4, 4]
        {
            { false, false, false, false },
            { false, true, true, false },
            { false, true, false, false },
            { false, false, false, false }
        };
        _currentBoard = new Board(almostBlock);
    
        var finalState = await _service.GetFinalState(Guid.NewGuid(), 10);
        // Should stabilize into a block
        Assert.True(finalState[1,1] && finalState[1,2] && finalState[2,1] && finalState[2,2]);
    }

    [Fact]
    public async Task GetStateAfterSteps_NegativeSteps_ThrowsException()
    {
        _currentBoard = new Board(new bool[2, 2]);
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.GetStateAfterSteps(Guid.NewGuid(), -1)
        );
    }
}
