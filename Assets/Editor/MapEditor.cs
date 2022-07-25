using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// CustomEditor特性：允许我们自定义组建的Inspector面板，这里指明是MapGenerator类的面板
[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator; // target即是对应脚本的实例
        //Debug.Log("It is " + Time.time);
        map.GenerateMap();
    }
}
