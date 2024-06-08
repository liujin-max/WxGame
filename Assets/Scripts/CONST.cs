
using System;
using UnityEngine;

public static class _C
{
    public static int DEFAULT_FRAME = 60;   //默认帧数

    public static string CLOUD_ENV = "cbcloud-6ght5vboe6f0f8c0";    //云开发环境ID


    public static string JSON_PATH = Application.dataPath + "/Resources/Json";

    public static float DEFAULT_GRID_WEIGHT = 1.25f;
    public static float DEFAULT_GRID_HEIGHT = 1.26f;


 
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
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    //方块类型
    public enum CARD_TYPE
    {
        JELLY = 1,
        FRAME    
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
        STONE   = 10020,
        WOOD    = 10021,
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








    public class VIBRATELEVEL
    {
        public static string HEAVY  = "heavy";
        public static string MEDIUM = "medium";
        public static string LIGHT  = "light";

    }
}
