using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBodyV4 : MonoBehaviour
{
    [Header("Arms")]
    public SpiderArmV4 FRarm;
    public SpiderArmV4 Flarm;
    public SpiderArmV4 BRarm;
    public SpiderArmV4 BLarm;
    [Header("Arm Transforms")]
    public Transform FRpos;
    public Transform FLpos;
    public Transform BRpos;
    public Transform BLpos;
    [Header("Height offset")]
    [Range(0f, 1f)]
    public float heightOffset;
    public float heightSpeed;
    public float bodySpeed = 0f;

    [Header("Rotation")]
    public float angleMult;
    public float rotateSpeed;
    private Vector3 velocity;
    private Vector3 prevPos;
    private bool FRmove = false;
    private bool FLmove = false;
    private bool BRmove = false;
    private bool BLmove = false;

    private void Awake()

    {
        prevPos = transform.position;
    }
    private void Update()
    {
        ChangeHeight();
        ChangeRotation();
        FRmove = FRarm.moveLeg;
        FLmove = Flarm.moveLeg;
        BRmove = BRarm.moveLeg;
        BLmove = BLarm.moveLeg;

        
        // if (FRarm.moveLeg || BLarm.moveLeg)
        // {
        //     FRarm.isGrounded = false;
        //     BLarm.isGrounded = false;
        //     Flarm.moveLeg = false;
        //     BRarm.moveLeg = false;
        // }else if (Flarm.moveLeg || BLarm.moveLeg)
        // {
        //     Flarm.isGrounded = false;
        //     BRarm.isGrounded = false;
        //     FRarm.moveLeg = false;
        //     BLarm.moveLeg = false;
        // }

        velocity = (transform.position - prevPos)/ Time.deltaTime;
        bodySpeed = velocity.magnitude * 2f;
        prevPos = transform.position;
    }

    private void ChangeHeight()
    {
        Vector3 averageHeight = (FRpos.position + FLpos.position + BRpos.position + BLpos.position)/4f;
        Vector3 newHeight = new Vector3(transform.position.x, averageHeight.y + heightOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newHeight, Time.deltaTime * heightSpeed);
    }
    private void ChangeRotation()
    {
        transform.up = Vector3.Slerp(transform.up, FindAverageNormal(), Time.deltaTime * rotateSpeed);
    }

     private Vector3 FindAverageNormal()
    {
        
        Vector3 n1 = FindNormal(FRarm.transform.position, Flarm.transform.position, BRarm.transform.position);
        Vector3 n2 = FindNormal(BLarm.transform.position, FRarm.transform.position, BRarm.transform.position);
        Vector3 n3 = FindNormal(Flarm.transform.position, FRarm.transform.position, BLarm.transform.position);
        Vector3 n4 = FindNormal(BRarm.transform.position, Flarm.transform.position, BLarm.transform.position);

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
}
