using System.IO;
using System.Windows;

// -- Main -- 

GameLogic gameManager = new GameLogic();
InputManager input = new InputManager();

ConsoleColor[] colors = gameManager.GetConsoleColors();

Task loop = new Task(gameManager.DoGameLoop);
loop.Start();

for (int i = 0; i < colors.Length; i++)
{
    Console.Write(colors[i]);
}

while (gameManager.running)
{
    input.AddInputsToQueue();
    Console.WriteLine(input.inputQueue[input.inputQueue.Count - 1].Key);
}


// -- Classes --

class GameLogic
{
    float FRAMERATE = 24f;

    public bool running = true;
    public bool inLoop = false;

    void GameLoop()
    {
        //Console.WriteLine(Console.WindowWidth + ", " + Console.WindowHeight);
        /*if (currentDimensions != previousDimensions)
        {
            Draw.ClearScreen();
            FillScreen();
            previousDimensions = currentDimensions;
        }*/
        
        OnWindowResize();
    }

    public void DoGameLoop()
    {
        double deltaTime = (1f / FRAMERATE) * 1000;
        while (running)
        {
            if (!inLoop)
            {
                inLoop = true;
                GameLoop();
                Thread.Sleep((int)deltaTime);
                inLoop = false;
            }
        }
    }

    void FillScreen()
    {
        for (int i = 0; i < Console.WindowHeight; i++)
        {
            for (int j = 0; j < Console.WindowWidth; j++)
            {
                Console.Write("#");
            }
        }
    }


    (int, int) currentDimensions;
    (int, int) previousDimensions = (0,0);

    void OnWindowResize()
    {
        
        currentDimensions = (Console.WindowWidth, Console.WindowHeight);

        if (currentDimensions != previousDimensions)
        {
            //FillScreen();
            
            Console.SetCursorPosition(0, 0);
        }

        previousDimensions = currentDimensions;
    }

    public ConsoleColor[] GetConsoleColors()
    {
        return (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor)); // 100% stolen from docs
    }
}

class InputManager
{
    public List<ConsoleKeyInfo> inputQueue = new List<ConsoleKeyInfo>();

    public static ConsoleKeyInfo GetInput()
    {
        return Console.ReadKey(true);
    }

    public void AddInputsToQueue()
    {
        inputQueue.Add(GetInput());
    }
}

static class Draw
{
    public static void ClearScreen()
    {
        Console.SetCursorPosition(0, 0);

        for (int i = 0; i < 10; i++) // TODO replace 10 with screen height
        {
            Console.WriteLine("                                        ");
        }

        Console.SetCursorPosition(0, 0);
    }
}

static class FileReader
{

}

static class TileMaps
{
    static Dictionary<string, string> MapTiles = new Dictionary<string, string>
    {
        { "s", "snake" },
        { "#", "wall" },
        { ".", "floor" }
    };

    static Dictionary<string, ConsoleChar> AsciiTilemap = new Dictionary<string, ConsoleChar>
    {
        //{ "snake",  new ConsoleChar("S", )}
    };
}

public class ConsoleChar
{
    string character;
    ConsoleColor foreground;
    ConsoleColor background;

    public ConsoleChar( string _character, ConsoleColor _foregroundColor, ConsoleColor _backgroundColor )
    {
        this.character = _character;
        this.foreground = _foregroundColor;
        this.background = _backgroundColor;
    }
}