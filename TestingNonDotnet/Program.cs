// -- Main -- 

Task loop = new Task(GameManager.DoGameLoop);
loop.Start();

GameManager.Logic.CreateDungeonFromMap();


while (GameManager.Logic.running)
{
    GameManager.Input.ManageInput();
}


// -- Classes --

static class GameManager
{
    static float FRAMERATE = 12f;

    public static InputManager Input;
    public static Rendering Renderer;
    public static MapFileReader Map;
    public static GameLogic Logic;

    static GameManager()
    {
        Input = new InputManager();
        Renderer = new Rendering();
        Map = new MapFileReader("map1.txt");
        Logic = new GameLogic();
    }

    static void GameLoop()
    {
        Renderer.OnWindowResize();

    }

    public static void DoGameLoop()
    {
        double deltaTime = (1f / FRAMERATE) * 1000;
        while (Logic.running)
        {
            if (!Logic.inLoop)
            {
                Logic.inLoop = true;
                GameLoop();
                Thread.Sleep((int)deltaTime);
                Logic.inLoop = false;
            }
        }
    }
}

class GameLogic
{
    public bool running = true;
    public bool inLoop = false;

    public List<string> dungeonMap = new List<string>();

    public Player player;

    enum Tiles
    {
        wall = '#',
        floor = '.',
        player_spawn = 'P',
    }

    // Inefficient and ugly but it only runs once
    // On the other hand, generating the rendered one is inefficient, ugly and runs every resize event
    public void CreateDungeonFromMap()
    {
        for (int i = 0; i < GameManager.Map.mapData.Count; i++)
        {
            dungeonMap.Add("");
            for (int j = 0; j < GameManager.Map.mapData[i].Length; j++)
            {
                switch (GameManager.Map.mapData[i][j])
                {
                    case (char)Tiles.wall:
                        dungeonMap[i] += (char)Tiles.wall;
                        break;

                    case (char)Tiles.floor:
                        dungeonMap[i] += (char)Tiles.floor;
                        break;

                    case (char)Tiles.player_spawn:
                        player = new Player(j, i);
                        dungeonMap[i] += (char)Tiles.floor;
                        break;
                }
            }
        }
    }
}

class Rendering
{
    int[] currentDimensions = new int[2];
    int[] previousDimensions = new int[] { 0, 0 };

    static Dictionary<string, ConsoleChar> CurrentTilemap = TileMaps.DefaultTilemap;

    public void OnWindowResize()
    {
        currentDimensions = new int[]{ Console.WindowWidth, Console.WindowHeight };

        for (int i = 0; i <= 1; i++)
        {
            if (currentDimensions[i] != previousDimensions[i])
            {
                GenerateBackground();
                Console.SetCursorPosition(0, 0);
                GenerateGameScreen();
                Console.SetCursorPosition(0, 0);
                RenderPlayer();
                Console.SetCursorPosition(0, 0);
                previousDimensions = currentDimensions;
                return;
            }
        }

        previousDimensions = currentDimensions;
    }

    public static void RenderCharacter(string tileName)
    {
        ConsoleChar renderedCharacter = CurrentTilemap[tileName];
        Console.ForegroundColor = renderedCharacter.foreground;
        Console.BackgroundColor = renderedCharacter.background;
        Console.Write(renderedCharacter.character);
    }

    public void GenerateBackground()
    {
        for (int i = 0; i < currentDimensions[1]; i++)
        {
            for (int j = 0; j < currentDimensions[0]; j++)
            {
                RenderCharacter("background_char");
            }
            Console.Write('\n');
        }
    }

    public void GenerateGameScreen() // these should be changed to screen block derived classes with a render function
    {
        string currentTile;
        char charAtThisIndex;

        for (int i = 0; i < GameManager.Logic.dungeonMap.Count; i++)
        {
            for (int j = 0; j < GameManager.Logic.dungeonMap[i].Length; j++)
            {
                charAtThisIndex = GameManager.Logic.dungeonMap[i][j];
                currentTile = TileMaps.MapTiles[charAtThisIndex];
                RenderCharacter(currentTile);
            }

            Console.ResetColor();
            Console.Write('\n');
        }
    }

