using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode {Auto, Burst, Single}; // 开火模式（全自动、连发、单点）
    public FireMode fireMode;

    //public Transform muzzle; // 枪口位置
    public Transform[] projectileSpawn; // 枪口位置（像霰弹枪那样散射）
    public Projectile projectile; // 子弹类型
    public float msBetweenShots = 100; // 射速/射击间隔
    public float muzzleVelocity = 35; // 枪口速度
    public int burstCount = 3; // 一次连发有多少颗子弹

    public Transform shell;
    public Transform shellEjection;
    MuzzleFlash muzzleflash;
     
    float nextShotTime;

    bool triggerReleasedSinceLastShot; // 当鼠标左键按下时该值为false，抬起时为true，为true时才能发射子弹
    int shotsRemainingInBurst;


    void Start()
    {
        muzzleflash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
    }

    void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            if (fireMode == FireMode.Burst)
            {
                 if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            // 多个枪口
            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation);
                newProjectile.SetSpeed(muzzleVelocity);

                
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount; // 抬起时重置下一次发射连发子弹数
    }
}
