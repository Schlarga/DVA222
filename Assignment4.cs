using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// Code by Maverick I. N.
// <> ||
public abstract class GridGameBase<TGrid>
{
    protected const int defSize = 3;
    static protected readonly Random rand = new Random();
    protected TGrid[,] puzzleGrid;
    protected TGrid userTile;
    
    protected int width, height;
    protected int x, y;
    private int count = 0;
    protected GridGameBase(int size = defSize)
    {
        if (size <= 2) size = defSize; // Unsolvable for everything under 2x2
        this.width = size;
        this.height = size;
        puzzleGrid = new TGrid[width, height];
    }
    protected void StartGame()
    {
        ConsoleKeyInfo key;
        CreateGrid();
        do
        {
            ShuffleGrid();
            SetIntoCorner();
        } while (CheckWin());

        PrintGrid();
        do
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape) { break; }
            if (SwitchPos(key.Key))
            {
                Console.Clear();
                PrintGrid();
                count++;
            }

        } while (!CheckWin());
        if (key.Key != ConsoleKey.Escape) { Console.WriteLine($"You won! \nThe amount of moves you made were: {count}"); }
        

    }
    protected abstract bool CheckWin();
    protected abstract void PrintGridTile(int row, int col);
    protected abstract TGrid CreateTile(int index);
    
    private void CreateGrid()
    {
        int arrayIndex = 0;
        TGrid[] array = new TGrid[width * height];

        for (int i = 0; i < width * height - 1; i++)
        {
            array[i] = CreateTile(i);
        }
        array[width * height - 1] = userTile;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                puzzleGrid[i, j] = array[arrayIndex];   // Converts the 1d grid into a 2d grid
                if (array[arrayIndex].Equals(userTile))  // looks for the userTile and sets the x and y variables to its coordinate
                {
                    x = j;
                    y = i;
                }
                arrayIndex++;

            }

        }
    }
    private void PrintGrid()
    {
        for (int i = 0; i < width; i++)
        {
            Console.WriteLine();
            for (int j = 0; j < height; j++)
            {
                Console.Write("  ");
                PrintGridTile(i, j);
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
    private void ShuffleGrid()  // The shuffleGrid is made in a way so that the resulting grid always is solvable since it makes random moves from solved grid
    {
        int[] directionChoices = { 0, 1, 2, 3 };    // 4 elements for the 4 cardinal directions
        int moveDirection;
        for(int i = 0; i < width*height*20; i++) 
        {
            moveDirection = directionChoices[rand.Next(4)]; // Chooses a random direction
            switch (moveDirection)
            {
                case 0:     // Left
                    if (x > 0)
                    {
                        SwitchPos(ConsoleKey.LeftArrow);
                        break;
                    }
                    else { goto case 1; }
                    
                case 1:     // Right
                    if (x < width - 1)
                    {
                        SwitchPos(ConsoleKey.RightArrow);
                        break;
                    }
                    else { goto case 0; }

                case 2:     // Up
                    if (y > 0)
                    {
                        SwitchPos(ConsoleKey.UpArrow);
                        break;
                    
                    }
                    else { goto case 3; }

                case 3:     // Down
                    if (y < height - 1)
                    {
                        SwitchPos(ConsoleKey.DownArrow);
                        break;
                    
                    }
                    else { goto case 2; }
            }
           
        }

    }
    private void SetIntoCorner()
    {
        for(int i = x; i < width - 1; i++)
        {
            SwitchPos(ConsoleKey.RightArrow);
        }
        for (int i = y; i < height - 1; i++)
        {
            SwitchPos(ConsoleKey.DownArrow);
        }
    }
    protected bool SwitchPos(ConsoleKey key)
    {   
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                if (x > 0)
                {
                    (puzzleGrid[y, x], puzzleGrid[y, x - 1]) = (puzzleGrid[y, x - 1], puzzleGrid[y, x]);
                    x--;
                    return true;
                }
                break;
            case ConsoleKey.RightArrow:
                if (x < width - 1)
                {
                    (puzzleGrid[y, x], puzzleGrid[y, x + 1]) = (puzzleGrid[y, x + 1], puzzleGrid[y, x]);
                    x++;
                    return true;
                }
                break;
                    
            case ConsoleKey.UpArrow:
                if (y > 0)
                {
                    (puzzleGrid[y, x], puzzleGrid[y - 1, x]) = (puzzleGrid[y - 1, x], puzzleGrid[y, x]);
                    y--;
                    return true;
                }
                break;
                    
            case ConsoleKey.DownArrow:
                if (y < height - 1)
                {
                    (puzzleGrid[y, x], puzzleGrid[y + 1, x]) = (puzzleGrid[y + 1, x], puzzleGrid[y, x]);
                    y++;
                    return true;
                }
                break;
        }
        return false;
    }
    public void PrintCoordinates()   // For debugging
    {
        Console.WriteLine($"the X cordinate is {x}");
        Console.WriteLine($"the Y cordinate is {y}");
    }


}
public class GridPuzzleGame : GridGameBase<int>
{
    public GridPuzzleGame(int size = defSize) : base(size)
    {
        userTile = 0;
        StartGame();
    }
    protected override bool CheckWin() // Checks so that every number is in order starting from 1
    {
        int k = 1;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (k == width * height)
                {
                    return true;
                }
                if(!(puzzleGrid[i, j] == k))
                {
                    return false;
                }
                k++;
            }
        }
        return true;
    }
    protected override int CreateTile(int index)
    {
        return index + 1;
    }
    protected override void PrintGridTile(int row, int col)
        {
            if (puzzleGrid[row, col].Equals(userTile)) { Console.Write("   "); } // Prints the userTile seperatly from the others
            else if(puzzleGrid[row, col] > 99) Console.Write("{0}", puzzleGrid[row, col]);
            else if (puzzleGrid[row, col] > 9) Console.Write(" {0}", puzzleGrid[row, col]); // Prints in a way so that it looks nice with numbers with two digits
            else Console.Write(" {0} ", puzzleGrid[row,col]);
            Console.Write(" ");
        }
}

