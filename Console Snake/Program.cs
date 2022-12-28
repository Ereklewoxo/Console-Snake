using System.Runtime.InteropServices;
using System.Text;

namespace Console_Snake
{
    public static class CustomColor
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int handle);
        public static void Color()
        {
            var handle = GetStdHandle(-11);
            GetConsoleMode(handle, out int mode);
            SetConsoleMode(handle, mode | 0x4);
        }
    }
    public class Title
    {
        public static string Logo = "" +
                  "     CONSOLE" +
                "\n     ██▀▀█ ██  █ ▄█▀█▄ █   █ █▀▀▀▀" +
                "\n      ▀█▄  █▀█▄█ █   █ █▄▄█▀ ████" +
                "\n     █▄▄██ █  ██ █   █ █  ▀█ █▄▄▄▄\n\n";
    }
    public class Menu
    {
        public static List<Tuple<string, string>> ThemeColors()
        {
            var colors = new List<Tuple<string, string>>();
            colors.Add(new Tuple<string, string>("\x1b[38;2;0;0;0m", "\x1b[48;2;255;255;255m"));
            colors.Add(new Tuple<string, string>("\x1b[38;2;32;55;0m", "\x1b[48;2;142;222;8m"));
            colors.Add(new Tuple<string, string>("\x1b[38;2;250;0;0m", "\x1b[48;2;10;0;0m"));
            colors.Add(new Tuple<string, string>("\x1b[38;2;229;235;7m", "\x1b[48;2;40;45;240m"));
            colors.Add(new Tuple<string, string>("\x1b[38;2;58;33;3m", "\x1b[48;2;243;240;195m"));
            return colors;
        }
        public static int Theme = 0;
        public static int MenuTheme()
        {
            ConsoleKeyInfo key;
            int selected = 0;
            do
            {
                if (selected == 0)
                {
                    Console.WriteLine("" +
                        "                >START<\n" +
                        "                 THEME ");
                }
                else if (selected == 1)
                {
                    Console.WriteLine("" +
                        "                 START \n" +
                        "                <THEME>");

                }
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.DownArrow)
                    selected = 1;
                else if (key.Key == ConsoleKey.UpArrow)
                    selected = 0;
                if (selected == 1)
                {
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            Theme = Math.Max(Theme - 1, 0);
                            Console.WriteLine(ThemeColors()[Theme].Item1 + ThemeColors()[Theme].Item2);
                            Console.Clear();
                            Console.WriteLine(Title.Logo + "\n\n");
                            break;
                        case ConsoleKey.RightArrow:
                            Theme = Math.Min(Theme + 1, ThemeColors().Count - 1);
                            Console.WriteLine(ThemeColors()[Theme].Item1 + ThemeColors()[Theme].Item2);
                            Console.Clear();
                            Console.WriteLine(Title.Logo + "\n\n");
                            break;
                    }
                }
                Console.SetCursorPosition(0, Console.CursorTop - 2);
            } while (key.Key != ConsoleKey.Enter);
            return Theme;
        }
    }
    public class Game
    {
        public static void GameLoop()
        {
            ConsoleKeyInfo key;
            Random rnd = new();
            int direction = 1;
            var snakeHead = new List<int> { 5, 5 };
            var snakeTailX = new List<int> { 4, 3 };
            var snakeTailY = new List<int> { 5, 5 };
            bool escape;
            bool noFruit = true;
            bool gameOver = false;
            int fruitX = 0;
            int fruitY = 0;
            int score = 0;
            int speed = 160;
            do
            {
                Console.Write("\x1b[48;2;0;0;0m\x1b[38;2;255;255;255m");
                if (speed > 20)
                    Console.WriteLine($" SCORE: {score}              SPEED: 1px/{speed}ms ");
                else
                    Console.WriteLine($" SCORE: {score}                  SPEED: MAX ");
                Console.Write(Menu.ThemeColors()[Menu.Theme].Item1 + Menu.ThemeColors()[Menu.Theme].Item2);
                if (noFruit)
                {
                    do
                    {
                        escape = true;
                        fruitX = rnd.Next(0, 20);
                        fruitY = rnd.Next(0, 20);
                        for (int i = 0; i < snakeTailX.Count; i++)
                        {
                            if (snakeTailX[i] == fruitX && snakeTailY[i] == fruitY)
                                escape = false;
                        }
                        if (fruitX == snakeHead[0] && fruitY == snakeHead[1])
                            escape = false;
                    } while (escape == false);
                    noFruit = false;
                }
                for (int i = 0; i < snakeTailX.Count; i++)
                {
                    if ((snakeTailX[i] == snakeHead[0] && snakeTailY[i] == snakeHead[1]) || snakeHead[0] < 0 || snakeHead[0] > 19 || snakeHead[1] < 0 || snakeHead[1] > 19)
                    {
                        gameOver = true;
                        break;
                    }
                }
                StringBuilder gameScreen = new();
                for (int y = 0; y < 20; y++)
                {
                    for (int x = 0; x < 20; x++)
                    {
                        string artSymbol;
                        if ((snakeHead[0] == x && snakeHead[1] == y) || (fruitX == x && fruitY == y))
                            artSymbol = "██";
                        else
                            artSymbol = "  ";
                        for (int i = 0; i < snakeTailX.Count; i++)
                        {
                            if (snakeTailX[i] == x && snakeTailY[i] == y)
                                artSymbol = "██";
                        }
                        gameScreen.Append(artSymbol);
                    }
                    if (y < 19)
                        gameScreen.AppendLine();
                }
                Console.Write(gameScreen);
                if (snakeHead[0] == fruitX && snakeHead[1] == fruitY)
                {
                    speed = Math.Max(speed-5, 20);
                    score++;
                    noFruit = true;
                    snakeTailX.Add(0);
                    snakeTailY.Add(0);
                }
                for (int i = snakeTailX.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        snakeTailX[i] = snakeHead[0];
                        snakeTailY[i] = snakeHead[1];
                    }
                    else
                    {
                        snakeTailX[i] = snakeTailX[i - 1];
                        snakeTailY[i] = snakeTailY[i - 1];
                    }
                }
                if (Console.KeyAvailable)
                {
                    key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.RightArrow && direction != 3)
                        direction = 1;
                    else if (key.Key == ConsoleKey.UpArrow && direction != 2)
                        direction = 0;
                    else if (key.Key == ConsoleKey.DownArrow && direction != 0)
                        direction = 2;
                    else if (key.Key == ConsoleKey.LeftArrow && direction != 1)
                        direction = 3;
                }
                switch (direction)
                {
                    case 0:
                        snakeHead[1]--;
                        break;
                    case 1:
                        snakeHead[0]++;
                        break;
                    case 2:
                        snakeHead[1]++;
                        break;
                    case 3:
                        snakeHead[0]--;
                        break;
                }
                Console.SetCursorPosition(0, Console.CursorTop - 20);
                Task.Delay(speed).Wait();
            } while (gameOver == false);
            Console.WriteLine($"\n               GAME OVER");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo key;
            Console.WindowHeight = 21;
            Console.WindowWidth = 40;
            Console.CursorVisible = false;
            Console.Write(Menu.ThemeColors()[Menu.Theme].Item1 + Menu.ThemeColors()[Menu.Theme].Item2);
            Console.Clear();
            do
            {
                Console.WriteLine(Title.Logo);
                Menu.MenuTheme();
                Console.Clear();
                Game.GameLoop();
                key = Console.ReadKey(true);
            } while (key.Key != ConsoleKey.Escape);
        }
    }
} 