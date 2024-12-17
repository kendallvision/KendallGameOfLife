using GameOfLife.Core.Models;
using GameOfLife.Core.Services;
using GameOfLife.Core.Interfaces;
using Moq;

namespace GameOfLife.Tests;

public class InputValidationTests
{
   private readonly Mock<IBoardRepository> _mockRepo;
   private readonly GameService _service;
   private Board _currentBoard;

   public InputValidationTests()
   {
       _mockRepo = new Mock<IBoardRepository>();
       _mockRepo.Setup(r => r.GetBoard(It.IsAny<Guid>()))
           .ReturnsAsync(() => _currentBoard);
       _mockRepo.Setup(r => r.SaveBoard(It.IsAny<Board>()))
           .Callback<Board>(board => _currentBoard = board);
           
       _service = new GameService(_mockRepo.Object);
   }

   [Fact]
   public async Task NonExistentBoard_ThrowsKeyNotFoundException()
   {
       _currentBoard = null;
       await Assert.ThrowsAsync<KeyNotFoundException>(
           async () => await _service.GetNextState(Guid.NewGuid())
       );
   }

   [Fact]
   public async Task SingleCellBoard_CalculatesCorrectly()
   {
       // A single living cell should die
       var singleCell = new bool[1, 1] { { true } };
       _currentBoard = new Board(singleCell);

       var nextGen = await _service.GetNextState(Guid.NewGuid());
       
       Assert.False(nextGen[1,1]); // Center cell should die
   }

   [Fact]
   public async Task GetFinalState_MaxGenerationsExceeded_ThrowsException()
   {
       // Use a pattern that doesn't stabilize quickly
       var oscillator = new bool[3, 3]
       {
           { false, true, false },
           { false, true, false },
           { false, true, false }
       };

       _currentBoard = new Board(oscillator);

       // Try with very low max generations to force exception
       await Assert.ThrowsAsync<Exception>(
           async () => await _service.GetFinalState(Guid.NewGuid(), 1)
       );
   }

   [Fact]
   public async Task TinyBoard_ExpandsCorrectly()
   {
       var tiny = new bool[2, 2]
       {
           { true, true },
           { true, false }
       };

       _currentBoard = new Board(tiny);
       var nextGen = await _service.GetNextState(Guid.NewGuid());

       // Check that board expanded and still has correct next generation
       Assert.True(nextGen.GetLength(0) > 2);
       Assert.True(nextGen.GetLength(1) > 2);
   }
}
