namespace AstroDefenderPro.GameEngine.Core;

public struct Vector2
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static Vector2 Zero => new(0, 0);
    public static Vector2 Up => new(0, -1);
    public static Vector2 Down => new(0, 1);
    public static Vector2 Left => new(-1, 0);
    public static Vector2 Right => new(1, 0);

    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2 operator *(Vector2 a, float scalar) => new(a.X * scalar, a.Y * scalar);
    public static Vector2 operator /(Vector2 a, float scalar) => new(a.X / scalar, a.Y / scalar);

    public float Length => (float)Math.Sqrt(X * X + Y * Y);
    public Vector2 Normalized => Length > 0 ? this / Length : Zero;

    public float Distance(Vector2 other) => (this - other).Length;
}

public struct Rectangle
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public Rectangle(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public float Left => X;
    public float Right => X + Width;
    public float Top => Y;
    public float Bottom => Y + Height;
    public Vector2 Center => new(X + Width / 2, Y + Height / 2);

    public bool Intersects(Rectangle other)
    {
        return Left < other.Right && Right > other.Left && 
               Top < other.Bottom && Bottom > other.Top;
    }

    public bool Contains(Vector2 point)
    {
        return point.X >= Left && point.X <= Right && 
               point.Y >= Top && point.Y <= Bottom;
    }
}
