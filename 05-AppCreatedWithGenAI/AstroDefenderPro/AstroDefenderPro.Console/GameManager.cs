using AstroDefenderPro.AI.Core;
using AstroDefenderPro.AI.Strategies;
using AstroDefenderPro.GameEngine.Core;
using AstroDefenderPro.GameEngine.Systems;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace AstroDefenderPro.Console;

public enum AIMode
{
    Off,
    Local,
    Azure,
    Ollama
}

public class GameManager
{
    private readonly GameWorld _gameWorld;
    private readonly RenderSystem _renderSystem;
    private readonly WeaponSystem _weaponSystem;
    private readonly GameStateAnalyzer _analyzer;
    private readonly AdvancedAIStrategy _aiStrategy;
    private readonly GameStats _gameStats;
    private readonly IConfiguration _configuration;
    
    private AIMode _currentAIMode = AIMode.Off;
    private float _gameSpeedMs;
    private bool _isRunning = true;
    private bool _isPaused = false;
    private bool _showFps = false;
    private bool _slowMotion = false;
    private string _aiInfo = "";
    
    private readonly Stopwatch _gameTimer;
    private DateTime _lastFrame;
    private DateTime _lastAIUpdate;
    private readonly Random _random;

    public GameManager(IConfiguration configuration, int width, int height)
    {
        _configuration = configuration;
        _gameWorld = new GameWorld(width, height);
        _renderSystem = new RenderSystem(width, height);
        _weaponSystem = new WeaponSystem(_gameWorld);
        _analyzer = new GameStateAnalyzer();
        _aiStrategy = new AdvancedAIStrategy();
        _gameStats = new GameStats();
        
        _gameTimer = new Stopwatch();
        _lastFrame = DateTime.Now;
        _lastAIUpdate = DateTime.Now;
        _random = new Random();
    }

    public async Task RunGameAsync(int speed)
    {
        _gameSpeedMs = speed switch
        {
            2 => 60f,
            3 => 35f,
            4 => 20f,
            _ => 100f
        };

        // Initialize console settings
        System.Console.Clear();
        System.Console.CursorVisible = false;
        
        // Try to set console buffer size to prevent scrolling
        try
        {
            System.Console.SetBufferSize(System.Console.WindowWidth, System.Console.WindowHeight);
        }
        catch
        {
            // Ignore if not supported on this platform
        }

        _gameWorld.StartNewGame();
        _gameTimer.Start();
        
        ShowInstructions();
        await Task.Delay(2000);
        
        // Clear console before starting game loop
        System.Console.Clear();

        while (_isRunning)
        {
            var frameStart = DateTime.Now;
            var deltaTime = (float)(frameStart - _lastFrame).TotalSeconds;
            
            // Clamp delta time to prevent huge jumps
            deltaTime = Math.Min(deltaTime, 0.1f);
            
            if (_slowMotion) deltaTime *= 0.3f;
            
            _lastFrame = frameStart;

            if (!_isPaused)
            {
                await ProcessInputAsync();
                await UpdateGameAsync(deltaTime);
                await ProcessAIAsync(deltaTime);
            }
            else
            {
                await ProcessPauseInputAsync();
            }

            RenderGame();
            UpdateFPS(deltaTime);

            // Frame rate limiting - increase target frame time for smoother gameplay
            var frameTime = (DateTime.Now - frameStart).TotalMilliseconds;
            var targetFrameTime = _gameSpeedMs;
            if (frameTime < targetFrameTime)
            {
                await Task.Delay((int)(targetFrameTime - frameTime));
            }
        }
    }

