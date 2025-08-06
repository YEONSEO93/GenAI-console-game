using AstroDefenderPro.GameEngine.Core;

namespace AstroDefenderPro.GameEngine.Entities;

public class Bullet : GameEntity
{
    public int Damage { get; }
    public bool IsPlayerBullet { get; }
    
    public Bullet(Vector2 position, Vector2 velocity, bool isPlayerBullet, int damage = 1) 
        : base(position, isPlayerBullet ? GameEntityType.PlayerBullet : GameEntityType.EnemyBullet)
    {
        Velocity = velocity;
        IsPlayerBullet = isPlayerBullet;
        Damage = damage;
        Color = isPlayerBullet ? ConsoleColor.Green : ConsoleColor.Red;
        Sprite = isPlayerBullet ? ['|'] : ['!'];
        Width = 1;
        Height = 1;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        // Deactivate bullet if it goes off screen
        if (Position.Y < -1 || Position.Y > 25)
        {
            IsActive = false;
        }
    }
}

public class Laser : GameEntity
{
    public int Damage { get; }
    public float LifeTime { get; set; } = 0.1f;
    public float CurrentLife { get; set; }
    
    public Laser(Vector2 position, Vector2 direction, int damage = 2) 
        : base(position, GameEntityType.Laser)
    {
        Damage = damage;
        Color = ConsoleColor.Cyan;
        Sprite = ['║'];
        Width = 1;
        Height = 1;
        
        // Laser travels very fast
        Velocity = direction * 100f;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        CurrentLife += deltaTime;
        if (CurrentLife >= LifeTime)
        {
            IsActive = false;
        }
        
        // Deactivate if off screen
        if (Position.Y < -1 || Position.Y > 25)
        {
            IsActive = false;
        }
    }

    public override void Render(char[,] buffer, ConsoleColor[,] colorBuffer, int bufferWidth, int bufferHeight)
    {
        if (!IsActive || !IsVisible) return;

        // Render laser beam from start position to current position
        int startY = (int)Position.Y;
        int x = (int)Position.X;
        
        for (int y = startY; y >= 0 && y < bufferHeight; y--)
        {
            if (x >= 0 && x < bufferWidth)
            {
                buffer[y, x] = '║';
                colorBuffer[y, x] = Color;
            }
        }
    }
}

public class Missile : GameEntity
{
    public int Damage { get; }
    public Vector2 Target { get; set; }
    public float Speed { get; set; } = 25f;
    public float TrackingStrength { get; set; } = 0.1f;
    
    public Missile(Vector2 position, Vector2 target, int damage = 3) 
        : base(position, GameEntityType.Missile)
    {
        Target = target;
        Damage = damage;
        Color = ConsoleColor.Yellow;
        Sprite = ['◆'];
        Width = 1;
        Height = 1;
    }

    public override void Update(float deltaTime)
    {
        // Home in on target
        Vector2 direction = (Target - Position).Normalized;
        Velocity = Velocity + direction * TrackingStrength;
        Velocity = Velocity.Normalized * Speed;
        
        base.Update(deltaTime);
        
        // Deactivate if off screen
        if (Position.Y < -1 || Position.Y > 25 || Position.X < -1 || Position.X > 80)
        {
            IsActive = false;
        }
    }

    public void UpdateTarget(Vector2 newTarget)
    {
        Target = newTarget;
    }
}
