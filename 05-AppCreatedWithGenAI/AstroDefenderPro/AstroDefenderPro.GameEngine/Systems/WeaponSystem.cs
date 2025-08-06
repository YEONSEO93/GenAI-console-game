using AstroDefenderPro.GameEngine.Core;
using AstroDefenderPro.GameEngine.Entities;

namespace AstroDefenderPro.GameEngine.Systems;

public class WeaponSystem
{
    private readonly GameWorld _gameWorld;
    
    public WeaponSystem(GameWorld gameWorld)
    {
        _gameWorld = gameWorld;
    }

    public void FirePlayerWeapon(Player player)
    {
        if (!player.CanShoot()) return;

        var playerPos = player.Position;
        
        switch (player.CurrentWeapon)
        {
            case WeaponType.Basic:
                FireBasicBullet(playerPos, player);
                break;
                
            case WeaponType.Double:
                FireDoubleBullet(playerPos, player);
                break;
                
            case WeaponType.Triple:
                FireTripleBullet(playerPos, player);
                break;
                
            case WeaponType.Laser:
                FireLaser(playerPos);
                break;
                
            case WeaponType.Missile:
                FireMissile(playerPos);
                break;
                
            case WeaponType.Spread:
                FireSpreadBullets(playerPos, player);
                break;
        }
        
        player.OnBulletFired();
    }

    private void FireBasicBullet(Vector2 position, Player player)
    {
        var velocity = new Vector2(0, -50f);
        _gameWorld.PlayerBullets.Add(new Bullet(position, velocity, true, 1));
    }

    private void FireDoubleBullet(Vector2 position, Player player)
    {
        var leftPos = new Vector2(position.X - 0.5f, position.Y);
        var rightPos = new Vector2(position.X + 0.5f, position.Y);
        var velocity = new Vector2(0, -50f);
        
        _gameWorld.PlayerBullets.Add(new Bullet(leftPos, velocity, true, 1));
        _gameWorld.PlayerBullets.Add(new Bullet(rightPos, velocity, true, 1));
    }

    private void FireTripleBullet(Vector2 position, Player player)
    {
        var centerPos = position;
        var leftPos = new Vector2(position.X - 1f, position.Y);
        var rightPos = new Vector2(position.X + 1f, position.Y);
        var velocity = new Vector2(0, -50f);
        
        _gameWorld.PlayerBullets.Add(new Bullet(centerPos, velocity, true, 1));
        _gameWorld.PlayerBullets.Add(new Bullet(leftPos, velocity, true, 1));
        _gameWorld.PlayerBullets.Add(new Bullet(rightPos, velocity, true, 1));
    }

    private void FireLaser(Vector2 position)
    {
        var direction = Vector2.Up;
        _gameWorld.Lasers.Add(new Laser(position, direction, 3));
    }

    private void FireMissile(Vector2 position)
    {
        // Find nearest enemy for targeting
        var nearestEnemy = _gameWorld.Enemies
            .Where(e => e.IsActive)
            .OrderBy(e => position.Distance(e.Position))
            .FirstOrDefault();
            
        if (nearestEnemy != null)
        {
            _gameWorld.Missiles.Add(new Missile(position, nearestEnemy.Position, 4));
        }
    }

    private void FireSpreadBullets(Vector2 position, Player player)
    {
        for (int i = -2; i <= 2; i++)
        {
            var angle = i * 0.3f; // Spread angle
            var velocity = new Vector2(
                (float)(Math.Sin(angle) * 45f),
                -45f
            );
            _gameWorld.PlayerBullets.Add(new Bullet(position, velocity, true, 1));
        }
    }

    public void FireEnemyWeapon(Enemy enemy)
    {
        if (!enemy.CanShoot || !enemy.ShouldShoot(_gameWorld.Player.Position)) return;

        var position = new Vector2(enemy.Position.X, enemy.Position.Y + 1);
        var velocity = new Vector2(0, 30f); // Bullets go down
        
        _gameWorld.EnemyBullets.Add(new Bullet(position, velocity, false, 1));
        enemy.OnShoot();
    }

    public void UpdateMissileTargets()
    {
        foreach (var missile in _gameWorld.Missiles.Where(m => m.IsActive))
        {
            // Update missile target to nearest enemy
            var nearestEnemy = _gameWorld.Enemies
                .Where(e => e.IsActive)
                .OrderBy(e => missile.Position.Distance(e.Position))
                .FirstOrDefault();
                
            if (nearestEnemy != null)
            {
                missile.UpdateTarget(nearestEnemy.Position);
            }
        }
    }
}
