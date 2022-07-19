using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle; // 枪口位置
    public Projectile projectile; // 子弹类型
    public float msBetweenShots = 100; // 射速/射击间隔
    public float muzzleVelocity = 35; // 枪口速度

    float nextShotTime;


    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            newProjectile.SetSpeed(muzzleVelocity);
        }
    }
}
