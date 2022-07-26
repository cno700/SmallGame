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
        //base.OnInspectorGUI();
        /*
         * 使用基于InspectorGUI的方法会导致一直刷新地图，当地图复杂到一定程度时会带来很大负担，
         * 所以改为使用绘制默认检查器（DrawDefaultInspector）。
         * 该方法会返回一个bool值，只有当Inspector中的值更新时才为true。
         */
        
        MapGenerator map = target as MapGenerator; // target即是对应脚本的实例
        
        if (DrawDefaultInspector())
        {
            //Debug.Log("It is " + Time.time);
            map.GenerateMap();
        }

        // 老师原意：如果脚本里发生了改变，我们也希望能够手动更新地图
        // （所以意思是改变脚本并不会对Inspector面板产生影响进而不会刷新地图？）
        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}
