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
        // �ƶ�����
        //vector3 moveinput = new vector3(input.getaxis("horizontal"), 0, input.getaxis("vertical"));
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        // GetAxis��GetAxisRaw����������ǰ��ӵ��ƽ�����ɹ��ܣ��ƶ�����ǰ����һ�㣩

        Vector3 moveVelocity = moveInput.normalized * moveSpeed; // ��һ���Ի�÷����ٳ����ٶ�
        controller.Move(moveVelocity);


        // ָ������
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition); // ��ȡ���ָ�����λ�õ�����
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // ����ƽ�棬��һ������Ϊƽ��ķ��������ڶ�������Ϊƽ����ʼ��
        float rayDistance; // Line35ʹ��out�ؼ��ֿ��Խ�δ��ʼ���ı������ݸ�����������Ͳ������ݡ����ô��ݣ�

        // ���ray���ƽ���ཻ��rayDistanceΪ���������ľ���
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.blue); // ��Scene����ʾָ�򽻵�ĺ���
            controller.LookAt(point);
        }


        // ��������
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }
    }
}
