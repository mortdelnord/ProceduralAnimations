using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ArmScriptV5 : MonoBehaviour
{
    [Header("Body Script")]
    public SpiderBodyV5 spiderBody;
    public bool isGroup1;
    //public SpiderBodyV4 spiderBody;
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
    public float maxDisToNew;
    public Vector3 rotatePlane;

    //private vars

    private Vector3 groundInside;
    private Vector3 groundOutside;
    private Vector3 target;
    private Vector3 newPos;
    private Vector3 lastBodyPos;
    private Vector3 midpoint;

    public bool isGrounded = true;
    

    private void Start()
    {
        target = transform.position;
    }


    private void Update()
    {
        //bodySpeed = spiderBody.bodySpeed;
        Physics.Raycast(raycastInside.position, Vector3.down, out insideHit, raycastDis);
        Physics.Raycast(raycastOutside.position, Vector3.down, out outsideHit, raycastDis);

        Debug.DrawRay(raycastInside.position, Vector3.down, Color.blue);
        Debug.DrawRay(raycastOutside.position, Vector3.down, Color.red);

        groundInside = insideHit.point;
        groundOutside = outsideHit.point;

        float insdeDis = Vector3.Distance(transform.position, groundInside);
        float outsideDis = Vector3.Distance(transform.position, groundOutside);
        float bodyDis = Vector3.Distance(transform.position, body.position);
        

        if (!isGrounded)
        {
            #region Slerp Method
            Vector3 center = (newPos + target) * 0.5f;

            center -= new Vector3(0, centerOffset, 0);
            Vector3 startPosCenter = target - center;
            Vector3 endPosCenter = newPos - center;
            
            timer += Time.deltaTime * legSpeed;
            //float timerFrac = timer/timerMax;
            float timerFrac = Mathf.SmoothStep(0f, 1f, timer);
    
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
                isGrounded = true;
                target = newPos;
                if (isGroup1)
                {
                    spiderBody.FRarmMove = false;
                }else
                {
                    spiderBody.FRarmMove = true;
                }
                spiderBody.isWalking = false;
                spiderBody.isGrounded = true;
                
            }
            #endregion
            #region Creep Method
            // Vector3 center = (newPos + target) * 0.5f;
            // center -= new Vector3(0, centerOffset, 0);

            // Vector3 startCenter = target - center;
            // Vector3 endCenter = newPos - center;
            // float stepDis;
            // float newBodyDis; 
            // float timerFrac;
            
            // stepDis = Vector3.Distance(target, newPos);
            // newBodyDis = Vector3.Distance(lastBodyPos, body.position); 
            // Debug.DrawLine(lastBodyPos, body.position, Color.blue);
            // timerFrac = Mathf.InverseLerp(0f,stepDis, newBodyDis) * 2f;
            // transform.position = Vector3.Slerp(startCenter, endCenter, timerFrac);
            // transform.position += center;

            // if (timerFrac >= 1f || timerFrac <= 0f)
            // {
            //     Debug.Log("timer frac less than 0 or more than 1");
            //     isGrounded = true;
                
            //     target = newPos;
            //     if (isGroup1)
            //     {
            //         spiderBody.FRarmMove = false;
            //     }else
            //     {
            //         spiderBody.FRarmMove = true;
            //     }
            //     spiderBody.isWalking = false;
            //     spiderBody.isGrounded = true;
            //     timer = 0f;
                
            //     //moveLeg = false;
            // }
            #endregion

            
        }else
        {
            if (insdeDis >= maxTargetDis && isGrounded)
        {
            //Debug.Log(" inside more than max target dis");
            
            //Debug.Log(gameObject + " insidedis more than max " + newPos);
            
            newPos = groundInside;
        }
        if(outsideDis >= maxTargetDis && isGrounded)
        {
            //Debug.Log(" outside more than max target dis");
            
            //Debug.Log(gameObject + " outside dic more than max " + newPos);
            
            newPos = groundOutside;
        }
        if (bodyDis >= maxBodyDis && isGrounded)
        {
            //Debug.Log(" body more than max body dis");
            
            //Debug.Log(gameObject + " body dis more than max " + newPos);
            
            newPos = groundInside;
        }
        if (bodyDis <= minBodyDis && isGrounded)
        {
            //Debug.Log(" body less than min body dis");
            
            //Debug.Log(gameObject + " body dis less than min " + newPos);
            
            newPos = groundOutside;
        }
            //lastBodyPos = body.position;
            transform.position = target;
            lastBodyPos = body.position;
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
