using AstroDefenderPro.AI.Core;
using AstroDefenderPro.GameEngine.Core;

namespace AstroDefenderPro.AI.Strategies;

public class AdvancedAIStrategy
{
    private readonly GameStateAnalyzer _analyzer;
    private readonly Random _random;
    private AIDecision? _lastDecision;
    private float _lastDecisionTime;
    private Vector2 _lastPlayerPosition;

    public AdvancedAIStrategy()
    {
        _analyzer = new GameStateAnalyzer();
        _random = new Random();
    }

    public AIDecision MakeDecision(GameAnalysis analysis, float deltaTime)
    {
        _lastDecisionTime += deltaTime;
        
        var decision = new AIDecision
        {
            Confidence = 0.5f,
            Reasoning = "Analyzing situation...",
            Considerations = []
        };

        // Emergency responses (highest priority)
        if (TryEmergencyResponse(analysis, decision))
            return FinalizeDecision(decision, analysis);

        // Tactical decisions (medium priority)
        if (TryTacticalMove(analysis, decision))
            return FinalizeDecision(decision, analysis);

        // Strategic decisions (lowest priority)
        if (TryStrategicMove(analysis, decision))
            return FinalizeDecision(decision, analysis);

        // Default action
        decision.Action = AIAction.None;
        decision.Reasoning = "Maintaining position";
        decision.Confidence = 0.3f;

        return FinalizeDecision(decision, analysis);
    }

    private bool TryEmergencyResponse(GameAnalysis analysis, AIDecision decision)
    {
        // Dodge incoming bullets
        var immediateThreats = analysis.EnemyBullets.Where(b => b.TimeToReachPlayer < 1.5f).ToList();
        if (immediateThreats.Any())
        {
            var threat = immediateThreats.OrderBy(t => t.TimeToReachPlayer).First();
            decision.Action = threat.Position.X < analysis.PlayerPosition.X ? AIAction.MoveRight : AIAction.MoveLeft;
            decision.Confidence = 0.9f;
            decision.Reasoning = $"Dodging bullet! Time to hit: {threat.TimeToReachPlayer:F1}s";
            decision.Considerations.Add("EMERGENCY: Incoming projectile");
            return true;
        }

        // Activate shield if available and under heavy fire
        if (!analysis.HasShield && analysis.ThreatLevel > 4f && analysis.PlayerHealth <= 1)
        {
            decision.Action = AIAction.ActivateShield;
            decision.Confidence = 0.8f;
            decision.Reasoning = "Activating emergency shield - low health and high threat";
            decision.Considerations.Add("EMERGENCY: Low health, activating shield");
            return true;
        }

        return false;
    }

    private bool TryTacticalMove(GameAnalysis analysis, AIDecision decision)
    {
        // Power-up collection strategy
        if (analysis.BestPowerUp.HasValue && analysis.ThreatLevel < 2f)
        {
            var powerUp = analysis.PowerUps.OrderByDescending(p => p.Priority).First();
            if (powerUp.DistanceToPlayer < 10f && powerUp.Priority > 6f)
            {
                decision.Action = powerUp.Position.X > analysis.PlayerPosition.X ? AIAction.MoveRight : AIAction.MoveLeft;
                decision.Confidence = 0.7f;
                decision.Reasoning = $"Moving to collect {powerUp.Type} power-up (Priority: {powerUp.Priority:F1})";
                decision.Considerations.Add($"Collecting high-priority power-up: {powerUp.Type}");
                return true;
            }
        }

        // Optimal positioning for shooting
        if (ShouldShoot(analysis))
        {
            decision.Action = AIAction.Shoot;
            decision.Confidence = 0.8f;
            decision.Reasoning = "Engaging targets";
            decision.Considerations.Add("Good shooting opportunity");
            return true;
        }

        // Move to optimal position
        float positionDiff = analysis.OptimalPosition - analysis.PlayerPosition.X;
        if (Math.Abs(positionDiff) > 2f)
        {
            decision.Action = positionDiff > 0 ? AIAction.MoveRight : AIAction.MoveLeft;
            decision.Confidence = 0.6f;
            decision.Reasoning = $"Moving to optimal position (Target: {analysis.OptimalPosition:F1})";
            decision.Considerations.Add($"Repositioning for better tactical advantage");
            return true;
        }

        return false;
    }

