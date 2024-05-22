
using System;
using UnityEngine;

public static class _C
{
    public static int DEFAULT_FRAME = 60;   //默认帧数

    public static string CLOUD_ENV = "cbcloud-6ght5vboe6f0f8c0";    //云开发环境ID





 
    //状态机的状态列表
    public enum FSMSTATE
    {
        GAME_START      = 0,
        GAME_RECORD     = 1,
        GAME_IDLE       = 2,
        GAME_PLAY       = 3,
        GAME_COMPLEX    = 4,
        GAME_SHOP       = 5,
        GAME_END        = 6
    }

    public enum ANIMAL
    {
        CAT     = 1,
        DOG     = 2,
        MOUSE   = 3 
    }

    public enum SIDE
    {
        OUR = 1,
        ENEMY,
        NEUTRAL
    }




    public static int DEFAULT_WEIGHT  = 5;
    public static int DEFAULT_HEIGHT = 5;









    public class VIBRATELEVEL
    {
        public static string HEAVY  = "heavy";
        public static string MEDIUM = "medium";
        public static string LIGHT  = "light";

    }
}
