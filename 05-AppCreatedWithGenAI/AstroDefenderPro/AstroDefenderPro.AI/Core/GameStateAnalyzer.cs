using AstroDefenderPro.GameEngine.Core;
using AstroDefenderPro.GameEngine.Entities;
using AstroDefenderPro.GameEngine.Systems;

namespace AstroDefenderPro.AI.Core;

public enum AIAction
{
    None,
    MoveLeft,
    MoveRight,
    Shoot,
    SpecialWeapon,
    ActivateShield
}

public class GameAnalysis
{
    public Vector2 PlayerPosition { get; set; }
    public int PlayerHealth { get; set; }
    public int PlayerLives { get; set; }
    public WeaponType CurrentWeapon { get; set; }
    public bool HasShield { get; set; }
    public float ShieldTimeRemaining { get; set; }
    
    public List<EnemyInfo> Enemies { get; set; } = [];
    public List<ProjectileInfo> EnemyBullets { get; set; } = [];
    public List<PowerUpInfo> PowerUps { get; set; } = [];
    
    public int Score { get; set; }
    public int Level { get; set; }
    public GameState GameState { get; set; }
    
    // Strategic analysis
    public float ThreatLevel { get; set; }
    public Vector2? NearestThreat { get; set; }
    public Vector2? BestPowerUp { get; set; }
    public float OptimalPosition { get; set; }
    public bool ShouldPrioritizeDefense { get; set; }
}

public class EnemyInfo
{
    public Vector2 Position { get; set; }
    public GameEntityType Type { get; set; }
    public int Health { get; set; }
    public int Points { get; set; }
    public float DistanceToPlayer { get; set; }
    public bool CanShoot { get; set; }
    public float ThreatLevel { get; set; }
}

public class ProjectileInfo
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float TimeToReachPlayer { get; set; }
    public float ThreatLevel { get; set; }
}

public class PowerUpInfo
{
    public Vector2 Position { get; set; }
    public PowerUpType Type { get; set; }
    public float DistanceToPlayer { get; set; }
    public float Priority { get; set; }
    public float TimeToReach { get; set; }
}

public class AIDecision
{
    public AIAction Action { get; set; }
    public float Confidence { get; set; }
    public string Reasoning { get; set; } = "";
    public Vector2? TargetPosition { get; set; }
    public float ExpectedReward { get; set; }
    public List<string> Considerations { get; set; } = [];
}

public class GameStateAnalyzer
{
    public GameAnalysis AnalyzeGameState(GameWorld gameWorld)
    {
        var player = gameWorld.Player;
        var analysis = new GameAnalysis
        {
            PlayerPosition = player.Position,
            PlayerHealth = player.Health,
            PlayerLives = player.Lives,
            CurrentWeapon = player.CurrentWeapon,
            HasShield = player.HasShield,
            ShieldTimeRemaining = player.ShieldTimeRemaining,
            Score = player.Score,
            Level = gameWorld.Level,
            GameState = gameWorld.State
        };

        // Analyze enemies
        foreach (var enemy in gameWorld.Enemies.Where(e => e.IsActive))
        {
            var distance = player.Position.Distance(enemy.Position);
            analysis.Enemies.Add(new EnemyInfo
            {
                Position = enemy.Position,
                Type = enemy.EntityType,
                Health = enemy.Health,
                Points = enemy.Points,
                DistanceToPlayer = distance,
                CanShoot = enemy.CanShoot,
                ThreatLevel = CalculateEnemyThreat(enemy, distance)
            });
        }

        // Analyze enemy bullets
        foreach (var bullet in gameWorld.EnemyBullets.Where(b => b.IsActive))
        {
            var timeToHit = CalculateTimeToHit(bullet, player.Position);
            analysis.EnemyBullets.Add(new ProjectileInfo
            {
                Position = bullet.Position,
                Velocity = bullet.Velocity,
                TimeToReachPlayer = timeToHit,
                ThreatLevel = CalculateBulletThreat(bullet, timeToHit)
            });
        }

        // Analyze power-ups
        foreach (var powerUp in gameWorld.PowerUps.Where(p => p.IsActive))
        {
            var distance = player.Position.Distance(powerUp.Position);
            analysis.PowerUps.Add(new PowerUpInfo
            {
                Position = powerUp.Position,
                Type = powerUp.Type,
                DistanceToPlayer = distance,
                Priority = CalculatePowerUpPriority(powerUp.Type, player),
                TimeToReach = distance / player.Speed
            });
        }

        // Calculate strategic metrics
        CalculateStrategicMetrics(analysis);

        return analysis;
    }

