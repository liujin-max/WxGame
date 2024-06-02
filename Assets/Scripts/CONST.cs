
using System;
using UnityEngine;

public static class _C
{
    public static int DEFAULT_FRAME = 60;   //默认帧数

    public static string CLOUD_ENV = "cbcloud-6ght5vboe6f0f8c0";    //云开发环境ID





 
    //状态机的状态列表
    public enum FSMSTATE
    {
        IDLE,       //待机
        ELIMINATE,  //消除阶段
        CHAIN,      //连锁反应阶段,
        RESULT      //
    }



    public static int DEFAULT_WEIGHT  = 10;
    public static int DEFAULT_HEIGHT = 10;









    public class VIBRATELEVEL
    {
        public static string HEAVY  = "heavy";
        public static string MEDIUM = "medium";
        public static string LIGHT  = "light";

    }
}
