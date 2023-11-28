using System.IO;
using System.Windows;

// -- Main -- 

GameLogic gameManager = new GameLogic();
InputManager input = new InputManager();

Task loop = new Task(gameManager.DoGameLoop);
loop.Start();

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
    (int, int) previousDimensions = (0, 0);

    void GameLoop()
    {
        (int, int) currentDimensions = (Console.WindowWidth, Console.WindowHeight); // continue from here, put all this in functions
        //Console.WriteLine(Console.WindowWidth + ", " + Console.WindowHeight);
        /*if (currentDimensions != previousDimensions)
        {
            Draw.ClearScreen();
            FillScreen();
            previousDimensions = currentDimensions;
        }*/
        
        Console.WriteLine(currentDimensions + ", " + previousDimensions);
        previousDimensions = currentDimensions;
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
            for (int j = 0; j < Console.WindowWidth; i++)
            {
                Console.Write("#");
            }
        }
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
    static void DrawScreen(List<string> screenToDraw)
    {

    }

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

