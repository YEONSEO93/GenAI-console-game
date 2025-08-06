using AstroDefenderPro.GameEngine.Core;

namespace AstroDefenderPro.GameEngine.Entities;

public abstract class GameEntity
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVisible { get; set; } = true;
    public int Health { get; set; } = 1;
    public int MaxHealth { get; protected set; } = 1;
    public ConsoleColor Color { get; set; } = ConsoleColor.White;
    public char[] Sprite { get; set; } = [];
    public int Width { get; protected set; } = 1;
    public int Height { get; protected set; } = 1;
    public GameEntityType EntityType { get; protected set; }
    public float LastUpdateTime { get; set; }

    protected GameEntity(Vector2 position, GameEntityType entityType)
    {
        Position = position;
        EntityType = entityType;
        LastUpdateTime = Environment.TickCount / 1000f;
    }

    public virtual void Update(float deltaTime)
    {
        if (!IsActive) return;
        
        Position += Velocity * deltaTime;
        LastUpdateTime += deltaTime;
    }

    public virtual void Render(char[,] buffer, ConsoleColor[,] colorBuffer, int bufferWidth, int bufferHeight)
    {
        if (!IsActive || !IsVisible) return;

        int x = (int)Position.X;
        int y = (int)Position.Y;

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
                        colorBuffer[drawY, drawX] = Color;
                    }
                }
            }
        }
    }

    public Rectangle GetBounds()
    {
        return new Rectangle(Position.X, Position.Y, Width, Height);
    }

    public bool CollidesWith(GameEntity other)
    {
        if (!IsActive || !other.IsActive) return false;
        return GetBounds().Intersects(other.GetBounds());
    }

    public virtual void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
        if (Health <= 0)
        {
            OnDestroyed();
        }
    }

    public virtual void OnDestroyed()
    {
        IsActive = false;
    }

    public bool IsAlive => IsActive && Health > 0;
}
