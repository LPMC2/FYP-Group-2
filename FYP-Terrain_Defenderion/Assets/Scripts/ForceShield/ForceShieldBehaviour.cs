using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceShieldBehaviour : CollisionDetector
{
    // Update is called once per frame
    void Update()
    {
        if(HitEntities != null)
        {
            foreach (GameObject hitObj in HitEntities)
            {
                Projectile projectile = hitObj.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.ResetProj();
                }
                else
                    HitEntities.Remove(hitObj);
            }
        }
    }
}
