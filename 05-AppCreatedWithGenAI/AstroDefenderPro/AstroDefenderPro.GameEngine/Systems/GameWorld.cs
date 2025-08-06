using AstroDefenderPro.GameEngine.Core;
using AstroDefenderPro.GameEngine.Entities;

namespace AstroDefenderPro.GameEngine.Systems;

public class GameWorld
{
    public Player Player { get; private set; } = null!;
    public List<Enemy> Enemies { get; private set; } = null!;
    public List<Bullet> PlayerBullets { get; private set; } = null!;
    public List<Bullet> EnemyBullets { get; private set; } = null!;
    public List<Laser> Lasers { get; private set; } = null!;
    public List<Missile> Missiles { get; private set; } = null!;
    public List<PowerUp> PowerUps { get; private set; } = null!;
    public List<Explosion> Explosions { get; private set; } = null!;
    public List<Particle> Particles { get; private set; } = null!;
    
    public int GameWidth { get; }
    public int GameHeight { get; }
    public int Level { get; set; } = 1;
    public GameState State { get; set; } = GameState.Menu;
    public Random Random { get; }

    public GameWorld(int width, int height)
    {
        GameWidth = width;
        GameHeight = height;
        Random = new Random();
        
        InitializeLists();
        InitializePlayer();
    }

    private void InitializeLists()
    {
        Enemies = new List<Enemy>();
        PlayerBullets = new List<Bullet>();
        EnemyBullets = new List<Bullet>();
        Lasers = new List<Laser>();
        Missiles = new List<Missile>();
        PowerUps = new List<PowerUp>();
        Explosions = new List<Explosion>();
        Particles = new List<Particle>();
    }

    private void InitializePlayer()
    {
        Player = new Player(new Vector2(GameWidth / 2, GameHeight - 3));
    }

    public void StartNewGame()
    {
        State = GameState.Playing;
        Level = 1;
        Player.Score = 0;
        Player.Lives = 3;
        Player.Health = Player.MaxHealth;
        Player.Position = new Vector2(GameWidth / 2, GameHeight - 3);
        
        ClearAllEntities();
        SpawnEnemiesForLevel(Level);
    }

    public void StartNewLevel()
    {
        Level++;
        ClearProjectiles();
        SpawnEnemiesForLevel(Level);
        
        // Maybe spawn a power-up at level start
        if (Random.NextDouble() < 0.3)
        {
            SpawnRandomPowerUp();
        }
    }

    private void ClearAllEntities()
    {
        Enemies.Clear();
        PlayerBullets.Clear();
        EnemyBullets.Clear();
        Lasers.Clear();
        Missiles.Clear();
        PowerUps.Clear();
        Explosions.Clear();
        Particles.Clear();
    }

    private void ClearProjectiles()
    {
        PlayerBullets.Clear();
        EnemyBullets.Clear();
        Lasers.Clear();
        Missiles.Clear();
    }

    public void SpawnEnemiesForLevel(int level)
    {
        int baseEnemyCount = 5 + level * 2;
        int strongEnemyCount = level / 3;
        int fastEnemyCount = level / 2;
        
        // Spawn basic enemies in formation
        for (int i = 0; i < baseEnemyCount; i++)
        {
            float x = 5 + (i % 8) * 8;
            float y = 2 + (i / 8) * 3;
            Enemies.Add(new BasicEnemy(new Vector2(x, y)));
        }
        
        // Spawn fast enemies
        for (int i = 0; i < fastEnemyCount; i++)
        {
            float x = 10 + i * 15;
            float y = 5;
            Enemies.Add(new FastEnemy(new Vector2(x, y)));
        }
        
        // Spawn strong enemies
        for (int i = 0; i < strongEnemyCount; i++)
        {
            float x = 15 + i * 20;
            float y = 8;
            Enemies.Add(new StrongEnemy(new Vector2(x, y)));
        }
        
        // Spawn boss every 5 levels
        if (level % 5 == 0)
        {
            Enemies.Add(new BossEnemy(new Vector2(GameWidth / 2 - 1, 3)));
        }
    }

    public void SpawnRandomPowerUp()
    {
        var powerUpTypes = Enum.GetValues<PowerUpType>();
        var randomType = powerUpTypes[Random.Next(powerUpTypes.Length)];
        var x = Random.Next(5, GameWidth - 5);
        PowerUps.Add(new PowerUp(new Vector2(x, 0), randomType));
    }

    public void SpawnExplosion(Vector2 position, int particleCount = 5)
    {
        Explosions.Add(new Explosion(position));
        
        // Spawn particles
        for (int i = 0; i < particleCount; i++)
        {
            var angle = Random.NextDouble() * Math.PI * 2;
            var speed = Random.Next(10, 30);
            var velocity = new Vector2(
                (float)(Math.Cos(angle) * speed),
                (float)(Math.Sin(angle) * speed)
            );
            
            var colors = new[] { ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.White };
            var sprites = new[] { '*', '.', '+', 'x' };
            
            Particles.Add(new Particle(
                position,
                velocity,
                Random.NextSingle() * 0.8f + 0.2f,
                sprites[Random.Next(sprites.Length)],
                colors[Random.Next(colors.Length)]
            ));
        }
    }

