using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficult : MonoBehaviour
{
    // public AnimationCurve difficultyCurve;
    // private int maxStage = 100; // 最大阶段数
    // private int maxDifficulty = 22000; // 最大难度值




    public AnimationCurve difficultyCurve2;
    private int maxStage2 = 1000; // 最大阶段数
    private int maxDifficulty2 = 500000; // 最大难度值

    // 根据阶段（Stage）获取难度值
    public int GetDifficultyAtStage(int stage)
    {
        float normalizedTime = Mathf.Clamp01((float)stage / maxStage2);
        float difficultyValue = difficultyCurve2.Evaluate(normalizedTime) * maxDifficulty2;
        return (int)difficultyValue;


        // float normalizedTime = Mathf.Clamp01((float)stage / maxStage);
        // float difficultyValue = difficultyCurve.Evaluate(normalizedTime) * maxDifficulty;
        // return (int)difficultyValue;
    }
}
