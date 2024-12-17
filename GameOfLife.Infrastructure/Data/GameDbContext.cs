using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using GameOfLife.Core.Models;

namespace GameOfLife.Infrastructure.Data;

public class GameDbContext : DbContext
{
    public DbSet<Board> Boards { get; set; }

    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt);
            // Convert the bool[,] State to a string for storage with custom comparison
            entity.Property(e => e.State)
                .HasConversion(
                    v => SerializeState(v),
                    v => DeserializeState(v))
                .Metadata.SetValueComparer(new ValueComparer<bool[,]>(
                    (a1, a2) => CompareArrays(a1, a2),
                    arr => GetArrayHashCode(arr),
                    arr => CopyArray(arr)));
        });
    }

    private static bool CompareArrays(bool[,] a1, bool[,] a2)
    {
        if (a1 == null || a2 == null) {
            return a1 == a2;
				}

        if (a1.GetLength(0) != a2.GetLength(0) || a1.GetLength(1) != a2.GetLength(1)) {
            return false;
				}

        for (int i = 0; i < a1.GetLength(0); i++)
        {
            for (int j = 0; j < a1.GetLength(1); j++)
            {
                if (a1[i, j] != a2[i, j]) {
                    return false;
								}
            }
        }
        return true;
    }

    private static int GetArrayHashCode(bool[,] arr)
    {
        if (arr == null) {
            return 0;
				}

        int hash = 17;
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                hash = hash * 31 + arr[i, j].GetHashCode();
            }
        }
        return hash;
    }

    private static bool[,] CopyArray(bool[,] arr)
    {
        if (arr == null) {
            return null;
				}

        var result = new bool[arr.GetLength(0), arr.GetLength(1)];
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                result[i, j] = arr[i, j];
            }
        }
				
        return result;
    }

    private static string SerializeState(bool[,] state)
    {
        var width = state.GetLength(0);
        var height = state.GetLength(1);
        var result = $"{width},{height},";
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                result += state[x, y] ? "1" : "0";
            }
        }
        return result;
    }

    private static bool[,] DeserializeState(string serialized)
    {
        var parts = serialized.Split(',');
        var width = int.Parse(parts[0]);
        var height = int.Parse(parts[1]);
        var data = parts[2];
        
        var state = new bool[width, height];
        var index = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                state[x, y] = data[index++] == '1';
            }
        }
        return state;
    }
}