    private bool TryStrategicMove(GameAnalysis analysis, AIDecision decision)
    {
        // Use special weapons against tough enemies
        if (analysis.CurrentWeapon != WeaponType.Basic)
        {
            var strongEnemies = analysis.Enemies.Where(e => 
                e.Type == GameEntityType.StrongEnemy || e.Type == GameEntityType.BossEnemy).ToList();
            
            if (strongEnemies.Any() && IsWellPositioned(analysis, strongEnemies))
            {
                decision.Action = AIAction.SpecialWeapon;
                decision.Confidence = 0.7f;
                decision.Reasoning = $"Using {analysis.CurrentWeapon} against strong enemies";
                decision.Considerations.Add($"Special weapon deployment: {analysis.CurrentWeapon}");
                return true;
            }
        }

        // Maintain safe distance from boss enemies
        var bossEnemies = analysis.Enemies.Where(e => e.Type == GameEntityType.BossEnemy).ToList();
        if (bossEnemies.Any())
        {
            var boss = bossEnemies.First();
            if (boss.DistanceToPlayer < 8f)
            {
                // Move away from boss
                decision.Action = boss.Position.X > analysis.PlayerPosition.X ? AIAction.MoveLeft : AIAction.MoveRight;
                decision.Confidence = 0.6f;
                decision.Reasoning = "Maintaining safe distance from boss";
                decision.Considerations.Add("Boss encounter: maintaining distance");
                return true;
            }
        }

        // Aggressive positioning when shields are active
        if (analysis.HasShield && analysis.ShieldTimeRemaining > 2f)
        {
            var nearestEnemy = analysis.Enemies.OrderBy(e => e.DistanceToPlayer).FirstOrDefault();
            if (nearestEnemy != null && nearestEnemy.DistanceToPlayer > 5f)
            {
                decision.Action = nearestEnemy.Position.X > analysis.PlayerPosition.X ? AIAction.MoveRight : AIAction.MoveLeft;
                decision.Confidence = 0.5f;
                decision.Reasoning = "Aggressive positioning while shielded";
                decision.Considerations.Add($"Shield active ({analysis.ShieldTimeRemaining:F1}s) - being aggressive");
                return true;
            }
        }

        return false;
    }

    private bool ShouldShoot(GameAnalysis analysis)
    {
        // Don't shoot if no enemies
        if (!analysis.Enemies.Any()) return false;

        // Always shoot if enemies are close
        if (analysis.Enemies.Any(e => e.DistanceToPlayer < 8f)) return true;

        // Shoot at aligned enemies
        var alignedEnemies = analysis.Enemies.Where(e => 
            Math.Abs(e.Position.X - analysis.PlayerPosition.X) < 3f && 
            e.Position.Y < analysis.PlayerPosition.Y).ToList();

        if (alignedEnemies.Any()) return true;

        // Shoot at high-value targets
        var highValueTargets = analysis.Enemies.Where(e => 
            e.Points >= 50 && e.DistanceToPlayer < 15f).ToList();

        return highValueTargets.Any();
    }

    private bool IsWellPositioned(GameAnalysis analysis, List<EnemyInfo> targets)
    {
        if (!targets.Any()) return false;

        // Check if we have a clear shot at most targets
        int alignedTargets = targets.Count(t => Math.Abs(t.Position.X - analysis.PlayerPosition.X) < 4f);
        return alignedTargets >= targets.Count / 2;
    }

    private AIDecision FinalizeDecision(AIDecision decision, GameAnalysis analysis)
    {
        // Add context to reasoning
        decision.Considerations.Add($"Threat Level: {analysis.ThreatLevel:F1}");
        decision.Considerations.Add($"Health: {analysis.PlayerHealth}/{3}");
        decision.Considerations.Add($"Enemies: {analysis.Enemies.Count}");
        
        if (analysis.HasShield)
        {
            decision.Considerations.Add($"Shield: {analysis.ShieldTimeRemaining:F1}s");
        }

        // Calculate expected reward
        decision.ExpectedReward = CalculateExpectedReward(decision, analysis);

        _lastDecision = decision;
        _lastPlayerPosition = analysis.PlayerPosition;
        _lastDecisionTime = 0;

        return decision;
    }

    private float CalculateExpectedReward(AIDecision decision, GameAnalysis analysis)
    {
        float reward = 0f;

        switch (decision.Action)
        {
            case AIAction.Shoot:
                // Reward based on likely hits
                var targets = analysis.Enemies.Where(e => Math.Abs(e.Position.X - analysis.PlayerPosition.X) < 3f);
                reward = targets.Sum(t => t.Points * 0.1f);
                break;

            case AIAction.MoveLeft:
            case AIAction.MoveRight:
                // Reward for avoiding threats and approaching power-ups
                reward = -analysis.ThreatLevel * 0.5f;
                if (analysis.BestPowerUp.HasValue)
                {
                    var powerUp = analysis.PowerUps.OrderByDescending(p => p.Priority).First();
                    reward += powerUp.Priority * 0.3f;
                }
                break;

            case AIAction.ActivateShield:
                // High reward for using shield when in danger
                reward = analysis.ThreatLevel * 2f;
                break;

            case AIAction.SpecialWeapon:
                // Reward based on special weapon effectiveness
                var strongTargets = analysis.Enemies.Where(e => e.ThreatLevel > 2f);
                reward = strongTargets.Sum(t => t.Points * 0.15f);
                break;
        }

        return reward * decision.Confidence;
    }
}
