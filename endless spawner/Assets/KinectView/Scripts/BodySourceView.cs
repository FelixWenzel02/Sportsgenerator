using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UniRx;
using UnityEngine.UI;
using Kinect = Windows.Kinect;
using Vector3 = UnityEngine.Vector3;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    
    // this is set to true after the T-pose calibration has been done successfully
    private bool hasBeenCalibrated;
    [SerializeField] private Image fillbar;
    [SerializeField] private Image calibrationPanel;
    [SerializeField] private TextMeshProUGUI calibrationSuccessfulText;
    [SerializeField] private TextMeshProUGUI testUIDetected;
    
    public ReactiveProperty<bool> jumped;
    public ReactiveProperty<bool> crouched;

    /// <summary>
    /// 
    /// Moved left/right stay true as long as the player stays left or right.
    ///
    /// Both values only change if the player moved back to the middle.
    /// 
    /// </summary>
    public ReactiveProperty<bool> movedLeft;
    public ReactiveProperty<bool> movedRight;
    public ReactiveProperty<bool> movedMiddle;

    /// <summary>
    /// 
    /// This function waits for some time and sets the jumped value to false again, so the next jump
    /// can be registered.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForNextJump()
    {
        yield return new WaitForSeconds(2);
        jumped.Value = false;
    }
    
    /// <summary>
    /// 
    /// This function waits for some time and sets the crouched value to false again, so the next crouch
    /// can be registered.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForNextCrouch()
    {
        yield return new WaitForSeconds(2);
        crouched.Value = false;
    }
    
    

    private IEnumerator DisplayDetectionText(string detectedAction)
    {
        testUIDetected.gameObject.SetActive(true);
        testUIDetected.text = $"Detected: {detectedAction}";
        yield return new WaitForSeconds(1.5f);
        testUIDetected.gameObject.SetActive(false);
    } 
    
    private void Awake()
    {
        jumped = new ReactiveProperty<bool>(false);
        crouched = new ReactiveProperty<bool>(false);
        movedLeft = new ReactiveProperty<bool>(false);
        movedRight = new ReactiveProperty<bool>(false);
        movedMiddle = new ReactiveProperty<bool>(false);
        
        jumped.Subscribe(x =>
        {
            if (x && hasBeenCalibrated)
            {
                Debug.Log("Jump has been detected. (Reactively..)");
                StartCoroutine(DisplayDetectionText("jump"));
                StartCoroutine(WaitForNextJump());
            }
        });

        crouched.Subscribe(x =>
        {
            if (x && hasBeenCalibrated)
            {
                Debug.Log("Crouch has been detected. (Reactively..)");
                StartCoroutine(DisplayDetectionText("crouch"));
                StartCoroutine(WaitForNextCrouch());
            }
        });

        movedLeft.Subscribe(x =>
        {
            if (x && hasBeenCalibrated)
            {
                Debug.Log("Move to left has been detected. (Reactively..)");
                StartCoroutine(DisplayDetectionText("moved to left")); 
            }
        });

        movedRight.Subscribe(x =>
        {
            if (x && hasBeenCalibrated)
            {
                Debug.Log("Move to right has been detected. (Reactively..)");
                StartCoroutine(DisplayDetectionText("moved to right")); 
            }
        });

        // moving to middle sets movedLeft and movedRight to false
        movedMiddle.Subscribe(x =>
        {
            StartCoroutine(DisplayDetectionText("moved to middle"));
            movedRight.Value = false;
            movedLeft.Value = false;
        });

    }

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    
    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
        
        DoCalibration();
        
        if (hasBeenCalibrated)
        {
            DetectJump();
            DetectCrouch();
            DetectLeftStep();
            DetectRightStep();
            DetectMoveToMiddle();
        }
        
        // TODO: considering player height, distance to camera and all those things -> those values are currently hardcoded, find a solution to this
        // First thoughts:
        // 1) calibration at the beginning (restart) of the game to find the players joints and their normal position
        // 2) do some research on how this is typically done
        
        // Other concerns:
        // Trigger in game needs to have a certain delay for the player to react to the movement
        // Sven Liedtke's advice: place collider with an offset so the player is delayed but his movements are recognized as correct.
        
    }

    #region MyCurrentImplementation
    
    /// <summary>
    /// calibration sets the normal values (i.e.: idle position) of all the important joints
    ///
    /// this is triggered by a T-pose the player has to make before playing.
    /// 
    /// </summary>
    private void DoCalibration()
    {
        if (hasBeenCalibrated) return;

        if (TPoseCondition() && fillbar.fillAmount != 1)
        {
            fillbar.fillAmount += 0.2f * Time.deltaTime;
            if (fillbar.fillAmount == 1)
            {
                SetIdlePositionsAndCalibrationTrue();
                Debug.Log("Calibration done!");
                calibrationPanel.gameObject.SetActive(false);
                StartCoroutine(CalibrationSuccessfulTextFade(2));
            }
        }
        else
        {
            fillbar.fillAmount = 0;
        }
    }

    IEnumerator CalibrationSuccessfulTextFade(float duration)
    {
        calibrationSuccessfulText.gameObject.SetActive(true);

        var currentColor = calibrationSuccessfulText.color;
        
        while (calibrationSuccessfulText.color.a > 0)
        {
            currentColor.a -= Time.deltaTime / duration;
            calibrationSuccessfulText.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a);
            yield return null;
        }
    }
    
    /// <summary>
    /// 
    /// Checks the joint positions and returns true, if they are found to be in T-pose position and false otherwise.
    /// Also checks if the important joints for the detections are even detected by the camera, if one is missing
    /// function returns false
    /// TODO: one should not use absolute values here, as the distance makes everything relative.
    /// 
    /// </summary>
    /// <returns></returns>
    private bool TPoseCondition()
    {
        return ankleRight != null &&
               ankleLeft != null &&
               head != null &&
               spineMid != null && 
               handLeft != null &&
               handRight != null &&
               shoulderLeft != null &&
               shoulderRight != null && 
               elbowLeft != null && 
               elbowRight != null &&
               Math.Abs(elbowLeft.transform.position.y - shoulderLeft.transform.position.y) < 1f &&
               Math.Abs(elbowRight.transform.position.y - shoulderRight.transform.position.y) < 1f &&
               Math.Abs(handLeft.transform.position.y - shoulderLeft.transform.position.y) < 1f &&
               Math.Abs(handRight.transform.position.y - shoulderRight.transform.position.y) < 1f;
    }
    
    /// <summary>
    /// Sets the idle T-pose positions of the joints and 
    /// </summary>
    public void SetIdlePositionsAndCalibrationTrue()
    {
        idleAnkleRight = new GameObject("Idle_FootRight");
        idleAnkleRight.transform.position = ankleRight.transform.position;

        idleAnkleLeft = new GameObject("Idle_FootLeft");
        idleAnkleLeft.transform.position = ankleLeft.transform.position;

        idleHead = new GameObject("Idle_Head");
        idleHead.transform.position = head.transform.position;
            
        idleSpineMid = new GameObject("Idle_SpineMid");
        idleSpineMid.transform.position = spineMid.transform.position;

        idleHandLeft = new GameObject("Idle_HandLeft");
        idleHandLeft.transform.position = handLeft.transform.position;

        idleHandRight = new GameObject("Idle_HandRight");
        idleHandRight.transform.position = handRight.transform.position;

        idleShoulderLeft = new GameObject("Idle_ShoulderLeft");
        idleShoulderLeft.transform.position = shoulderLeft.transform.position;

        idleShoulderRight = new GameObject("Idle_ShoulderRight");
        idleShoulderRight.transform.position = shoulderRight.transform.position;

        idleElbowLeft = new GameObject("Idle_ElbowLeft");
        idleElbowLeft.transform.position = elbowLeft.transform.position;

        idleElbowRight = new GameObject("Idle_ElbowRight");
        idleElbowRight.transform.position = elbowRight.transform.position;
        
        hasBeenCalibrated = true;
        
    }
    
    /// <summary>
    /// 
    /// Detects a jump by comparing the left and right foot position to the idle position.
    /// If goes over a certain percentage (in this case 25%) of the overall height (head->leftAnkle/rightAnkle),
    /// then it counts as a jump.
    /// 
    /// Currently: Writes to console when a jump has been detected.
    /// 
    /// </summary>
    private void DetectJump()
    {
        // Detecting a jump
        if (ankleLeft != null && ankleRight != null)
        {
            bool leftFootUpPlusPercentage = ankleLeft.transform.position.y > idleAnkleLeft.transform.position.y +
                Vector3.Distance(idleHead.transform.position, idleAnkleLeft.transform.position) / 25;
            bool rightFootUpPlusPercentage = ankleRight.transform.position.y > idleAnkleRight.transform.position.y +
                Vector3.Distance(idleHead.transform.position, idleAnkleRight.transform.position) / 25;
            if (leftFootUpPlusPercentage && rightFootUpPlusPercentage) 
            {
                jumped.Value = true;
            }
        }
    }

    /// <summary>
    ///
    /// Detects a crouch by comparing the idle head position with the current head position.
    /// The players head needs to go at least to 60% of his body height (or lower) in order to detect a crouch.
    ///
    /// Currently: Writes to console when a crouch has been detected.
    /// 
    /// </summary>
    private void DetectCrouch()
    {
        // Detecting a crouch
        if (head != null)
        {
            if (Math.Abs(Vector3.Distance(idleHead.transform.position, head.transform.position)) >
                0.4f * Math.Abs(Vector3.Distance(idleHead.transform.position, ankleLeft.transform.position)))
            {
                crouched.Value = true;
            }
            /*
            if (head.transform.position.y < Vector3.Distance(idleHead.transform.position, ankleLeft.transform.position) * 0.5)
            {
                crouched.Value = true;
            }
            */
        }
    }

    /**
     * A righ step in the game is actually inverted, which means that a right step is a left step
     * (look from behind the person to make it the right way)
     *
     *  ==========================================================================================================================
     *  ==========================================================================================================================
     *  =======Here I am implementing the actual REAL WORLD right step, invert the figure in Unity so it fits the movement.=======
     *  ==========================================================================================================================
     *  ==========================================================================================================================
     */
    private void DetectRightStep()
    {
        if (head != null && ankleLeft != null && ankleRight != null && spineMid != null)
        {
            var bodyHeight = Vector3.Distance(idleHead.transform.position, idleAnkleLeft.transform.position); 
            
            // head normal position x value: around -1 to 1
            bool headRight = head.transform.position.x < idleHead.transform.position.x - bodyHeight*0.25f;

            // left foot normal position x value: around -2 and 0
            bool leftAnkle = ankleLeft.transform.position.x < idleAnkleLeft.transform.position.x - bodyHeight*0.25f;

            // right foot normal position x value: around 0 and 2
            bool rightAnkle = ankleRight.transform.position.x <
                              idleAnkleRight.transform.position.x - bodyHeight * 0.25f;
            // spine mid normal position x value: around -2 to 2
            bool midSpineRight = spineMid.transform.position.x < idleSpineMid.transform.position.x - bodyHeight*0.25f;

            if (headRight && leftAnkle && rightAnkle && midSpineRight)
            {
                Debug.Log("Right step has been detected!");
                movedMiddle.Value = false;
                movedRight.Value = true;
            }
        }
    }
    
    /**
     * A left step in the game is actually inverted, which means that a left step is a right step
     * (look from behind the person to make it the right way)
     *
     *  ==========================================================================================================================
     *  ==========================================================================================================================
     *  =======Here I am implementing the actual REAL WORLD left step, invert the figure in Unity so it fits the movement.=======
     *  ==========================================================================================================================
     *  ==========================================================================================================================
     * 
     */
    private void DetectLeftStep()
    {
        if (head != null && ankleLeft != null && ankleRight != null && spineMid != null)
        {
            var bodyHeight = Vector3.Distance(idleHead.transform.position, idleAnkleLeft.transform.position);
            
            // head normal position x value: around -1 to 1
            bool headRight = head.transform.position.x > idleHead.transform.position.x + bodyHeight*0.25f;

            // left foot normal position x value: around -2 and 0
            bool leftFoot = ankleLeft.transform.position.x > idleAnkleLeft.transform.position.x + bodyHeight*0.25f;

            // right foot normal position x value: around 0 and 2
            bool rightFoot = ankleRight.transform.position.x > idleAnkleRight.transform.position.x + bodyHeight * 0.25f;

            // spine mid normal position x value: around -2 to 2
            bool midSpine = spineMid.transform.position.x > idleSpineMid.transform.position.x + bodyHeight*0.25f;

            if (headRight && leftFoot && rightFoot && midSpine)
            {
                Debug.Log("Left step has been detected!");
                movedLeft.Value = true;
                movedMiddle.Value = false;
            }
        }
    }

    private void DetectMoveToMiddle()
    {
        if (head != null && ankleLeft != null && ankleRight != null && spineMid != null)
        {
            var bodyHeight = Vector3.Distance(idleHead.transform.position, idleAnkleLeft.transform.position); 
            
            // head normal position x value: around -1 to 1
            bool headMiddle = Vector3.Distance(head.transform.position, idleHead.transform.position) <
                              0.05f * bodyHeight;

            // left foot normal position x value: around -2 and 0
            bool leftAnkle = Vector3.Distance(ankleLeft.transform.position, idleAnkleLeft.transform.position) <
                             0.05f * bodyHeight;
            
            // right foot normal position x value: around 0 and 2
            bool rightAnkle = Vector3.Distance(ankleRight.transform.position, idleAnkleRight.transform.position) <
                              0.05f * bodyHeight;
            
            // spine mid normal position x value: around -2 to 2
            bool midSpineMiddle = Vector3.Distance(spineMid.transform.position, idleSpineMid.transform.position) <
                                  0.05f * bodyHeight;

            if (headMiddle && leftAnkle && rightAnkle && midSpineMiddle)
            {
                Debug.Log("Move to middle has been detected!");
                movedMiddle.Value = true;
            }
        }
    }
    
    #endregion
    
    // foot needed for jumps, head needed for crouch
    public GameObject ankleRight, ankleLeft, head, spineMid, handLeft, handRight, shoulderLeft, shoulderRight, elbowLeft, elbowRight;
    public GameObject idleAnkleRight, idleAnkleLeft, idleHead, idleSpineMid, idleHandLeft, idleHandRight, idleShoulderLeft, idleShoulderRight, idleElbowLeft, idleElbowRight;

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            switch (jt)
            {
                case Kinect.JointType.AnkleLeft:
                    ankleLeft = jointObj;
                    break;
                case Kinect.JointType.AnkleRight:
                    ankleRight = jointObj;
                    break;
                case Kinect.JointType.Head:
                    head = jointObj;
                    break;
                case Kinect.JointType.SpineMid:
                    spineMid = jointObj;
                    break;
                case Kinect.JointType.HandLeft:
                    handLeft = jointObj;
                    break;
                case Kinect.JointType.HandRight:
                    handRight = jointObj;
                    break;
                case Kinect.JointType.ShoulderLeft:
                    shoulderLeft = jointObj;
                    break;
                case Kinect.JointType.ShoulderRight:
                    shoulderRight = jointObj;
                    break;
                case Kinect.JointType.ElbowLeft:
                    elbowLeft = jointObj;
                    break;
                case Kinect.JointType.ElbowRight:
                    elbowRight = jointObj;
                    break;
            }

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
