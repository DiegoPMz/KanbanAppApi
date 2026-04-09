using Bogus;
using ErrorOr;
using KanbanAppApi.Common.Domain.BoardAggregate;

namespace KanbanAppApi.UnitTests.Common.Domain.BoardAggregate;

public class BoardTests
{
    private readonly Faker _faker = new();
    
    [Fact]
    public void Create_ShouldReturnInvalidNameError_WhenNameIsWhiteSpace()
    {
        // Arrange
        const string name = "  ";
        var userId = Guid.NewGuid();
        
        // Act
        var result = Board.Create(name, userId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.InvalidName(name).Code, result.FirstError.Code);
    }
    
    [Fact]
    public void Create_ShouldReturnInvalidNameError_WhenNameExceeds250Characters()
    {
        // Arrange
        var longName = _faker.Random.String2(251);
        var userId = Guid.NewGuid();
        
        // Act
        var result = Board.Create(longName, userId);
        
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.InvalidName(longName).Code, result.FirstError.Code);
    }

    [Fact]
    public void AddColumn_ShouldReturnConflict_WhenNameAlreadyExists()
    {
        // Arrange
        var board = new Faker<Board>()
            .CustomInstantiator(f => Board.Create("Sprint Board", Guid.NewGuid()).Value)
            .Generate();

        const string columnName = "ToDo";
        board.AddColumn(columnName, "#fff");

        // Act
        var result = board.AddColumn(columnName, null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.NameAlreadyExists(columnName).Code, result.FirstError.Code);
        Assert.Single(board.Columns);
    }

    [Fact]
    public void AddColumn_ShouldReturnLimitReached_WhenAddingMoreThanTenColumns()
    {
        // Arrange
        const int limit = 10;
        var board = new Faker<Board>()
            .CustomInstantiator(f => Board.Create("Sprint Board", Guid.NewGuid()).Value)
            .FinishWith((f, b) => 
            {
                for (int i = 0; i < limit; i++)
                {
                    b.AddColumn($"{f.Commerce.Department()} {i}", null);
                }
            })
            .Generate();
        
        // Act
        var result = board.AddColumn("Extra Column", null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.LimitReached(limit).Code, result.FirstError.Code);
        Assert.Equal(limit, board.Columns.Count);
    }
    
    [Fact]
    public void EditColumn_ShouldUpdateColumnDetails_WhenColumnExists()
    {
        // Arrange
        var board = Board.Create("Kanban", Guid.NewGuid()).Value;
        board.AddColumn("Old Name", "#000"); 
        var column = board.Columns.First();
    
        const string newName = "New Name";
        const string newColor = "#fff";

        // Act
        var result = board.EditColumn(column.Id, newName, newColor);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(newName, result.Value.Name);
        Assert.Equal(newColor, result.Value.Color);
    }
    
    [Fact]
    public void EditColumn_ShouldReturnNotFound_WhenColumnDoesNotExist()
    {
        // Arrange
        var board = Board.Create("Kanban", Guid.NewGuid()).Value;
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = board.EditColumn(nonExistentId, "Name", "#fff");

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.ColumnNotFound(nonExistentId.ToString()).Code, result.FirstError.Code);
    }
    
    [Fact]
    public void EditColumn_ShouldReturnConflict_WhenNewNameAlreadyExistsInAnotherColumn()
    {
        // Arrange
        var board = Board.Create("Scrum Board", Guid.NewGuid()).Value;
        board.AddColumn("To Do", "#000");
        board.AddColumn("In Progress", "#fff");
    
        var columnToEdit = board.Columns.First(c => c.Name == "In Progress");

        // Act
        var result = board.EditColumn(id : columnToEdit.Id, name: "To Do", color: "#ccc");

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.NameAlreadyExists("To Do").Code, result.FirstError.Code);
    }
    
    [Fact]
    public void RemoveColumn_ShouldRemoveFromCollection_WhenColumnExists()
    {
        // Arrange
        var board = Board.Create("Kanban", Guid.NewGuid()).Value;
        board.AddColumn("To Delete", "#000");
        var columnId = board.Columns.First().Id;

        // Act
        var result = board.RemoveColumn(columnId);

        // Assert
        Assert.False(result.IsError);
        Assert.Empty(board.Columns);
        Assert.IsType<Deleted>(result.Value);
    }
    
    [Fact]
    public void RemoveColumn_ShouldReturnNotFound_WhenColumnDoesNotExist()
    {
        // Arrange
        var board = Board.Create("Kanban", Guid.NewGuid()).Value;
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = board.RemoveColumn(nonExistentId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.ColumnNotFound(nonExistentId.ToString()).Code, result.FirstError.Code);
    }

   [Fact]
    public void ReorderColumns_ShouldReturnInvalidColumnCount_WhenCountDoesNotMatch()
    {
        // Arrange
        var board = CreateBoardWithColumns(3);
        var newOrders = board.Columns.Take(2)
            .Select((c, i) => new ColumnOrderInput(c.Id, i)).ToList();

        // Act
        var result = board.ReorderColumns(newOrders);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.InvalidColumnCount.Code, result.FirstError.Code);
    }

    [Fact]
    public void ReorderColumns_ShouldReturnColumnNotFound_WhenInputContainsForeignId()
    {
        // Arrange
        var board = CreateBoardWithColumns(2);
        var foreignId = Guid.NewGuid();
        var newOrders = new List<ColumnOrderInput>
        {
            new(board.Columns.First().Id, 0),
            new(foreignId, 1)
        };

        // Act
        var result = board.ReorderColumns(newOrders);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.ColumnNotFoundInBoard.Code, result.FirstError.Code);
    }

    [Fact]
    public void ReorderColumns_ShouldReturnDuplicateOrder_WhenOrdersAreRepeated()
    {
        // Arrange
        var board = CreateBoardWithColumns(2);
        var newOrders = new List<ColumnOrderInput>
        {
            new(board.Columns.ElementAt(0).Id, 0),
            new(board.Columns.ElementAt(1).Id, 0)
        };

        // Act
        var result = board.ReorderColumns(newOrders);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(BoardErrors.DuplicateColumnOrder.Code, result.FirstError.Code);
    }

    [Fact]
    public void ReorderColumns_ShouldUpdateOrders_WhenInputIsValid()
    {
        var board = Board.Create("Sprint Board", Guid.NewGuid()).Value;
        board.AddColumn("C1", "#000"); // Order 0
        board.AddColumn("C2", "#fff"); // Order 1

        var c1 = board.Columns.First(c => c.Name == "C1");
        var c2 = board.Columns.First(c => c.Name == "C2");

        var newOrders = new List<ColumnOrderInput>
        {
            new(c1.Id, 1), 
            new(c2.Id, 0)
        };

        // 2. Act
        var result = board.ReorderColumns(newOrders);

        // 3. Assert
        Assert.False(result.IsError);
        Assert.IsType<Updated>(result.Value);
    
        Assert.Equal(1, c1.Order);
        Assert.Equal(0, c2.Order);
    }

    private Board CreateBoardWithColumns(int count)
    {
        var board = Board.Create("Test Board", Guid.NewGuid()).Value;
        for (int i = 0; i < count; i++)
        {
            board.AddColumn($"Col {i}", "#fff");
        }
        return board;
    }
}