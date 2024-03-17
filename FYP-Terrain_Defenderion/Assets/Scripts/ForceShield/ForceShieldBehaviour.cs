using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceShieldBehaviour : CollisionDetector
{
    // Update is called once per frame
    void Update()
    {
        if(HitEntity != null)
        {
            Projectile projectile = HitEntity.GetComponent<Projectile>();
            if(projectile != null)
            {
                projectile.ResetProj();
            } else
            Destroy(HitEntity);
        }
    }
}
