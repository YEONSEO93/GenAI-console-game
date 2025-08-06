using AstroDefenderPro.GameEngine.Core;
using AstroDefenderPro.GameEngine.Entities;

namespace AstroDefenderPro.GameEngine.Systems;

public class RenderSystem
{
    private char[,] _charBuffer;
    private ConsoleColor[,] _colorBuffer;
    private readonly int _width;
    private readonly int _height;
    private readonly int _uiHeight = 5;

    public RenderSystem(int width, int height)
    {
        _width = width;
        _height = height + _uiHeight;
        _charBuffer = new char[_height, _width];
        _colorBuffer = new ConsoleColor[_height, _width];
    }

    public void Render(GameWorld gameWorld, GameStats stats, string aiInfo = "")
    {
        ClearBuffers();
        RenderGameBorder();
        RenderEntities(gameWorld);
        RenderUI(gameWorld, stats, aiInfo);
        DrawToConsoleImproved();
    }

    private void DrawToConsoleImproved()
    {
        // Use a different approach - rewrite character by character to avoid scrolling
        try
        {
            Console.SetCursorPosition(0, 0);
            
            for (int y = 0; y < _height; y++)
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < _width; x++)
                {
                    var color = _colorBuffer[y, x];
                    if (color != Console.ForegroundColor)
                    {
                        Console.ForegroundColor = color;
                    }
                    Console.Write(_charBuffer[y, x]);
                }
            }
            
            Console.ResetColor();
        }
        catch (ArgumentOutOfRangeException)
        {
            // Ignore cursor position errors - console window might be too small
        }
    }

    private void ClearBuffers()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _charBuffer[y, x] = ' ';
                _colorBuffer[y, x] = ConsoleColor.Black;
            }
        }
    }

    private void RenderGameBorder()
    {
        // Top border
        for (int x = 0; x < _width; x++)
        {
            _charBuffer[_uiHeight, x] = '═';
            _colorBuffer[_uiHeight, x] = ConsoleColor.Gray;
        }
        
        // Side borders
        for (int y = _uiHeight; y < _height; y++)
        {
            _charBuffer[y, 0] = '║';
            _charBuffer[y, _width - 1] = '║';
            _colorBuffer[y, 0] = ConsoleColor.Gray;
            _colorBuffer[y, _width - 1] = ConsoleColor.Gray;
        }
        
        // Bottom border
        for (int x = 0; x < _width; x++)
        {
            _charBuffer[_height - 1, x] = '═';
            _colorBuffer[_height - 1, x] = ConsoleColor.Gray;
        }
        
        // Corners
        _charBuffer[_uiHeight, 0] = '╔';
        _charBuffer[_uiHeight, _width - 1] = '╗';
        _charBuffer[_height - 1, 0] = '╚';
        _charBuffer[_height - 1, _width - 1] = '╝';
    }

    private void RenderEntities(GameWorld gameWorld)
    {
        var gameHeight = _height - _uiHeight - 1;
        var gameWidth = _width - 2;
        
        // Render all entities
        foreach (var entity in gameWorld.GetAllEntities().Where(e => e.IsActive))
        {
            RenderEntity(entity, gameWidth, gameHeight);
        }
    }

    private void RenderEntity(GameEntity entity, int gameWidth, int gameHeight)
    {
        // Offset for game area (after UI and border)
        int offsetX = 1;
        int offsetY = _uiHeight + 1;
        
        entity.Render(_charBuffer, _colorBuffer, gameWidth, gameHeight);
        
        // Adjust positions for offset
        int x = (int)entity.Position.X + offsetX;
        int y = (int)entity.Position.Y + offsetY;
        
        if (x >= offsetX && x < _width - 1 && y >= offsetY && y < _height - 1)
        {
            for (int i = 0; i < entity.Height; i++)
            {
                for (int j = 0; j < entity.Width; j++)
                {
                    int drawX = x + j;
                    int drawY = y + i;
                    
                    if (drawX < _width - 1 && drawY < _height - 1)
                    {
                        int spriteIndex = i * entity.Width + j;
                        if (spriteIndex < entity.Sprite.Length)
                        {
                            _charBuffer[drawY, drawX] = entity.Sprite[spriteIndex];
                            _colorBuffer[drawY, drawX] = entity.Color;
                        }
                    }
                }
            }
        }
    }

    private void RenderUI(GameWorld gameWorld, GameStats stats, string aiInfo)
    {
        var player = gameWorld.Player;
        
        // Line 0: Score and Level
        RenderText(0, 0, $"Score: {player.Score:D6}", ConsoleColor.White);
        RenderText(0, 20, $"Level: {gameWorld.Level}", ConsoleColor.Yellow);
        RenderText(0, 35, $"Lives: {player.Lives}", ConsoleColor.Green);
        
        // Line 1: Health and Weapon
        RenderText(1, 0, $"Health: ", ConsoleColor.White);
        RenderHealthBar(1, 8, player.Health, player.MaxHealth);
        RenderText(1, 20, $"Weapon: {player.CurrentWeapon}", GetWeaponColor(player.CurrentWeapon));
        
        // Line 2: Power-up status
        var powerUpText = "";
        if (player.HasShield)
            powerUpText += $"SHIELD({player.ShieldTimeRemaining:F1}s) ";
        if (player.HasSpeedBoost)
            powerUpText += $"SPEED({player.SpeedBoostTimeRemaining:F1}s) ";
        
        RenderText(2, 0, $"Status: {powerUpText}", ConsoleColor.Cyan);
        
        // Line 3: FPS and AI info
        RenderText(3, 0, $"FPS: {stats.GameFps:F1}", ConsoleColor.Gray);
        RenderText(3, 12, $"AI FPS: {stats.AiFps:F1}", ConsoleColor.Gray);
        RenderText(3, 25, aiInfo, ConsoleColor.Yellow);
        
        // Line 4: Enemy count
        var enemyCount = gameWorld.Enemies.Count(e => e.IsActive);
        RenderText(4, 0, $"Enemies: {enemyCount}", ConsoleColor.Red);
    }

    private void RenderHealthBar(int row, int col, int health, int maxHealth)
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (col + i < _width)
            {
                _charBuffer[row, col + i] = i < health ? '█' : '░';
                _colorBuffer[row, col + i] = i < health ? ConsoleColor.Green : ConsoleColor.DarkGray;
            }
        }
    }

    private ConsoleColor GetWeaponColor(WeaponType weapon)
    {
        return weapon switch
        {
            WeaponType.Basic => ConsoleColor.White,
            WeaponType.Double => ConsoleColor.Blue,
            WeaponType.Triple => ConsoleColor.DarkBlue,
            WeaponType.Laser => ConsoleColor.Cyan,
            WeaponType.Missile => ConsoleColor.Yellow,
            WeaponType.Spread => ConsoleColor.Magenta,
            _ => ConsoleColor.Gray
        };
    }

    private void RenderText(int row, int col, string text, ConsoleColor color)
    {
        for (int i = 0; i < text.Length && col + i < _width; i++)
        {
            _charBuffer[row, col + i] = text[i];
            _colorBuffer[row, col + i] = color;
        }
    }

    private void DrawToConsole()
    {
        // This method is replaced by DrawToConsoleImproved
        // Kept for compatibility but not used
    }
}

public class GameStats
{
    public float GameFps { get; set; }
    public float AiFps { get; set; }
    public float DeltaTime { get; set; }
    public int FrameCount { get; set; }
    public DateTime LastFpsUpdate { get; set; } = DateTime.Now;
}
