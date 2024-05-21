using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderV3 : MonoBehaviour
{
    [Header("Arm References")]
    public Transform FLleg; // Front Left Leg
    public Transform FRleg; // Front Right Leg
    public Transform BLleg; // Back Left Leg
    public Transform BRleg; // Back Right Leg

    [Header("Body Reference")]
    public Transform body;

    [Header("Raycast Origin Points")]
    public Transform FROorigin; // Front Right Outside 
    public Transform FRIorigin; // Front Right Inside
    public Transform FLOorigin; // Front Left Outside
    public Transform FLIorigin; // Front Left Inside
    public Transform BROorigin; // Back Right Outside
    public Transform BRIorigin; // Back RIght Inside
    public Transform BLOorigin; // Back Left Outside
    public Transform BLIorigin; // Back Left Inside

    [Header("Floats and Ranges")]
    
    [Range(.01f, 1f)]
    public float maxTargetDis;

    [Range (.01f, 3f)]
    public float maxBodyDis;

    [Range(.01f, 1f)]
    public float minBodyDis;
    [Range(0.1f, 10f)]
    public float raycasyDis;

    [Range(0.1f, 1000f)]
    public float legSpeed;
    [Range(0.01f, 1f)]
    public float centerOffset;
    [Range(0.1f, 100f)]
    public float timerMax = 100f;


    //private Vars

    private Vector3 FRground; // front right ground point
    private Vector3 FLground; // front left ground point
    private Vector3 BRground; // back right ground point
    private Vector3 BLground; // back left ground point

    private Vector3 FRtarget;
    private Vector3 FRnewPos;

    private Vector3 FLtarget;
    private Vector3 FLnewPos;

    private Vector3 BRtarget;
    private Vector3 BRnewPos;

    private Vector3 BLtarget;
    private Vector3 BLnewPos;

    private Vector3 FRcenter;
    private Vector3 FLcenter;
    private Vector3 BRcenter;
    private Vector3 BLcenter;

    private Vector3 FRstartCenter;
    private Vector3 FRendCenter;

    private Vector3 FLstartCenter;
    private Vector3 FLendCenter;

    private Vector3 BRstartCenter;
    private Vector3 BRendCenter;

    private Vector3 BLstartCenter;
    private Vector3 BLendCenter;

    private bool isGrounded = true;
    private bool moveFR = false;
    private bool moveFL = false;
    private bool moveBR = false;
    private bool moveBL = false;
    private float FRtimer = 0f;
    private float FLtimer = 0f;
    private float BRtimer = 0f;
    private float BLtimer = 0f;

    private RaycastHit FROhit;
    private RaycastHit FRIhit;
    private RaycastHit FLOhit;
    private RaycastHit FLIhit;
    private RaycastHit BROhit;
    private RaycastHit BRIhit;
    private RaycastHit BLOhit;
    private RaycastHit BLIhit;

    [Header("BodyOffset")]
    private Vector3 averagePos;
    public float bodyHeightOffset;
    public float heightSpeed;


    private void Awake()
    {
        FRtarget = FRleg.position;
        FLtarget = FLleg.position;
        BRtarget = BRleg.position;
        BLtarget = BLleg.position;

    }
    private void Update()
    {
        BodyHeight();
        ShootRaycast();
        CheckDistances();
        MoveLegs();
        //Debug.Log(isGrounded);
    }

    private void ShootRaycast()
    {
        Physics.Raycast(FROorigin.position, Vector3.down, out FROhit, raycasyDis);
        Physics.Raycast(FLOorigin.position, Vector3.down, out FLOhit, raycasyDis);
        Physics.Raycast(BROorigin.position, Vector3.down, out BROhit, raycasyDis);
        Physics.Raycast(BLOorigin.position, Vector3.down, out BLOhit, raycasyDis);
        Physics.Raycast(FRIorigin.position, Vector3.down, out FRIhit, raycasyDis);
        Physics.Raycast(FLIorigin.position, Vector3.down, out FLIhit, raycasyDis);
        Physics.Raycast(BRIorigin.position, Vector3.down, out BRIhit, raycasyDis);
        Physics.Raycast(BLIorigin.position, Vector3.down, out BLIhit, raycasyDis);

        Debug.DrawRay(FROorigin.position, Vector3.down, Color.blue);
        Debug.DrawRay(FLOorigin.position, Vector3.down, Color.blue);
        Debug.DrawRay(BROorigin.position, Vector3.down, Color.blue);
        Debug.DrawRay(BLOorigin.position, Vector3.down, Color.blue);

        Debug.DrawRay(FRIorigin.position, Vector3.down, Color.red);
        Debug.DrawRay(FLIorigin.position, Vector3.down, Color.red);
        Debug.DrawRay(BRIorigin.position, Vector3.down, Color.red);
        Debug.DrawRay(BLIorigin.position, Vector3.down, Color.red);
    }

    private void CheckDistances()
    {

        float FRdistanceBody = Vector3.Distance(FRleg.position, body.position);
        float FRdistanceOutside = Vector3.Distance(FRleg.position, FROhit.point);
        float FRdistanceInside = Vector3.Distance(FRleg.position, FRIhit.point);

        float FLdistanceBody = Vector3.Distance(FLleg.position, body.position);
        float FLdistanceOutside = Vector3.Distance(FLleg.position, FLOhit.point);
        float FLdistanceInside = Vector3.Distance(FLleg.position, FLIhit.point);

        float BRdistanceBody = Vector3.Distance(BRleg.position, body.position);
        float BRdistanceOutside = Vector3.Distance(BRleg.position, BROhit.point);
        float BRdistanceInside = Vector3.Distance(BRleg.position, BRIhit.point);

        float BLdistanceBody = Vector3.Distance(BLleg.position, body.position);
        float BLdistanceOutside = Vector3.Distance(BLleg.position, BLOhit.point);
        float BLdistanceInside = Vector3.Distance(BLleg.position, BLIhit.point);


        if ((FRdistanceBody <= minBodyDis && isGrounded)|| (FLdistanceBody <= minBodyDis && isGrounded) || (BRdistanceBody <= minBodyDis && isGrounded)|| (BLdistanceBody <= minBodyDis && isGrounded))
        {
            //isGrounded = false;
            if (FRdistanceBody <= minBodyDis && !moveFR && isGrounded)
            {
                isGrounded = false;
                FRnewPos = FROhit.point;
                moveFR = true;
            }else if (FLdistanceBody <= minBodyDis && !moveFL && isGrounded)
            {
                isGrounded = false;
                FLnewPos = FLOhit.point;
                moveFL = true;
            }else if (BRdistanceBody <= minBodyDis && !moveBR && isGrounded)
            {
                isGrounded = false;
                BRnewPos = BROhit.point;
                moveBR = true;
            }else if (BLdistanceBody <= minBodyDis && !moveBL && isGrounded)
            {
                isGrounded = false;
                BLnewPos = BLOhit.point;
                moveBL = true;
            }
        }else if ((FRdistanceBody >= maxBodyDis && isGrounded)|| (FLdistanceBody >= maxBodyDis && isGrounded) || (BRdistanceBody >= maxBodyDis && isGrounded) || (BLdistanceBody >= maxBodyDis && isGrounded))
        {
            //isGrounded = false;
            if (FRdistanceBody >= maxBodyDis && !moveFR && isGrounded)
            {
                isGrounded = false;
                FRnewPos = FRIhit.point;
                moveFR = true;

                BLnewPos = BLOhit.point;
                moveBL = true;
                
            }else if (FLdistanceBody >= maxBodyDis && !moveFL && isGrounded)
            {
                isGrounded = false;
                FLnewPos = FLIhit.point;
                moveFL = true;

                BRnewPos = BROhit.point;
                moveBR = true;

            }else if (BRdistanceBody >= maxBodyDis && !moveBR && isGrounded)
            {
                isGrounded = false;
                BRnewPos = BRIhit.point;
                moveBR = true;

                FLnewPos = FLOhit.point;
                moveFL = true;

            }else if (BLdistanceBody >= maxBodyDis && !moveBL && isGrounded)
            {
                isGrounded = false;
                BLnewPos = BLIhit.point;
                moveBL = true;

                FRnewPos = FROhit.point;
                moveFR = true;
            }
        }else if ((FRdistanceInside >= maxTargetDis && FRdistanceOutside >= maxTargetDis && isGrounded) || (FLdistanceInside >= maxTargetDis && FLdistanceOutside >= maxTargetDis && isGrounded) 
            || (BRdistanceInside >= maxTargetDis && BRdistanceOutside >= maxTargetDis && isGrounded) || (BLdistanceInside >= maxTargetDis && BLdistanceOutside >= maxTargetDis && isGrounded))
        {
            //isGrounded = false;

            if (FRdistanceInside >= maxTargetDis && FRdistanceOutside >= maxTargetDis && !moveFR  && isGrounded)
            {
                isGrounded = false;
                moveFR = true;
                if (FRdistanceInside > FRdistanceOutside)
                {
                    FRnewPos = FRIhit.point;
                }else
                {
                    FRnewPos = FROhit.point;
                }
            }else if (FLdistanceInside >= maxTargetDis && FLdistanceOutside >= maxTargetDis && !moveFL  && isGrounded)
            {
                isGrounded = false;
                moveFL = true;
                if (FLdistanceInside > FLdistanceOutside)
                {
                    FLnewPos = FLIhit.point;
                }else
                {
                    FLnewPos = FLOhit.point;
                }
            }else if (BRdistanceInside >= maxTargetDis && BRdistanceOutside >= maxTargetDis && !moveBR  && isGrounded)
            {
                isGrounded = false;
                moveBR = true;
                if (BRdistanceInside > BRdistanceOutside)
                {
                    BRnewPos = BRIhit.point;
                }else
                {
                    BRnewPos = BROhit.point;
                }
            }else if (BLdistanceInside >= maxTargetDis && BLdistanceOutside >= maxTargetDis && !moveBL  && isGrounded)
            {
                isGrounded = false;
                moveBL = true;
                if (BLdistanceInside > BLdistanceOutside)
                {
                    BLnewPos = BLIhit.point;
                }else
                {
                    BLnewPos = BLOhit.point;
                }
            }
        }
    }
    
    private void MoveLegs()
    {
        if (!isGrounded)
        {
            //Front Right Leg Movement 
            if (moveFR)
            {
                moveFL = false;
                moveBR = false;

                FRcenter = (FRnewPos + FRtarget) * 0.5f;
                FRcenter -= new Vector3(0, centerOffset, 0);

                FRstartCenter = FRtarget - FRcenter;
                FRendCenter = FRnewPos - FRcenter;

                FRtimer += Time.deltaTime * legSpeed;
                float timerFrac = FRtimer/timerMax;

                FRleg.position = Vector3.Slerp(FRstartCenter, FRendCenter, timerFrac);
                FRleg.position += FRcenter;

                if (timerFrac >= 1)
                {
                    FRtimer = 0f;
                    isGrounded = true;
                    moveFR = false;
                    FRtarget = FRnewPos;

                }

            }else
            {
                FRleg.position = FRtarget;
            }

            if (moveFL)
            {
                moveFR = false;
                moveBL = false;
                FLcenter = (FLnewPos + FLtarget) * 0.5f;
                FLcenter -= new Vector3(0, centerOffset, 0);

                FLstartCenter = FLtarget - FLcenter;
                FLendCenter = FLnewPos - FLcenter;

                FLtimer += Time.deltaTime * legSpeed;
                float timerFrac = FLtimer/timerMax;

                FLleg.position = Vector3.Slerp(FLstartCenter, FLendCenter, timerFrac);
                FLleg.position += FLcenter;

                if (timerFrac >= 1)
                {
                    FLtimer = 0f;
                    isGrounded = true;
                    moveFL = false;
                    FLtarget = FLnewPos;

                }
            }else
            {
                FLleg.position = FLtarget;
            }

            if (moveBR)
            {
                moveBL = false;
                moveFR = false;
                BRcenter = (BRnewPos + BRtarget) * 0.5f;
                BRcenter -= new Vector3(0, centerOffset, 0);

                BRstartCenter = BRtarget - BRcenter;
                BRendCenter = BRnewPos - BRcenter;

                BRtimer += Time.deltaTime * legSpeed;
                float timerFrac = BRtimer/timerMax;

                BRleg.position = Vector3.Slerp(BRstartCenter, BRendCenter, timerFrac);
                BRleg.position += BRcenter;

                if (timerFrac >= 1)
                {
                    BRtimer = 0f;
                    isGrounded = true;
                    moveBR = false;
                    BRtarget = BRnewPos;

                }
            }else
            {
                BRleg.position = BRtarget;
            }

            if (moveBL)
            {
                moveBR = false;
                moveFL = false;
                BLcenter = (BLnewPos + BLtarget) * 0.5f;
                BLcenter -= new Vector3(0, centerOffset, 0);

                BLstartCenter = BLtarget - BLcenter;
                BLendCenter = BLnewPos - BLcenter;

                BLtimer += Time.deltaTime * legSpeed;
                float timerFrac = BLtimer/timerMax;

                BLleg.position = Vector3.Slerp(BLstartCenter, BLendCenter, timerFrac);
                BLleg.position += BLcenter;

                if (timerFrac >= 1)
                {
                    BLtimer = 0f;
                    isGrounded = true;
                    moveBL = false;
                    BLtarget = BLnewPos;

                }
            }else
            {
                BLleg.position = BLtarget;
            }
            
        }else
        {
            FRleg.position = FRtarget;
            FLleg.position = FLtarget;
            BRleg.position = BRtarget;
            BLleg.position = BLtarget;
        }
    }


    private void BodyHeight()
    {
        averagePos = (FRleg.position + FLleg.position + BRleg.position + BLleg.position) / 4f;
        Vector3 nmewBodyPos = new Vector3(body.position.x, averagePos.y + bodyHeightOffset, body.position.z);
        body.position = Vector3.Lerp(body.position, nmewBodyPos, Time.deltaTime * heightSpeed);
    }
}
