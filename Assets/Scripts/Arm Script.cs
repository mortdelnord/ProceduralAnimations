using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    public TargetLocations targetLocations;
    public Transform raycastOrigin;
    public Transform insideRaycastOrigin;
    private Vector3 raycastGround;
    private Vector3 insideRaycastGround;
    
    [Range(.01f, 1f)]
    public float maxTargetDis;

    [Range (.01f, 3f)]
    public float maxBodyDis;

    [Range(.01f, 1f)]
    public float minBodyDis;

    public float raycasyDis = 5f;
    public float legSpeed;
    public float centerOffset;

    private float timer = 0f;
    public float timerMax = 100f;

    public Transform mainBody;

    private Vector3 targetposition;
    private Vector3 newPos;
    private bool shouldMove = false;

    private void Awake()
    {
        targetposition = transform.position;
    }

    public void Update()
    {
        float dynamicLegSpeed = legSpeed * targetLocations.bodySpeed;
        RaycastHit hit;
        RaycastHit insideHit;
        Physics.Raycast(raycastOrigin.position, Vector3.down, out hit, raycasyDis);
        Physics.Raycast(insideRaycastOrigin.position, Vector3.down, out insideHit, raycasyDis);
        
        Debug.DrawRay(raycastOrigin.position, Vector3.down, Color.blue);
        Debug.DrawRay(insideRaycastOrigin.position, Vector3.down, Color.red);

        raycastGround = hit.point;
        insideRaycastGround = insideHit.point;

        ////Debug.Log(Vector3.Distance(raycastGround, targetposition));
        if (Vector3.Distance(raycastGround, targetposition) >= maxTargetDis || Vector3.Distance(transform.position, mainBody.position) <= minBodyDis)
        {
            if (!shouldMove)
            {
                shouldMove = true;
                newPos = raycastGround;
            }
            ////Debug.Log("distance too great, should move leg");
        }

        if (Vector3.Distance(transform.position, mainBody.position) >= maxBodyDis)
        {
            if (!shouldMove)
            {
                shouldMove = true;
                newPos = insideRaycastGround;
            }
        }


        if (shouldMove)
        {
            //Debug.Log("Beginning to Move");
            //targetposition = newPos;
            Vector3 center = (newPos + targetposition) * 0.5f;

            center -= new Vector3(0, centerOffset, 0);
            Vector3 startPosCenter = targetposition - center;
            Vector3 endPosCenter = newPos - center;

            timer += Time.deltaTime * legSpeed;
            float timerFrac = timer/timerMax;
    
            // float fracComplete = (Time.time - startTime) / journeyTime;
            ////Debug.Log(timerFrac);

            transform.position = Vector3.Slerp(startPosCenter, endPosCenter, timerFrac);
            transform.position += center;
            if (timerFrac >= 1f)
            {
                timer = 0f;
                //startTime = Time.time;
                //fracComplete = 0f;
                ////Debug.Log("Finished mmoving");
                shouldMove = false;
                targetposition = newPos;
                
            }
        }else
        {
            transform.position = targetposition;
        }
        //transform.position = targetposition;
    }
}
