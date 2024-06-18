
using System;
using UnityEngine;

public static class _C
{
    public static int DEFAULT_FRAME = 60;   //默认帧数

    public static string CLOUD_ENV = "cbcloud-6ght5vboe6f0f8c0";    //云开发环境ID


    public static string JSON_PATH = Application.dataPath + "/Resources/Json";

    public static float DEFAULT_GRID_WEIGHT = 1.25f;
    public static float DEFAULT_GRID_HEIGHT = 1.26f;

    
    public static int BOMB_UNLOCK_LEVEL     = 11;       //开放合成炸弹
    public static int ENDLESS_UNLOCK_LEVEL  = 10;

    public static int DEFAULT_FOOD      = 30;       //体力上限
    public static int FOOD_RECOVERYTIME = 300;      //体力恢复时间 300秒


 
    //状态机的状态列表
    public enum FSMSTATE
    {
        IDLE,       //待机
        ELIMINATE,  //消除阶段
        CHAIN,      //连锁反应阶段,
        CHECK,
        RESULT      //
    }

    //方向
    public enum DIRECTION
    {
        NONE = 99,

        UP = 0,
        DOWN,
        LEFT,
        RIGHT,
    }

    //方块类型
    public enum CARD_TYPE
    {
        JELLY   = 1,
        SPECIAL = 2,
        FRAME   = 3,
    }

    //方块状态
    public enum CARD_STATE
    {
        NORMAL,
        GHOST    
    }

    //方块ID
    public enum CARD
    {
        UNIVERSAL   = 10000,
        MISSILE     = 10010,
        BOMB        = 10011,

        STONE       = 10020,
        WOOD        = 10021,
        PORTAL      = 10022,    //传送门
    }

    //消解方式
    public enum DEAD_TYPE
    {
        NORMAL,     //变高然后消解
        DIGESTE,    //直接消解
        BOMB,       //爆炸
    }

    //游戏状态
    public enum GAME_STATE
    {
        NONE,
        PLAY,
        PAUSE,
        END
    }

    public enum RESULT
    {
        VICTORY,
        LOSE,
        NONE
    }

    //动画节点的状态
    public enum DISPLAY_STATE
    {
        IDLE,
        PLAYING,
        END
    }

    //关卡类型
    public enum MODE
    {
        CHAPTER = 0,
        ENDLESS
    }







    public class VIBRATELEVEL
    {
        public static string HEAVY  = "heavy";
        public static string MEDIUM = "medium";
        public static string LIGHT  = "light";

    }
}