public class GridColorGame : GridGameBase<ConsoleColor>
{
    private List<ConsoleColor> usedColors;
    ConsoleColor[] flatGrid;
    public GridColorGame(int size = defSize) : base(size)
    {
        flatGrid = new ConsoleColor[width * height];
        usedColors = new List<ConsoleColor>();
        userTile = Console.BackgroundColor;
        StartGame();

    }
    protected override bool CheckWin()  
        {
            FlattenArray();
            for(int i = 0; i < height* width - 1; i++)
            {
                ConsoleColor currentColor = flatGrid[i];

                ConsoleColor nextColor = flatGrid[i+1];
                if (!currentColor.Equals(nextColor)){   // Checks if the color of the current value is different that the color of the next value
                    for(int j = i+2; j < height * width; j++)
                    {
                        ConsoleColor followingColor = flatGrid[j];   // If the current and next color are different, the followingColor will try to find a value with the same color as the "current", if it does it means that the user hasnt won since all tiles of the same color arent together
                        if (followingColor.Equals(currentColor))
                        {
                            return false;
                        }
                    }
                }

            }
            return x == width-1 && y == height-1;
        
        }
    protected override ConsoleColor CreateTile(int index)
    {
        ConsoleColor[] colors =  // Array of colors that arent too similar
        {
            ConsoleColor.Red, ConsoleColor.Blue,
            ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.Magenta,
            ConsoleColor.White, ConsoleColor.DarkGray, ConsoleColor.DarkGreen,
            ConsoleColor.DarkBlue, ConsoleColor.DarkRed, ConsoleColor.DarkYellow
        };

        ConsoleColor selectedColor;
        if(usedColors.Count == colors.Length)
        {
            usedColors.Clear();
        }
        if(index % height == 0 || usedColors.Count == 0)
        {
            do
            {
                selectedColor = colors[rand.Next(colors.Length)];   // Selects a random color until it has picked one that hasnt been picked yet
            } while (usedColors.Contains(selectedColor));
            usedColors.Add(selectedColor);
            return selectedColor;
        }
        return usedColors.Last();   
    }
    protected override void PrintGridTile(int row, int col)
    {
        Console.ForegroundColor = puzzleGrid[row, col];
        Console.BackgroundColor = puzzleGrid[row, col];
        Console.Write("   ");
        Console.Write(" ");

        Console.ResetColor();
        Console.Write("  ");

    }
    private void FlattenArray()  // Converts a 2d array into a 1d array
    {
        
        int flatIndex = 0;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                flatGrid[flatIndex++] = puzzleGrid[i, j];
            }

        }
    }

}
class Program
{
    public static void Main()
    {
        GridPuzzleGame numberGame = new GridPuzzleGame(10);
        //GridColorGame colorGame = new GridColorGame(9);
    }
}