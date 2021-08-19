using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileGun: MonoBehaviour
{
    //bullet
    public GameObject bullet;

    //bullet force 
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletLeft, bulletsShot;

    //bool
    bool shooting, readyToShoot, reloading;

    //reference 
    public Camera fpsCam;
    public Transform attackPoint;

    //graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //bug fixing
    public bool allowInvoke = true;

    private void Awake()
    {
        bulletLeft = magazineSize;
        readyToShoot = true;
    }

    void Update()
    {
        MyInput();

        if(ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (readyToShoot && shooting && !reloading && bulletLeft > 0)
            {
                Shoot();
            }
        }
    
    }
    private void MyInput()
    {
        
        //check if u allowed hold button
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
            
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if(Input.GetKey(KeyCode.R)&&bulletLeft<magazineSize && !reloading)
        {
            Reload();
        }
        //auto reload when empty the magazine
        if (readyToShoot && shooting && !reloading && bulletLeft <= 0)
        {
            Reload();
        }

        //shooting 
        
    }
 

    private void Shoot()
    {
        
        if (muzzleFlash != null)
        {
            
            GameObject MuzzleFl =  Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity) as GameObject;
            MuzzleFl.GetComponent<ParticleSystem>().Play();
            MuzzleFl.transform.parent = attackPoint;
            Destroy(MuzzleFl, 0.1f);
        }
        //Invoke resetshoot funtcion
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
        }
        
        readyToShoot = false;
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray,out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }
        //calculate direction from attackpoint to targetpoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //calcualte new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        Destroy(currentBullet, 2f);
        //rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;
        //add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //instantiate to bullet
        
        bulletLeft--;
        bulletsShot++;
        //if more than bulletPerTap like shotgun, pistol
        if(bulletsShot<bulletsPerTap && bulletLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletLeft = magazineSize;
        reloading = false;
    }
}