    private float CalculateEnemyThreat(Enemy enemy, float distance)
    {
        float baseThreat = enemy.EntityType switch
        {
            GameEntityType.BasicEnemy => 1f,
            GameEntityType.FastEnemy => 1.5f,
            GameEntityType.StrongEnemy => 2f,
            GameEntityType.BossEnemy => 5f,
            _ => 1f
        };

        // Closer enemies are more threatening
        float distanceFactor = Math.Max(0.1f, 20f / distance);
        
        // Enemies that can shoot are more dangerous
        float shootFactor = enemy.CanShoot ? 1.5f : 1f;

        return baseThreat * distanceFactor * shootFactor;
    }

    private float CalculateTimeToHit(Bullet bullet, Vector2 playerPos)
    {
        if (Math.Abs(bullet.Velocity.Y) < 0.1f) return float.MaxValue;
        
        float timeToPlayerY = (playerPos.Y - bullet.Position.Y) / bullet.Velocity.Y;
        if (timeToPlayerY <= 0) return float.MaxValue;

        float bulletXAtHit = bullet.Position.X + bullet.Velocity.X * timeToPlayerY;
        float distanceAtHit = Math.Abs(bulletXAtHit - playerPos.X);
        
        return distanceAtHit < 1f ? timeToPlayerY : float.MaxValue;
    }

    private float CalculateBulletThreat(Bullet bullet, float timeToHit)
    {
        if (timeToHit >= 10f) return 0f;
        return Math.Max(0f, 5f - timeToHit); // Higher threat for bullets that hit sooner
    }

    private float CalculatePowerUpPriority(PowerUpType type, Player player)
    {
        return type switch
        {
            PowerUpType.Health when player.Health < player.MaxHealth => 10f,
            PowerUpType.Shield when !player.HasShield => 8f,
            PowerUpType.TripleShot when player.CurrentWeapon != WeaponType.Triple => 7f,
            PowerUpType.DoubleShot when player.CurrentWeapon == WeaponType.Basic => 6f,
            PowerUpType.Laser => 6f,
            PowerUpType.Speed when !player.HasSpeedBoost => 4f,
            _ => 2f
        };
    }

    private void CalculateStrategicMetrics(GameAnalysis analysis)
    {
        // Calculate overall threat level
        analysis.ThreatLevel = analysis.Enemies.Sum(e => e.ThreatLevel) + 
                              analysis.EnemyBullets.Sum(b => b.ThreatLevel);

        // Find nearest threat
        var nearestEnemy = analysis.Enemies.OrderBy(e => e.DistanceToPlayer).FirstOrDefault();
        var nearestBullet = analysis.EnemyBullets.OrderBy(b => b.TimeToReachPlayer).FirstOrDefault();

        if (nearestBullet?.TimeToReachPlayer < 2f)
            analysis.NearestThreat = nearestBullet.Position;
        else if (nearestEnemy != null)
            analysis.NearestThreat = nearestEnemy.Position;

        // Find best power-up
        var bestPowerUp = analysis.PowerUps.OrderByDescending(p => p.Priority).FirstOrDefault();
        if (bestPowerUp != null)
            analysis.BestPowerUp = bestPowerUp.Position;

        // Calculate optimal position (avoid threats, approach power-ups)
        analysis.OptimalPosition = CalculateOptimalPosition(analysis);

        // Determine if should prioritize defense
        analysis.ShouldPrioritizeDefense = analysis.ThreatLevel > 3f || 
                                         analysis.EnemyBullets.Any(b => b.TimeToReachPlayer < 1.5f);
    }

    private float CalculateOptimalPosition(GameAnalysis analysis)
    {
        float optimalX = analysis.PlayerPosition.X;
        
        // Avoid immediate threats
        foreach (var bullet in analysis.EnemyBullets.Where(b => b.TimeToReachPlayer < 3f))
        {
            float avoidDirection = bullet.Position.X < analysis.PlayerPosition.X ? 1f : -1f;
            optimalX += avoidDirection * (4f - bullet.TimeToReachPlayer);
        }

        // Move towards power-ups
        if (analysis.BestPowerUp.HasValue && analysis.ThreatLevel < 2f)
        {
            float powerUpDirection = analysis.BestPowerUp.Value.X > analysis.PlayerPosition.X ? 1f : -1f;
            optimalX += powerUpDirection * 2f;
        }

        // Stay in bounds
        return Math.Max(2f, Math.Min(optimalX, 76f));
    }
}