    public void Update(float deltaTime)
    {
        if (State != GameState.Playing) return;

        // Update player
        Player.Update(deltaTime);

        // Update all entities
        UpdateEntities(deltaTime);
        
        // Handle collisions
        HandleCollisions();
        
        // Remove inactive entities
        RemoveInactiveEntities();
        
        // Check win/lose conditions
        CheckGameState();
        
        // Randomly spawn power-ups
        if (Random.NextDouble() < 0.001) // Small chance each frame
        {
            SpawnRandomPowerUp();
        }
    }

    private void UpdateEntities(float deltaTime)
    {
        foreach (var enemy in Enemies) enemy.Update(deltaTime);
        foreach (var bullet in PlayerBullets) bullet.Update(deltaTime);
        foreach (var bullet in EnemyBullets) bullet.Update(deltaTime);
        foreach (var laser in Lasers) laser.Update(deltaTime);
        foreach (var missile in Missiles) missile.Update(deltaTime);
        foreach (var powerUp in PowerUps) powerUp.Update(deltaTime);
        foreach (var explosion in Explosions) explosion.Update(deltaTime);
        foreach (var particle in Particles) particle.Update(deltaTime);
    }

    private void HandleCollisions()
    {
        // Player bullets vs enemies
        foreach (var bullet in PlayerBullets.Where(b => b.IsActive))
        {
            foreach (var enemy in Enemies.Where(e => e.IsActive))
            {
                if (bullet.CollidesWith(enemy))
                {
                    enemy.TakeDamage(bullet.Damage);
                    bullet.IsActive = false;
                    Player.OnBulletDestroyed();
                    
                    if (!enemy.IsActive)
                    {
                        Player.Score += enemy.Points;
                        SpawnExplosion(enemy.Position);
                    }
                    break;
                }
            }
        }
        
        // Lasers vs enemies
        foreach (var laser in Lasers.Where(l => l.IsActive))
        {
            foreach (var enemy in Enemies.Where(e => e.IsActive))
            {
                if (laser.CollidesWith(enemy))
                {
                    enemy.TakeDamage(laser.Damage);
                    if (!enemy.IsActive)
                    {
                        Player.Score += enemy.Points;
                        SpawnExplosion(enemy.Position);
                    }
                }
            }
        }
        
        // Missiles vs enemies
        foreach (var missile in Missiles.Where(m => m.IsActive))
        {
            foreach (var enemy in Enemies.Where(e => e.IsActive))
            {
                if (missile.CollidesWith(enemy))
                {
                    enemy.TakeDamage(missile.Damage);
                    missile.IsActive = false;
                    SpawnExplosion(missile.Position, 8);
                    
                    if (!enemy.IsActive)
                    {
                        Player.Score += enemy.Points;
                        SpawnExplosion(enemy.Position);
                    }
                    break;
                }
            }
        }
        
        // Enemy bullets vs player
        foreach (var bullet in EnemyBullets.Where(b => b.IsActive))
        {
            if (bullet.CollidesWith(Player))
            {
                Player.TakeDamage(bullet.Damage);
                bullet.IsActive = false;
                break;
            }
        }
        
        // Power-ups vs player
        foreach (var powerUp in PowerUps.Where(p => p.IsActive))
        {
            if (powerUp.CollidesWith(Player))
            {
                ApplyPowerUp(powerUp.Type);
                powerUp.IsActive = false;
                break;
            }
        }
    }

    private void ApplyPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.DoubleShot:
                Player.CurrentWeapon = WeaponType.Double;
                Player.WeaponLevel = Math.Max(Player.WeaponLevel, 2);
                break;
            case PowerUpType.TripleShot:
                Player.CurrentWeapon = WeaponType.Triple;
                Player.WeaponLevel = Math.Max(Player.WeaponLevel, 3);
                break;
            case PowerUpType.Laser:
                Player.CurrentWeapon = WeaponType.Laser;
                break;
            case PowerUpType.Shield:
                Player.ActivateShield();
                break;
            case PowerUpType.Health:
                Player.Heal();
                break;
            case PowerUpType.Speed:
                Player.ActivateSpeedBoost();
                break;
        }
    }

    private void RemoveInactiveEntities()
    {
        Enemies.RemoveAll(e => !e.IsActive);
        PlayerBullets.RemoveAll(b => !b.IsActive);
        EnemyBullets.RemoveAll(b => !b.IsActive);
        Lasers.RemoveAll(l => !l.IsActive);
        Missiles.RemoveAll(m => !m.IsActive);
        PowerUps.RemoveAll(p => !p.IsActive);
        Explosions.RemoveAll(e => !e.IsActive);
        Particles.RemoveAll(p => !p.IsActive);
    }

    private void CheckGameState()
    {
        // Check if player is dead
        if (Player.Lives <= 0)
        {
            State = GameState.GameOver;
            return;
        }
        
        // Check if all enemies are defeated
        if (!Enemies.Any(e => e.IsActive))
        {
            State = GameState.LevelTransition;
        }
    }

    public List<GameEntity> GetAllEntities()
    {
        var entities = new List<GameEntity> { Player };
        entities.AddRange(Enemies.Cast<GameEntity>());
        entities.AddRange(PlayerBullets.Cast<GameEntity>());
        entities.AddRange(EnemyBullets.Cast<GameEntity>());
        entities.AddRange(Lasers.Cast<GameEntity>());
        entities.AddRange(Missiles.Cast<GameEntity>());
        entities.AddRange(PowerUps.Cast<GameEntity>());
        entities.AddRange(Explosions.Cast<GameEntity>());
        entities.AddRange(Particles.Cast<GameEntity>());
        return entities;
    }
}
