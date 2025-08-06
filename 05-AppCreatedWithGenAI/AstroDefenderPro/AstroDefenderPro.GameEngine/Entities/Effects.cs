using AstroDefenderPro.GameEngine.Core;

namespace AstroDefenderPro.GameEngine.Entities;

public class PowerUp : GameEntity
{
    public PowerUpType Type { get; }
    public float LifeTime { get; set; } = 10f;
    public float CurrentLife { get; set; }
    public float BlinkTimer { get; set; }
    public bool IsBlinking { get; set; }
    
    public PowerUp(Vector2 position, PowerUpType type) 
        : base(position, GameEntityType.PowerUp)
    {
        Type = type;
        Velocity = new Vector2(0, 15f); // Fall down slowly
        Width = 1;
        Height = 1;
        
        // Set sprite and color based on type
        switch (type)
        {
            case PowerUpType.DoubleShot:
                Sprite = ['D'];
                Color = ConsoleColor.Blue;
                break;
            case PowerUpType.TripleShot:
                Sprite = ['T'];
                Color = ConsoleColor.DarkBlue;
                break;
            case PowerUpType.Laser:
                Sprite = ['L'];
                Color = ConsoleColor.Cyan;
                break;
            case PowerUpType.Shield:
                Sprite = ['S'];
                Color = ConsoleColor.Gray;
                break;
            case PowerUpType.Health:
                Sprite = ['H'];
                Color = ConsoleColor.Green;
                break;
            case PowerUpType.Speed:
                Sprite = ['F'];
                Color = ConsoleColor.White;
                break;
            default:
                Sprite = ['?'];
                Color = ConsoleColor.Gray;
                break;
        }
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        CurrentLife += deltaTime;
        BlinkTimer += deltaTime;
        
        // Start blinking when lifetime is almost up
        if (CurrentLife > LifeTime * 0.7f)
        {
            IsBlinking = true;
            if (BlinkTimer > 0.2f)
            {
                IsVisible = !IsVisible;
                BlinkTimer = 0;
            }
        }
        
        // Deactivate after lifetime expires
        if (CurrentLife >= LifeTime)
        {
            IsActive = false;
        }
        
        // Deactivate if falls off screen
        if (Position.Y > 25)
        {
            IsActive = false;
        }
    }
}

public class Explosion : GameEntity
{
    public float LifeTime { get; set; } = 0.5f;
    public float CurrentLife { get; set; }
    public int AnimationFrame { get; set; }
    public char[][] AnimationFrames { get; }
    
    public Explosion(Vector2 position) 
        : base(position, GameEntityType.Explosion)
    {
        Color = ConsoleColor.Yellow;
        Width = 1;
        Height = 1;
        
        // Animation frames for explosion
        AnimationFrames = [
            ['.'],
            ['*'],
            ['#'],
            ['@'],
            [' ']
        ];
        
        Sprite = AnimationFrames[0];
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        CurrentLife += deltaTime;
        
        // Update animation frame
        float frameTime = LifeTime / AnimationFrames.Length;
        AnimationFrame = Math.Min((int)(CurrentLife / frameTime), AnimationFrames.Length - 1);
        Sprite = AnimationFrames[AnimationFrame];
        
        // Change color during explosion
        Color = AnimationFrame switch
        {
            0 => ConsoleColor.White,
            1 => ConsoleColor.Yellow,
            2 => ConsoleColor.Red,
            3 => ConsoleColor.DarkRed,
            _ => ConsoleColor.Black
        };
        
        if (CurrentLife >= LifeTime)
        {
            IsActive = false;
        }
    }
}

public class Particle : GameEntity
{
    public float LifeTime { get; set; }
    public float CurrentLife { get; set; }
    public Vector2 InitialVelocity { get; }
    
    public Particle(Vector2 position, Vector2 velocity, float lifeTime = 1f, char sprite = '*', ConsoleColor color = ConsoleColor.White) 
        : base(position, GameEntityType.Particle)
    {
        Velocity = velocity;
        InitialVelocity = velocity;
        LifeTime = lifeTime;
        Color = color;
        Sprite = [sprite];
        Width = 1;
        Height = 1;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        CurrentLife += deltaTime;
        
        // Fade velocity over time
        float fadeRatio = 1f - (CurrentLife / LifeTime);
        Velocity = InitialVelocity * fadeRatio;
        
        if (CurrentLife >= LifeTime)
        {
            IsActive = false;
        }
    }
}
