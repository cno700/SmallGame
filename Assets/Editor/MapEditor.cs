using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// CustomEditor���ԣ����������Զ����齨��Inspector��壬����ָ����MapGenerator������
[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator; // target���Ƕ�Ӧ�ű���ʵ��
        //Debug.Log("It is " + Time.time);
        map.GenerateMap();
    }
}
