using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    PlayerCharacterController player;
    public Camera cam;

    public float damage = 10f;
    public float range = 100f;
    float bowShootTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && player.isBowAiming)
        {
            Shoot();
        }
        bowShootTimer += Time.deltaTime;
    }

    void Shoot()
    {
        if (player.arrowCount > 0 && bowShootTimer >= 0.8f)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
            {
                BossController boss = hit.transform.GetComponent<BossController>();

                if (boss != null)
                {
                    if (boss.currentShieldPoints > 0)
                    {
                        boss.TakeShieldDamage(damage);
                    }
                    else
                    {
                        boss.TakeDamage(damage);
                    }
                }
            }

            player.arrowCount -= 1;
            bowShootTimer = 0;
        }
    }
}