    private async Task ProcessInputAsync()
    {
        if (!System.Console.KeyAvailable) return;

        var key = System.Console.ReadKey(true).Key;
        
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                _gameWorld.Player.MoveLeft(0.1f, 0);
                break;
                
            case ConsoleKey.RightArrow:
                _gameWorld.Player.MoveRight(0.1f, _gameWorld.GameWidth);
                break;
                
            case ConsoleKey.Spacebar:
                if (_currentAIMode == AIMode.Off)
                    _weaponSystem.FirePlayerWeapon(_gameWorld.Player);
                break;
                
            case ConsoleKey.Z:
                if (_currentAIMode == AIMode.Off)
                    FireSpecialWeapon();
                break;
                
            case ConsoleKey.X:
                if (_currentAIMode == AIMode.Off && !_gameWorld.Player.HasShield)
                    _gameWorld.Player.ActivateShield();
                break;
                
            case ConsoleKey.C:
                _slowMotion = !_slowMotion;
                break;
                
            case ConsoleKey.P:
                _isPaused = !_isPaused;
                break;
                
            case ConsoleKey.F:
                _showFps = !_showFps;
                break;
                
            case ConsoleKey.S:
                await SaveScreenshotAsync();
                break;
                
            case ConsoleKey.A:
                await ToggleAzureAIAsync();
                break;
                
            case ConsoleKey.O:
                ToggleOllamaAI();
                break;
                
            case ConsoleKey.Q:
                _isRunning = false;
                break;
        }
    }

    private async Task ProcessPauseInputAsync()
    {
        if (!System.Console.KeyAvailable) return;

        var key = System.Console.ReadKey(true).Key;
        
        if (key == ConsoleKey.P)
        {
            _isPaused = false;
        }
        else if (key == ConsoleKey.Q)
        {
            _isRunning = false;
        }
    }

    private async Task UpdateGameAsync(float deltaTime)
    {
        // Update game world but enemies won't move due to disabled AI
        _gameWorld.Update(deltaTime);
        
        // Enemy AI movement and shooting is disabled in Enemy.UpdateAI()
        // No automatic enemy behavior

        // Update weapon systems
        _weaponSystem.UpdateMissileTargets();

        // Handle game state transitions
        if (_gameWorld.State == GameState.LevelTransition)
        {
            await HandleLevelTransitionAsync();
        }
        else if (_gameWorld.State == GameState.GameOver)
        {
            await HandleGameOverAsync();
        }
    }

    private async Task ProcessAIAsync(float deltaTime)
    {
        if (_currentAIMode == AIMode.Off) return;

        var timeSinceLastAI = (DateTime.Now - _lastAIUpdate).TotalSeconds;
        if (timeSinceLastAI < 0.2) return; // AI updates at 5 FPS max

        var analysis = _analyzer.AnalyzeGameState(_gameWorld);
        var decision = _aiStrategy.MakeDecision(analysis, (float)timeSinceLastAI);

        // Execute AI decision
        await ExecuteAIDecisionAsync(decision);

        // Update AI info for display
        _aiInfo = $"AI: {decision.Action} ({decision.Confidence:P0}) - {decision.Reasoning}";

        _lastAIUpdate = DateTime.Now;
        _gameStats.AiFps = 1f / (float)timeSinceLastAI;
    }

    private async Task ExecuteAIDecisionAsync(AIDecision decision)
    {
        switch (decision.Action)
        {
            case AIAction.MoveLeft:
                _gameWorld.Player.MoveLeft(0.1f, 0);
                break;
                
            case AIAction.MoveRight:
                _gameWorld.Player.MoveRight(0.1f, _gameWorld.GameWidth);
                break;
                
            case AIAction.Shoot:
                _weaponSystem.FirePlayerWeapon(_gameWorld.Player);
                break;
                
            case AIAction.SpecialWeapon:
                FireSpecialWeapon();
                break;
                
            case AIAction.ActivateShield:
                if (!_gameWorld.Player.HasShield)
                    _gameWorld.Player.ActivateShield();
                break;
        }

        await Task.CompletedTask;
    }

    private void FireSpecialWeapon()
    {
        switch (_gameWorld.Player.CurrentWeapon)
        {
            case WeaponType.Laser:
                _weaponSystem.FirePlayerWeapon(_gameWorld.Player);
                break;
            case WeaponType.Missile:
                _weaponSystem.FirePlayerWeapon(_gameWorld.Player);
                break;
            default:
                _weaponSystem.FirePlayerWeapon(_gameWorld.Player);
                break;
        }
    }

    private void RenderGame()
    {
        var displayInfo = _isPaused ? "PAUSED - Press P to continue" : 
                         _showFps ? _aiInfo : "";
                         
        _renderSystem.Render(_gameWorld, _gameStats, displayInfo);
        
        if (_isPaused)
        {
            RenderPauseScreen();
        }
    }

    private void RenderPauseScreen()
    {
        System.Console.SetCursorPosition(20, 12);
        System.Console.BackgroundColor = ConsoleColor.DarkBlue;
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.Write("     GAME PAUSED     ");
        System.Console.SetCursorPosition(20, 13);
        System.Console.Write(" P - Resume  Q - Quit ");
        System.Console.ResetColor();
    }

    private void UpdateFPS(float deltaTime)
    {
        _gameStats.FrameCount++;
        _gameStats.DeltaTime = deltaTime;
        
        var timeSinceLastUpdate = (DateTime.Now - _gameStats.LastFpsUpdate).TotalSeconds;
        if (timeSinceLastUpdate >= 1.0)
        {
            _gameStats.GameFps = _gameStats.FrameCount / (float)timeSinceLastUpdate;
            _gameStats.FrameCount = 0;
            _gameStats.LastFpsUpdate = DateTime.Now;
        }
    }

    private async Task HandleLevelTransitionAsync()
    {
        System.Console.SetCursorPosition(25, 12);
        System.Console.BackgroundColor = ConsoleColor.DarkGreen;
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.Write($"    LEVEL {_gameWorld.Level} COMPLETE!    ");
        System.Console.ResetColor();
        
        await Task.Delay(2000);
        _gameWorld.StartNewLevel();
    }

    private async Task HandleGameOverAsync()
    {
        System.Console.SetCursorPosition(25, 12);
        System.Console.BackgroundColor = ConsoleColor.DarkRed;
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.Write("    GAME OVER    ");
        System.Console.SetCursorPosition(20, 13);
        System.Console.Write($" Final Score: {_gameWorld.Player.Score:D6} ");
        System.Console.SetCursorPosition(20, 14);
        System.Console.Write(" Press any key to exit ");
        System.Console.ResetColor();
        
        System.Console.ReadKey(true);
        _isRunning = false;
    }

    private async Task SaveScreenshotAsync()
    {
        // Screenshot functionality (simplified)
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        System.Console.Beep(); // Audio feedback
        _aiInfo = $"Screenshot saved: screenshot_{timestamp}.txt";
        await Task.Delay(1000);
    }

    private async Task ToggleAzureAIAsync()
    {
        if (_currentAIMode == AIMode.Azure)
        {
            _currentAIMode = AIMode.Off;
            _aiInfo = "Azure AI disabled";
        }
        else
        {
            var endpoint = _configuration["AZURE_OPENAI_ENDPOINT"];
            var apiKey = _configuration["AZURE_OPENAI_APIKEY"];
            
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _aiInfo = "Azure AI not configured";
                await Task.Delay(2000);
                return;
            }
            
            _currentAIMode = AIMode.Azure;
            _aiInfo = "Azure AI enabled";
        }
    }

    private void ToggleOllamaAI()
    {
        if (_currentAIMode == AIMode.Ollama)
        {
            _currentAIMode = AIMode.Off;
            _aiInfo = "Ollama AI disabled";
        }
        else
        {
            _currentAIMode = AIMode.Ollama;
            _aiInfo = "Ollama AI enabled";
        }
    }

    private void ShowInstructions()
    {
        System.Console.Clear();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine("🎮 AstroDefender Pro - Manual Control Mode 🎮");
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine("CONTROLS:");
        System.Console.WriteLine("← → Arrow Keys: Move left/right");
        System.Console.WriteLine("SPACE: Shoot");
        System.Console.WriteLine("Z: Special weapon");
        System.Console.WriteLine("X: Shield");
        System.Console.WriteLine("P: Pause/Resume");
        System.Console.WriteLine("Q: Quit");
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("Enemy AI is DISABLED - you have full control!");
        System.Console.WriteLine("Game starting in 2 seconds...");
        System.Console.ResetColor();
    }
}
