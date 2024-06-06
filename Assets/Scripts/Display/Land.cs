using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责对每个关卡做特殊处理
public class Land
{
    public Transform GRID_ROOT;
    public Transform ENTITY_ROOT;


    public Land()
    {
        GRID_ROOT   = GameObject.Find("Field/Grids").transform;
        ENTITY_ROOT = GameObject.Find("Field/Entitys").transform;
    }
}
