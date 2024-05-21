using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SpiderBodyV5 : MonoBehaviour
{
    [Header("Arm Scripts")]
    public ArmScriptV5 FRarm;
    public ArmScriptV5 FLarm;
    public ArmScriptV5 BRarm;
    public ArmScriptV5 BLarm;

    [Header("Previous Body Position")]
    public Vector3 prevBodyPos;
    [Header("Distance Limits")]
    [Range(0.001f, 2f)]
    public float bodyMaxDis;
    [Header("height offset")]
    [Range(-10f, 10f)]
    public float heightOffset;
    [Header("Height Speed")]
    public float heightSpeed;
    [Header("Angel Multiplier")]
    public float angleMult = 1f;
    [Header("Rotate Speed")]
    [Range(0.1f, 100f)]
    public float rotateSpeed;

    public bool isWalking = false;
    public bool isGrounded = true;
    public bool FRarmMove = true;


    private void Start()
    {
        prevBodyPos = transform.position;
    }
    private void Update()
    {
        ChangeHeight();
        ChangeRotation();
        if (isGrounded)
        {

        }
        if (!isWalking)
        {
            prevBodyPos = transform.position;
            isWalking = true;
        }

        if (prevBodyPos != null)
        {
            float bodyDis = Vector3.Distance(prevBodyPos, transform.position);
            if (bodyDis >= bodyMaxDis && isGrounded)
            {
                isGrounded = false;
                if(FRarmMove)
                {
                    Debug.Log("Move Front Right and Back Left ");
                    FRarm.isGrounded = false;
                    BLarm.isGrounded = false;
                }else
                {
                    Debug.Log("Move Front Left and Back Right ");
                    FLarm.isGrounded = false;
                    BRarm.isGrounded = false;
                }
            }
        }
        
    }
    private void ChangeHeight()
    {

        Vector3 averageHeight = (FRarm.transform.position + FLarm.transform.position + BRarm.transform.position + BLarm.transform.position)/4f;
        Vector3 newHeight = new Vector3(transform.position.x, averageHeight.y + heightOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newHeight, Time.deltaTime * heightSpeed);
    }


    private void ChangeRotation()
    {
        transform.up = Vector3.Slerp(transform.up, FindAverageNormal(), Time.deltaTime * rotateSpeed);
    }

    private Vector3 FindAverageNormal()
    {
        
        Vector3 n1 = FindNormal(FRarm.transform.position, FLarm.transform.position, BRarm.transform.position);
        Vector3 n2 = FindNormal(BLarm.transform.position, FRarm.transform.position, BRarm.transform.position);
        Vector3 n3 = FindNormal(FLarm.transform.position, FRarm.transform.position, BLarm.transform.position);
        Vector3 n4 = FindNormal(BRarm.transform.position, FLarm.transform.position, BLarm.transform.position);

        Vector3 averageAngle = (n1 + n2 + n3 + n4)/ 4f;
        averageAngle.x = averageAngle.x * angleMult;
        averageAngle.y = averageAngle.y * angleMult;
        averageAngle.z = averageAngle.z * angleMult;
        //Debug.DrawRay(transform.position, averageAngle.normalized, Color.green, 2f);
        return averageAngle;
    }

    private Vector3 FindNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Plane plane = new Plane(p1, p2, p3);
        Vector3 positivePoint = (p1+p2+p3)/3f;
        positivePoint += Vector3.up;
        if (!plane.GetSide(positivePoint))
        {
            plane.Flip();
        }
        Vector3 normal = plane.normal;
        //Debug.DrawRay(p1, normal, Color.cyan, 2f);
        //Debug.Log(normal + "To plane: " + plane);
        return normal;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(prevBodyPos, new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.DrawCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.DrawLine(prevBodyPos, transform.position);
        
    }

    

}
