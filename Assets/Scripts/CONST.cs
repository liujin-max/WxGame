
using System;
using UnityEngine;

public static class _C
{


    public enum LAYER
    {
        DEFAULT     = 0,
        BALLIDLE    = 6,    //待机
        BALLACTING  = 7,    //运动中
        BALLRECYCLE = 8,     //回收

        OBTINVALID = 9,    //宝石无效

        BALLREADY   = 13,    //代发射
    }

    //状态机的状态列表
    public enum FSMSTATE
    {
        GAME_START      = 0,
        GAME_IDLE       = 1,
        GAME_PLAY       = 2,
        GAME_COMPLEX    = 3,
        GAME_SHOP       = 4,
        GAME_END        = 5
    }

    //
    public enum RESULT
    {
        LOSE = 0,
        WIN = 1,
        NULL = 2
    }

  
    public static int GLASSPRICE   = 2;    //碎片价格
    public static int DEFAULT_COIN = 5;
    public static int DEFAULT_GLASS= 5;



    public static Vector2 DEFAULT_GRAVITY = new Vector2(0, -9.81f);


    public static Vector3 VEC3INVALID       = new Vector3(-999, -999, -999);
    public static Vector3 BALL_ORIGIN_POS   = new Vector3(0, 6.9f, 0);  //待机位置
    public static Vector3 BALL_SHOOT_POS    = new Vector3(0, 6.6f, 0);  //发射位置



    public static string REDCOLOR   = "<#FF6631>";
    public static string GREENCOLOR = "<#45FF31>";


    public static float CANCELHEIGHT    = 5.6f;

    //边界
    public static float BOARD_LEFT  = -4.0f;
    public static float BOARD_RIGHT  = 4.0f;
    public static float BOARD_TOP  = 4f;
    public static float BOARD_BOTTOM  = -8f;

    public static float OBSTACLE_ORIGIN_Y   = -7;
    public static float OBSTACLE_OFFSET     = 1.5f;



    public static string[] ObstaclePrefabs = {
        "Prefab/Obstacle/Obstacle_Fang",
        "Prefab/Obstacle/Obstacle_SanJiao",
        "Prefab/Obstacle/Obstacle_Wubian",
        "Prefab/Obstacle/Obstacle_Yuan",
    };


    //球类型,同时也是Ghost类型
    public enum BALLTYPE
    {
        NORMAL  = 0,
        BOOM    = 1,
        STRONG  = 2,
        SPLIT   = 3,
        SMALL   = 4,
        THROUGH = 5,
        RISE    = 6,
        REBOUND = 7,    //反弹
        CRAZY   = 8,
        PENG    = 9,
        MANLI   = 10,   //蛮力
        HALO    = 11,   //光环
        SANJIAO = 12,
        FANG    = 13,
        YUAN    = 14,
        LING    = 15,
        REGEN   = 16,       //再生
        GROWTH  = 17,
        EXPAND  = 18,
        FOCUS   = 19,

        SCALE   = 21,       //缩放弹珠
        SPEED   = 22,
        REFLEX  = 23,
        BLACKHOLE = 24,     //黑洞弹珠
        MASS    = 25,
    }   



    public enum COMPLEXTEVEMT
    {
        NEW     = 0,
        UPGRADE = 1,
        GLASS   = 2
    }
}
