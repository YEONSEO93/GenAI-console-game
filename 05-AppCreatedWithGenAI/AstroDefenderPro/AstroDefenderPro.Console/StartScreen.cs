namespace AstroDefenderPro.Console;

public static class StartScreen
{
    public static void Show()
    {
        System.Console.Clear();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        
        System.Console.WriteLine(@"
    ╔═══════════════════════════════════════════════════════════════════════════════╗
    ║                                                                               ║
    ║      ██████╗ ███████╗████████╗██████╗  ██████╗                               ║
    ║     ██╔══██╗██╔════╝╚══██╔══╝██╔══██╗██╔═══██╗                              ║
    ║     ██████╔╝███████╗   ██║   ██████╔╝██║   ██║                              ║
    ║     ██╔══██╗╚════██║   ██║   ██╔══██╗██║   ██║                              ║
    ║     ██║  ██║███████║   ██║   ██║  ██║╚██████╔╝                              ║
    ║     ╚═╝  ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝                               ║
    ║                                                                               ║
    ║     ██████╗ ███████╗███████╗███████╗███╗   ██╗██████╗ ███████╗██████╗        ║
    ║     ██╔══██╗██╔════╝██╔════╝██╔════╝████╗  ██║██╔══██╗██╔════╝██╔══██╗       ║
    ║     ██║  ██║█████╗  █████╗  █████╗  ██╔██╗ ██║██║  ██║█████╗  ██████╔╝       ║
    ║     ██║  ██║██╔══╝  ██╔══╝  ██╔══╝  ██║╚██╗██║██║  ██║██╔══╝  ██╔══██╗       ║
    ║     ██████╔╝███████╗██║     ███████╗██║ ╚████║██████╔╝███████╗██║  ██║       ║
    ║     ╚═════╝ ╚══════╝╚═╝     ╚══════╝╚═╝  ╚═══╝╚═════╝ ╚══════╝╚═╝  ╚═╝       ║
    ║                                                                               ║
    ║                              ██████╗ ██████╗  ██████╗                        ║
    ║                              ██╔══██╗██╔══██╗██╔═══██╗                       ║
    ║                              ██████╔╝██████╔╝██║   ██║                       ║
    ║                              ██╔═══╝ ██╔══██╗██║   ██║                       ║
    ║                              ██║     ██║  ██║╚██████╔╝                       ║
    ║                              ╚═╝     ╚═╝  ╚═╝ ╚═════╝                        ║
    ║                                                                               ║
    ╚═══════════════════════════════════════════════════════════════════════════════╝
        ");

        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine(@"
                          🚀 ADVANCED AI-POWERED SPACE COMBAT 🚀
        ");

        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine(@"
    ╔═══════════════════════════════════════════════════════════════════════════════╗
    ║                                 FEATURES                                      ║
    ║                                                                               ║
    ║  🎮 Multiple Enemy Types:  Basic, Fast, Strong, Boss Enemies                 ║
    ║  ⚡ Power-up System:      Double/Triple Shot, Laser, Shield, Health          ║
    ║  🔫 Advanced Weapons:     Basic, Laser, Missile, Spread Shot                 ║
    ║  🤖 Smart AI:            Azure OpenAI & Ollama Support                       ║
    ║  🎯 Strategic Gameplay:   Level Progression, Boss Battles                    ║
    ║  💥 Visual Effects:       Explosions, Particles, Shield Effects             ║
    ║                                                                               ║
    ╚═══════════════════════════════════════════════════════════════════════════════╝
        ");

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine(@"
    ╔═══════════════════════════════════════════════════════════════════════════════╗
    ║                               CONTROLS                                        ║
    ║                                                                               ║
    ║  ←/→ Move      SPACE Shoot      Z Special Weapon    X Shield                 ║
    ║  A   Azure AI  O     Ollama AI  F FPS Display       S Screenshot            ║
    ║  P   Pause     Q     Quit       C Slow Motion       1-4 Speed               ║
    ║                                                                               ║
    ╚═══════════════════════════════════════════════════════════════════════════════╝
        ");

        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine(@"
                         🎯 SELECT GAME SPEED 🎯
                    
                    1 - SLOW (Strategic)
                    2 - MEDIUM (Balanced) 
                    3 - FAST (Action)
                    4 - EXTREME (Chaos)
                    ENTER - Default (Medium)
        ");

        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine(@"
                    Press a number key or ENTER to start...
        ");

        System.Console.ResetColor();
    }
}
