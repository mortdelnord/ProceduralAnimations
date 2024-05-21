using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderArmV4 : MonoBehaviour
{
    public SpiderBodyV4 spiderBody;
    [Header("Leg and Body")]
    public Transform leg;
    public Transform body;
    [Header("Raycast Reference")]
    public Transform raycastInside;
    public Transform raycastOutside;

    private RaycastHit insideHit;
    private RaycastHit outsideHit;

    [Header("Floats and Ranges")]
    
    [Range(.01f, 1f)]
    public float maxTargetDis;

    [Range (.01f, 3f)]
    public float maxBodyDis;

    [Range(.01f, 1f)]
    public float minBodyDis;
    [Range(0.1f, 10f)]
    public float raycastDis;

    [Range(0.1f, 1000f)]
    public float legSpeed;
    private float bodySpeed;
    [Range(0.01f, 1f)]
    public float centerOffset;
    [Range(0.1f, 100f)]
    public float timerMax = 100f;
    private float timer = 0f;

    //private vars

    private Vector3 groundInside;
    private Vector3 groundOutside;
    private Vector3 target;
    private Vector3 newPos;
    private Vector3 lastBodyPos;

    public bool isGrounded = true;
    public bool moveLeg = false;

    private void Start()
    {
        target = transform.position;
    }


    private void Update()
    {
        bodySpeed = spiderBody.bodySpeed;
        Physics.Raycast(raycastInside.position, Vector3.down, out insideHit, raycastDis);
        Physics.Raycast(raycastOutside.position, Vector3.down, out outsideHit, raycastDis);

        Debug.DrawRay(raycastInside.position, Vector3.down, Color.blue);
        Debug.DrawRay(raycastOutside.position, Vector3.down, Color.red);

        groundInside = insideHit.point;
        groundOutside = outsideHit.point;

        float insdeDis = Vector3.Distance(transform.position, groundInside);
        float outsideDis = Vector3.Distance(transform.position, groundOutside);
        float bodyDis = Vector3.Distance(transform.position, body.position);

        if (insdeDis >= maxTargetDis && isGrounded)
        {
            Debug.Log(" inside more than max target dis");
            moveLeg = true;
            //Debug.Log(gameObject + " insidedis more than max " + newPos);
            isGrounded = false;
            newPos = groundInside;
        }
        if(outsideDis >= maxTargetDis && isGrounded)
        {
            Debug.Log(" outside more than max target dis");
            moveLeg = true;
            //Debug.Log(gameObject + " outside dic more than max " + newPos);
            isGrounded = false;
            newPos = groundOutside;
        }
        if (bodyDis >= maxBodyDis && isGrounded)
        {
            Debug.Log(" body more than max body dis");
            moveLeg = true;
            //Debug.Log(gameObject + " body dis more than max " + newPos);
            isGrounded = false;
            newPos = groundInside;
        }
        if (bodyDis <= minBodyDis && isGrounded)
        {
            Debug.Log(" body less than min body dis");
            moveLeg = true;
            //Debug.Log(gameObject + " body dis less than min " + newPos);
            isGrounded = false;
            newPos = groundOutside;
        }
        

        if (!isGrounded && moveLeg)
        {
            Vector3 center = (newPos + target) * 0.5f;
            center -= new Vector3(0, centerOffset, 0);

            Vector3 startCenter = target - center;
            Vector3 endCenter = newPos - center;
            float stepDis;
            float newBodyDis; 
            float timerFrac;
            // if (bodySpeed >= 1f)
            // {
            //     stepDis = Vector3.Distance(target, newPos);
            //     newBodyDis = Vector3.Distance(lastBodyPos, body.position); 
            //     timerFrac = Mathf.InverseLerp(0f,stepDis, newBodyDis) * 2f;

            // }else
            // {
            //     timer += Time.deltaTime * legSpeed;
            //     timerFrac = timer/timerMax;
            // }
            stepDis = Vector3.Distance(target, newPos);
            newBodyDis = Vector3.Distance(lastBodyPos, body.position); 
            Debug.DrawLine(lastBodyPos, body.position, Color.blue);
            timerFrac = Mathf.InverseLerp(0f,stepDis, newBodyDis) * 2f;

            // timer += Time.deltaTime * legSpeed;
            // float timerFrac = timer/timerMax;
            //timerFrac *= bodySpeed;
            //Debug.Log(timerFrac);

            transform.position = Vector3.Slerp(startCenter, endCenter, timerFrac);
            transform.position += center;

            if (timerFrac >= 1f || timerFrac <= 0f)
            {
                isGrounded = true;
                if (timerFrac >= 1f)
                {
                    target = newPos;
                    //
                }
                timer = 0f;
                //moveLeg = false;
            }
        }else
        {
            lastBodyPos = body.position;
            transform.position = target;
        }

        
    }

    private void OnDrawGizmos()
    {
        
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPos, 0.01f);
        Gizmos.DrawWireSphere(target, 0.01f);
        Gizmos.DrawLine(target, newPos);
    }

    
}
