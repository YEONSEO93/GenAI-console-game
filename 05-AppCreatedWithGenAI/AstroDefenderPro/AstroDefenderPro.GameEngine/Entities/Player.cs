using AstroDefenderPro.GameEngine.Core;

namespace AstroDefenderPro.GameEngine.Entities;

public class Player : GameEntity
{
    public int Score { get; set; }
    public int Lives { get; set; } = 3;
    public WeaponType CurrentWeapon { get; set; } = WeaponType.Basic;
    public int MaxBullets { get; set; } = 3;
    public int ActiveBullets { get; set; }
    public float Speed { get; set; } = 20f;
    public float BaseSpeed { get; } = 20f;
    
    // Power-up states
    public bool HasShield { get; set; }
    public float ShieldTimeRemaining { get; set; }
    public bool HasSpeedBoost { get; set; }
    public float SpeedBoostTimeRemaining { get; set; }
    public int WeaponLevel { get; set; } = 1;
    public float LastShotTime { get; set; }
    public float ShotCooldown { get; set; } = 0.2f;

    public Player(Vector2 position) : base(position, GameEntityType.Player)
    {
        Health = 3;
        MaxHealth = 3;
        Color = ConsoleColor.Green;
        Sprite = ['▲'];
        Width = 1;
        Height = 1;
        Score = 0;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Update power-up timers
        if (HasShield)
        {
            ShieldTimeRemaining -= deltaTime;
            if (ShieldTimeRemaining <= 0)
            {
                HasShield = false;
                Color = ConsoleColor.Green; // Reset color
            }
        }

        if (HasSpeedBoost)
        {
            SpeedBoostTimeRemaining -= deltaTime;
            if (SpeedBoostTimeRemaining <= 0)
            {
                HasSpeedBoost = false;
                Speed = BaseSpeed;
            }
        }

        LastShotTime += deltaTime;
    }

    public void MoveLeft(float deltaTime, int minX = 0)
    {
        float newX = Position.X - Speed * deltaTime;
        if (newX >= minX)
        {
            Position = new Vector2(newX, Position.Y);
        }
    }

    public void MoveRight(float deltaTime, int maxX)
    {
        float newX = Position.X + Speed * deltaTime;
        if (newX <= maxX - Width)
        {
            Position = new Vector2(newX, Position.Y);
        }
    }

    public bool CanShoot()
    {
        return ActiveBullets < MaxBullets && LastShotTime >= ShotCooldown;
    }

    public void OnBulletFired()
    {
        ActiveBullets++;
        LastShotTime = 0;
    }

    public void OnBulletDestroyed()
    {
        ActiveBullets = Math.Max(0, ActiveBullets - 1);
    }

    public void ActivateShield(float duration = 5f)
    {
        HasShield = true;
        ShieldTimeRemaining = duration;
        Color = ConsoleColor.Cyan;
    }

    public void ActivateSpeedBoost(float duration = 10f, float speedMultiplier = 1.5f)
    {
        HasSpeedBoost = true;
        SpeedBoostTimeRemaining = duration;
        Speed = BaseSpeed * speedMultiplier;
    }

    public void UpgradeWeapon()
    {
        WeaponLevel = Math.Min(WeaponLevel + 1, 3);
        CurrentWeapon = WeaponLevel switch
        {
            2 => WeaponType.Double,
            3 => WeaponType.Triple,
            _ => WeaponType.Basic
        };
        MaxBullets = WeaponLevel * 2 + 1;
    }

    public void Heal(int amount = 1)
    {
        Health = Math.Min(Health + amount, MaxHealth);
    }

    public override void TakeDamage(int damage)
    {
        if (HasShield) return; // Shield protects from damage
        
        base.TakeDamage(damage);
        if (Health <= 0)
        {
            Lives--;
            if (Lives > 0)
            {
                Health = MaxHealth; // Respawn with full health
                // Reset position to center bottom
                Position = new Vector2(40, 18);
                // Reset power-ups
                HasShield = false;
                HasSpeedBoost = false;
                Speed = BaseSpeed;
                Color = ConsoleColor.Green;
            }
        }
    }

    public override void Render(char[,] buffer, ConsoleColor[,] colorBuffer, int bufferWidth, int bufferHeight)
    {
        base.Render(buffer, colorBuffer, bufferWidth, bufferHeight);
        
        // Render shield effect
        if (HasShield)
        {
            RenderShieldEffect(buffer, colorBuffer, bufferWidth, bufferHeight);
        }
    }

    private void RenderShieldEffect(char[,] buffer, ConsoleColor[,] colorBuffer, int bufferWidth, int bufferHeight)
    {
        int x = (int)Position.X;
        int y = (int)Position.Y;
        
        // Draw shield around player
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue; // Skip player position
                
                int shieldX = x + dx;
                int shieldY = y + dy;
                
                if (shieldX >= 0 && shieldX < bufferWidth && shieldY >= 0 && shieldY < bufferHeight)
                {
                    if (buffer[shieldY, shieldX] == ' ')
                    {
                        buffer[shieldY, shieldX] = '○';
                        colorBuffer[shieldY, shieldX] = ConsoleColor.Cyan;
                    }
                }
            }
        }
    }
}
