// 全局变量和常量
public class Constants 
{
    public const float tileSizeWidth = 134;
    public const float tileSizeHeight = 134;

    public static bool turn = true;
    public const float startPhaseTime = 5f;
    
    public const float actionPhaseTime = 12f;   // 行动阶段持续时间
    public const float endAniShortTime = 2f;
    public const float endAniLongTime = 3f;
    public const int totalTurnNumber = 2;       // 总回合数
    
    // 玩家得分情况
    public static int p1Score = 0;
    public static int p2Score = 0;
    // public const int totalTurnNumber = 3;       // 总回合数

    // 动画时长
    public static float longAnimTime = 5f;
    public static float shortAnimTime = 4f;
    public static bool isStart = false;
    
    // 网格大小
    public const float cellSize = 100f;
}
