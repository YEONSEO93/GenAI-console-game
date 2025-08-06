using AstroDefenderPro.Console;
using AstroDefenderPro.GameEngine.Core;
using AstroDefenderPro.GameEngine.Systems;
using Microsoft.Extensions.Configuration;

// ╔═══════════════════════════════════════════════════════════════════╗
// ║                        ASTRO DEFENDER PRO                         ║
// ║                   Advanced AI-Powered Space Game                   ║
// ╚═══════════════════════════════════════════════════════════════════╝

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.CursorVisible = false;
Console.Clear();

// Set console properties to prevent scrolling
try
{
    // Disable console scrolling by setting buffer equal to window
    if (OperatingSystem.IsWindows())
    {
        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
    }
}
catch
{
    // Ignore if not supported
}

// Initialize configuration
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Show start screen
StartScreen.Show();

// Get game speed
int speed = await GetGameSpeedAsync();

// Initialize game
Console.Clear();
var gameManager = new GameManager(configuration, 80, 20);
await gameManager.RunGameAsync(speed);

static async Task<int> GetGameSpeedAsync()
{
    int speed = 1; // Default: Slow
    
    while (true)
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;
            speed = key switch
            {
                ConsoleKey.Enter or ConsoleKey.D1 or ConsoleKey.NumPad1 => 1,
                ConsoleKey.D2 or ConsoleKey.NumPad2 => 2,
                ConsoleKey.D3 or ConsoleKey.NumPad3 => 3,
                ConsoleKey.D4 or ConsoleKey.NumPad4 => 4,
                _ => speed
            };
            
            if (key is ConsoleKey.Enter or ConsoleKey.D1 or ConsoleKey.NumPad1 or 
                ConsoleKey.D2 or ConsoleKey.NumPad2 or ConsoleKey.D3 or ConsoleKey.NumPad3 or
                ConsoleKey.D4 or ConsoleKey.NumPad4)
                break;
        }
        
        await Task.Delay(50);
    }
    
    return speed;
}