    void RenderPlayer()
    {
        Console.SetCursorPosition(GameManager.Logic.player.currentPosition[0], GameManager.Logic.player.currentPosition[1]);
        RenderCharacter("player");
    }

    public ConsoleColor[] GetConsoleColors()
    {
        return (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor)); // 100% stolen from docs
    }
}

class InputManager
{
    public static ConsoleKeyInfo GetInput()
    {
        return Console.ReadKey(true);
    }

    public void ManageInput()
    {
        switch (GetInput().Key)
        {
            case ConsoleKey.Escape:
                GameManager.Logic.running = false;
                break;

            case ConsoleKey.W:
                GameManager.Logic.player.Move((0, 1));
                break;
        }
    }
}

public static class TileMaps
{
    public static Dictionary<char, string> MapTiles = new Dictionary<char, string>
    {
        { '#', "wall" },
        { '.', "floor" },
        { 'P', "player_spawn" },
        { '!', "enemy_spawn" },
    };

    public static Dictionary<string, ConsoleChar> DefaultTilemap = new Dictionary<string, ConsoleChar> // probably better as an enum
    {
        { "wall",  new ConsoleChar('▓', ConsoleColor.DarkGray, ConsoleColor.Cyan) },
        { "floor", new ConsoleChar('░', ConsoleColor.DarkGray, ConsoleColor.Gray) },
        { "background_char", new ConsoleChar('░', ConsoleColor.Gray, ConsoleColor.Black) },

        { "enemy", new ConsoleChar('!', ConsoleColor.DarkRed, ConsoleColor.White) },
        { "enemy_attacking", new ConsoleChar('!', ConsoleColor.DarkRed, ConsoleColor.Red) },

        { "player", new ConsoleChar('P', ConsoleColor.DarkBlue, ConsoleColor.White) }
    };
}

public class ConsoleChar
{
    public char character; // yeah i know it isn't a char, unicode is easier this way
    public ConsoleColor foreground;
    public ConsoleColor background;

    public static Dictionary<string, ConsoleColor> colors = new Dictionary<string, ConsoleColor>
    {
        { "black", ConsoleColor.Black }
    };

    public ConsoleChar( char _character, ConsoleColor _foregroundColor, ConsoleColor _backgroundColor )
    {
        this.character = _character;
        this.foreground = _foregroundColor;
        this.background = _backgroundColor;
    }
}


public class MapFileReader
{
    public List<string> mapData = new List<string> { };

    public MapFileReader(string mapFilePath)
    {
        StreamReader stream = new StreamReader(mapFilePath);
        string? line;

        while ((line = stream.ReadLine()) != null)
        {
            mapData.Add(line);
        }
    }
}



abstract class Entity
{
    public List<int> currentPosition;
    public char currentTile;

    //static ScreenBlock sprite;
}

class Player : Entity
{
    int maxHealth = 3;
    int currentHealth;

    public Player(int xPosition, int yPosition)
    {
        this.currentPosition = new List<int> { xPosition, yPosition };
        this.currentTile = '.';
    }

    public void Move((int, int) direction)
    {
        List<int> desiredPosition;
        desiredPosition = new List<int> { currentPosition[0] + direction.Item1, currentPosition[1] + direction.Item2 };

        if (GameManager.Logic.dungeonMap[desiredPosition[0]][desiredPosition[1]] != '#') 
        {
            currentPosition = desiredPosition;
        }
    }
}

class Enemy : Entity
{

}

// currently unused
abstract class ScreenBlock // remove abstract and use as child class? or have entity inherit from screen block?
{
    int height;
    int width;
    int[] position; // Top Left Character Coordinate of block
    string[] blockContent;

    public abstract void SetBlockContent();

    void Render()
    {
        for (int i = 0; i < blockContent.Length; i++)
        {
            for (int j = 0; j < blockContent[i].Length; j++)
            {

            }

            Console.Write('\n');
        }
    }
} 

