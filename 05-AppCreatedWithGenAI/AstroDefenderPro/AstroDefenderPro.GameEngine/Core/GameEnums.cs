namespace AstroDefenderPro.GameEngine.Core;

public enum GameEntityType
{
    Player,
    BasicEnemy,
    FastEnemy,
    StrongEnemy,
    BossEnemy,
    PlayerBullet,
    EnemyBullet,
    Laser,
    Missile,
    PowerUp,
    Explosion,
    Particle
}

public enum PowerUpType
{
    DoubleShot,
    TripleShot,
    Laser,
    Shield,
    Health,
    Speed
}

public enum WeaponType
{
    Basic,
    Double,
    Triple,
    Laser,
    Missile,
    Spread
}

public enum EnemyState
{
    Idle,
    Moving,
    Attacking,
    Charging,
    Retreating
}

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver,
    Victory,
    LevelTransition
}
