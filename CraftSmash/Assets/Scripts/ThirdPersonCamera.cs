using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public bool lockCursor;
    public float mouseSensitivity = 5;
    public Transform target;
    public float maxDistanceFromTarget = 4;
    public float minDistanceFromTarget = 0.5f;

    public float currentDistanceFromTarget;
    public Vector2 pitchMinMax = new Vector2(0, 85);

    PlayerCharacterController player;
    BossController boss;

    [SerializeField]
    Camera cam;

    public float rotationSmoothTime = 0.08f;

    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public LayerMask collisionLayer;

    float yaw;
    float pitch;
    
    


    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        boss = GameObject.Find("Boss").GetComponent<BossController>();

        currentDistanceFromTarget = maxDistanceFromTarget;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        
    }

    void LateUpdate()
    {
        if (!player.escapeMenu && !player.craftingMenu && !player.introTutorial)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch += Input.GetAxis("Mouse Y") * mouseSensitivity * -1;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            RaycastHit hit;
            Vector3 distance;
            distance = (transform.position - target.position);
            float cameraDistance = distance.magnitude + 0.1f;
            Vector3 towardCamera = (transform.position - target.position).normalized;


            //Camera Collision
            if (Physics.Raycast(target.position, towardCamera, out hit, cameraDistance, collisionLayer))
            {
                currentDistanceFromTarget = (target.position - hit.point).magnitude;
            }
            else
            {
                currentDistanceFromTarget = maxDistanceFromTarget;
            }

            if (player.isAlive && boss.isAlive)
            {
                
                currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
                transform.eulerAngles = currentRotation;

                //Bow aiming
                if (player.isBowAiming)
                {
                    Vector3 bowAimHeight = new Vector3(0, 1, 0);

                    

                    transform.position = (target.position + bowAimHeight) - transform.forward * currentDistanceFromTarget;
                }
                else
                {
                    transform.position = target.position - transform.forward * currentDistanceFromTarget;
                }

                
            }
        }
    }

    


}
