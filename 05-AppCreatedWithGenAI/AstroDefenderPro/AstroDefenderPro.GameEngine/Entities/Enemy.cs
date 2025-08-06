using AstroDefenderPro.GameEngine.Core;

namespace AstroDefenderPro.GameEngine.Entities;

public class Enemy : GameEntity
{
    public EnemyState State { get; set; } = EnemyState.Moving;
    public int Points { get; }
    public float ShootCooldown { get; set; } = 2f;
    public float LastShotTime { get; set; }
    public bool CanShoot { get; set; }
    public float MovementSpeed { get; set; }
    public Vector2 OriginalPosition { get; }
    public float StateTimer { get; set; }

    protected Enemy(Vector2 position, GameEntityType enemyType, int health, int points, ConsoleColor color, char[] sprite) 
        : base(position, enemyType)
    {
        Health = health;
        MaxHealth = health;
        Points = points;
        Color = color;
        Sprite = sprite;
        Width = sprite.Length > 3 ? 3 : sprite.Length;
        Height = 1;
        OriginalPosition = position;
        MovementSpeed = 10f;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        LastShotTime += deltaTime;
        StateTimer += deltaTime;
        
        UpdateAI(deltaTime);
    }

    protected virtual void UpdateAI(float deltaTime)
    {
        // AI movement is temporarily disabled for manual control mode
        // All enemy movement and AI behavior is paused
    }

    public bool ShouldShoot(Vector2 playerPosition)
    {
        if (!CanShoot || LastShotTime < ShootCooldown) return false;
        
        // Simple shooting logic - shoot if player is somewhat aligned
        float distanceX = Math.Abs(Position.X - playerPosition.X);
        return distanceX < 5f && Position.Y < playerPosition.Y;
    }

    public void OnShoot()
    {
        LastShotTime = 0;
        State = EnemyState.Attacking;
        StateTimer = 0;
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();
        // Could trigger explosion effect here
    }
}

public class BasicEnemy : Enemy
{
    public BasicEnemy(Vector2 position) 
        : base(position, GameEntityType.BasicEnemy, 1, 10, ConsoleColor.Red, ['>', '<'])
    {
        CanShoot = true;
        ShootCooldown = 3f;
        MovementSpeed = 8f;
        Velocity = new Vector2(MovementSpeed, 0);
    }
}

public class FastEnemy : Enemy
{
    public FastEnemy(Vector2 position) 
        : base(position, GameEntityType.FastEnemy, 1, 20, ConsoleColor.Yellow, ['»', '«'])
    {
        CanShoot = true;
        ShootCooldown = 2f;
        MovementSpeed = 15f;
        Velocity = new Vector2(MovementSpeed, 0);
    }

    protected override void UpdateAI(float deltaTime)
    {
        // Fast enemies move more erratically
        switch (State)
        {
            case EnemyState.Moving:
                if (StateTimer > 1f)
                {
                    Velocity = new Vector2(-Velocity.X, Velocity.Y);
                    StateTimer = 0;
                }
                break;
        }
    }
}

public class StrongEnemy : Enemy
{
    public StrongEnemy(Vector2 position) 
        : base(position, GameEntityType.StrongEnemy, 3, 50, ConsoleColor.Magenta, ['█', '█', '█'])
    {
        CanShoot = true;
        ShootCooldown = 1.5f;
        MovementSpeed = 5f;
        Velocity = new Vector2(MovementSpeed, 0);
        Width = 3;
    }

    protected override void UpdateAI(float deltaTime)
    {
        // AI movement is temporarily disabled for manual control mode
        // All enemy movement and AI behavior is paused
    }
}

public class BossEnemy : Enemy
{
    public int Phase { get; private set; } = 1;
    public int MaxPhases { get; } = 3;
    
    public BossEnemy(Vector2 position) 
        : base(position, GameEntityType.BossEnemy, 15, 500, ConsoleColor.DarkRed, 
               ['╔', '═', '╗', '║', 'X', '║', '╚', '═', '╝'])
    {
        CanShoot = true;
        ShootCooldown = 0.8f;
        MovementSpeed = 3f;
        Velocity = new Vector2(MovementSpeed, 0);
        Width = 3;
        Height = 3;
    }

    protected override void UpdateAI(float deltaTime)
    {
        // AI movement is temporarily disabled for manual control mode
        // All enemy movement and AI behavior is paused
    }

    public override void Render(char[,] buffer, ConsoleColor[,] colorBuffer, int bufferWidth, int bufferHeight)
    {
        if (!IsActive || !IsVisible) return;

        int x = (int)Position.X;
        int y = (int)Position.Y;

        // Render boss as 3x3 sprite
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                int drawX = x + j;
                int drawY = y + i;

                if (drawX >= 0 && drawX < bufferWidth && drawY >= 0 && drawY < bufferHeight)
                {
                    int spriteIndex = i * Width + j;
                    if (spriteIndex < Sprite.Length)
                    {
                        buffer[drawY, drawX] = Sprite[spriteIndex];
                        // Color changes based on phase
                        colorBuffer[drawY, drawX] = Phase switch
                        {
                            2 => ConsoleColor.Red,
                            3 => ConsoleColor.White,
                            _ => Color
                        };
                    }
                }
            }
        }
    }
}
