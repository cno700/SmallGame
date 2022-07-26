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
        //base.OnInspectorGUI();
        /*
         * ʹ�û���InspectorGUI�ķ����ᵼ��һֱˢ�µ�ͼ������ͼ���ӵ�һ���̶�ʱ������ܴ󸺵���
         * ���Ը�Ϊʹ�û���Ĭ�ϼ������DrawDefaultInspector����
         * �÷����᷵��һ��boolֵ��ֻ�е�Inspector�е�ֵ����ʱ��Ϊtrue��
         */
        
        MapGenerator map = target as MapGenerator; // target���Ƕ�Ӧ�ű���ʵ��
        
        if (DrawDefaultInspector())
        {
            //Debug.Log("It is " + Time.time);
            map.GenerateMap();
        }

        // ��ʦԭ�⣺����ű��﷢���˸ı䣬����Ҳϣ���ܹ��ֶ����µ�ͼ
        // ��������˼�Ǹı�ű��������Inspector������Ӱ���������ˢ�µ�ͼ����
        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}
