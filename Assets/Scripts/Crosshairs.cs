using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHighlightColour;
    Color originalDotColour;

    private void Start()
    {
        //Cursor.visible = false; // 隐藏鼠标
        originalDotColour = dot.color;
    }

    void Update()
    {
        // “更新准心的位置为鼠标位置”和“瞄准敌人时准心变色”两个功能分别在Player里“player转向鼠标位置”处实现和调用

        //transform.Rotate(Vector3.up * 40 * Time.deltaTime, Space.World); // 让准心保持旋转，两种写法都可
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColour;

        }
        else
        {
            dot.color = originalDotColour;
        }

    }
}
