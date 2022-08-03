using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    
    void Update()
    {
        // 移动输入
        //vector3 moveinput = new vector3(input.getaxis("horizontal"), 0, input.getaxis("vertical"));
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        // GetAxis和GetAxisRaw的区别在于前者拥有平滑过渡功能（移动会往前滑行一点）

        Vector3 moveVelocity = moveInput.normalized * moveSpeed; // 归一化以获得方向，再乘以速度
        controller.Move(moveVelocity);


        // 指向输入
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition); // 获取相机指向鼠标位置的射线
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // 创建平面，第一个参数为平面的法向量，第二个参数为平面起始点
        float rayDistance; // Line35使用out关键字可以将未初始化的变量传递给方法（输出型参数传递、引用传递）

        // 如果ray与地平面相交，rayDistance为相机到交点的距离
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.blue); // 在Scene中显示指向交点的红线
            controller.LookAt(point);
        }


        // 武器输入
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
    }
}
