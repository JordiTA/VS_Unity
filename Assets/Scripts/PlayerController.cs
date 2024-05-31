using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //CAR CARACTERISTICS
    private int maxSpeed = 220;
    private int maxReverseSpeed = 45;

    private int accelerationMultiplier = 15;

    private int maxSteeringAngle = 35;
    private float steeringSpeed = 0.5f;

    private int brakeForce = 450;

    private int decelerationMultiplier = 1;

    private int handbrakeDriftMultiplier = 5;

    private Vector3 bodyMassCenter;

    //RUEDAS
    [Header("Wheels MESHES & COLLIDERS")]
    [Space(10)]
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    [Space(10)]
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    [Space(10)]
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    [Space(10)]
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

    //UI
    [Space(20)]
    [Header("UI")]
    [Space(10)]
    public TextMeshProUGUI carSpeedText;
    [Space(10)]
    public TextMeshProUGUI scoreText;

    private float carScore;
    private float carTotalScore;

    //PS
    [Space(20)]
    [Header("Particle Systems")]
    [Space(10)]
    public ParticleSystem LeftTireSmoke;
    public ParticleSystem RightTireSmoke;
    [Space(10)]
    public TrailRenderer LeftTireMark;
    public TrailRenderer RightTireMark;

    //AUDIO
    [Space(20)]
    [Header("AUDIO")]
    [Space(10)]
    public AudioSource carEngineSound;
    public AudioSource tireDriftSound;
    private float carEngineSoundPitch;


    //VARIABLES PRIVATE
    private float carSpeed;
    //No deja ponerlas private sino no funciona idk why
    [HideInInspector]
    public bool isDrifting;
    [HideInInspector]
    public bool isTractionLocked;

    private Rigidbody rbCar;

    private float steeringAxis;
    private float throttleAxis;
    private float driftingAxis;
    private float localVelocityZ;
    private float localVelocityX;
    private bool deceleratingCar;

    private WheelFrictionCurve FLwheelFriction;
    private float FLWextremumSlip;
    private WheelFrictionCurve FRwheelFriction;
    private float FRWextremumSlip;
    private WheelFrictionCurve RLwheelFriction;
    private float RLWextremumSlip;
    private WheelFrictionCurve RRwheelFriction;
    private float RRWextremumSlip;

    // Start is called before the first frame update
    void Start()
    {
        rbCar = gameObject.GetComponent<Rigidbody>();
        rbCar.centerOfMass = bodyMassCenter;

        FLwheelFriction = new WheelFrictionCurve();
        FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
        FRwheelFriction = new WheelFrictionCurve();
        FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
        RLwheelFriction = new WheelFrictionCurve();
        RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
        RRwheelFriction = new WheelFrictionCurve();
        RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        //UI
        InvokeRepeating("CarUI", 0f, 0.1f);

        //PARTICLE SYSTEM
        if (LeftTireSmoke != null && LeftTireSmoke.isPlaying)
        {
            LeftTireSmoke.Stop();
        }
        if (RightTireSmoke != null && RightTireSmoke.isPlaying)
        {
            RightTireSmoke.Stop();
        }
        if (LeftTireMark != null)
        {
            LeftTireMark.emitting = false;
        }
        if (RightTireMark != null)
        {
            RightTireMark.emitting = false;
        }

        //AUDIO
        InvokeRepeating("CarSounds", 0f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        SetUpCar();

        if (InputManager._INPUT_MANAGER.GetForwardButton())
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            MoveForward();
        }

        if (InputManager._INPUT_MANAGER.GetReverseButton())
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            MoveReverse();
        }

        if (InputManager._INPUT_MANAGER.GetTurnLeftButton())
        {
            TurnLeft();
        }

        if (InputManager._INPUT_MANAGER.GetTurnRightButton())
        {
            TurnRight();
        }

        if (InputManager._INPUT_MANAGER.GetHandbrakeButton())
        {
            CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            Handbrake();
        }
        else
        {
            RecoverTraction();
        }

        if (!InputManager._INPUT_MANAGER.GetReverseButton() && !InputManager._INPUT_MANAGER.GetForwardButton())
        {
            ThrottleOff();
        }

        if (!InputManager._INPUT_MANAGER.GetForwardButton() && !InputManager._INPUT_MANAGER.GetReverseButton() && !InputManager._INPUT_MANAGER.GetHandbrakeButton() && !deceleratingCar)
        {
            InvokeRepeating("DecelerateCar", 0f, 0.1f);
            deceleratingCar = true;
        }
        if (!InputManager._INPUT_MANAGER.GetTurnLeftButton() && !InputManager._INPUT_MANAGER.GetTurnRightButton())
        {
            ResetSteeringAngle();
        }
        if (isDrifting)
        {
            carScore += Time.deltaTime;
        }
        else
        {
            carScore = 0f;
        }
        AnimateWheelMeshes();
    }

    private void SetUpCar()
    {
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;

        localVelocityX = transform.InverseTransformDirection(rbCar.velocity).x;

        localVelocityZ = transform.InverseTransformDirection(rbCar.velocity).z;
    }
    #region Movement
    //FORWARD
    private void MoveForward()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        throttleAxis = throttleAxis + (Time.deltaTime * 3f);
        if (throttleAxis > 1f)
        {
            throttleAxis = 1f;
        }

        if (localVelocityZ < -1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.RoundToInt(carSpeed) < maxSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }
    //REVERSE
    private void MoveReverse()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }
        throttleAxis = throttleAxis - (Time.deltaTime * 3f);
        if (throttleAxis < -1f)
        {
            throttleAxis = -1f;
        }

        if (localVelocityZ > 1f)
        {
            Brakes();
        }
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }
    //TURN LEFT
    private void TurnLeft()
    {
        steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis < -1f)
        {
            steeringAxis = -1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }
    //TURN RIGHT
    private void TurnRight()
    {
        steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        if (steeringAxis > 1f)
        {
            steeringAxis = 1f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }
    //HANDBRAKE
    private void Handbrake()
    {
        CancelInvoke("RecoverTraction");

        driftingAxis = driftingAxis + (Time.deltaTime);
        float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

        if (secureStartingPoint < FLWextremumSlip)
        {
            driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
        }
        if (driftingAxis > 1f)
        {
            driftingAxis = 1f;
        }

        if (Mathf.Abs(localVelocityX) > 2.0f)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        if (driftingAxis < 1f)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;
        }

        isTractionLocked = true;
        DriftCarPS();
    }
    //CUANDO DEJAMOS DE USAR HANDBRAKE
    private void RecoverTraction()
    {
        isTractionLocked = false;
        driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
        if (driftingAxis < 0f)
        {
            driftingAxis = 0f;
        }
        if (FLwheelFriction.extremumSlip > FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            Invoke("RecoverTraction", Time.deltaTime);

        }
        else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            driftingAxis = 0f;
        }
    }
    //CUANDO NO USAMOS NI FORWARD NI REVERSE:
    private void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }
    //CUANDO NO GIRAMOS HACIA NINGUNA DIRECCION:
    private void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis = steeringAxis + (Time.deltaTime * 10f * steeringSpeed);
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis = steeringAxis - (Time.deltaTime * 10f * steeringSpeed);
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        var steeringAngle = steeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    //CUANDO NO PULSAMOS NI FORWARD NI REVERSE NI HANDBRAKE:
    private void DecelerateCar()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            isDrifting = true;
            DriftCarPS();
        }
        else
        {
            isDrifting = false;
            DriftCarPS();
        }

        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }
        rbCar.velocity = rbCar.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));

        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
        // Si la magnitud de la velocidad es menor que 0.25f (very slow velocity), el coche se parara por completo
        if (rbCar.velocity.magnitude < 0.25f)
        {
            rbCar.velocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }
    #endregion
    #region Brakes
    private void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }
    #endregion
    #region Particle System
    private void DriftCarPS()
    {
        try
        {
            if (isDrifting && !LeftTireSmoke.isPlaying && !RightTireSmoke.isPlaying)
            {
                LeftTireSmoke.Play();
                RightTireSmoke.Play();
            }
            else if (!isDrifting && LeftTireSmoke.isPlaying && RightTireSmoke.isPlaying)
            {
                LeftTireSmoke.Stop();
                RightTireSmoke.Stop();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }

        try
        {
            if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
            {
                LeftTireMark.emitting = true;
                RightTireMark.emitting = true;
            }
            else
            {
                LeftTireMark.emitting = false;
                RightTireMark.emitting = false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }
    #endregion
    #region Audio
    private void CarSounds()
    {
        try
        {
            if (carEngineSound != null)
            {
                float engineSoundPitch = carEngineSoundPitch + (Mathf.Abs(rbCar.velocity.magnitude) / 25f);
                carEngineSound.pitch = engineSoundPitch;
            }
            if ((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f))
            {
                if (!tireDriftSound.isPlaying)
                {
                    tireDriftSound.Play();
                }
            }
            else if ((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f))
            {
                tireDriftSound.Stop();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }
    #endregion
    #region Wheel Animation
    private void AnimateWheelMeshes()
    {
        try
        {
            Quaternion FLWRotation;
            Vector3 FLWPosition;
            frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
            frontLeftMesh.transform.position = FLWPosition;
            frontLeftMesh.transform.rotation = FLWRotation;

            Quaternion FRWRotation;
            Vector3 FRWPosition;
            frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
            frontRightMesh.transform.position = FRWPosition;
            frontRightMesh.transform.rotation = FRWRotation;

            Quaternion RLWRotation;
            Vector3 RLWPosition;
            rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
            rearLeftMesh.transform.position = RLWPosition;
            rearLeftMesh.transform.rotation = RLWRotation;

            Quaternion RRWRotation;
            Vector3 RRWPosition;
            rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
            rearRightMesh.transform.position = RRWPosition;
            rearRightMesh.transform.rotation = RRWRotation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }
    #endregion
    #region UI
    public void CarUI()
    {
        try
        {
            float absoluteCarSpeed = Mathf.Abs(carSpeed);
            carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
        try
        {
            carTotalScore += Mathf.Abs(carScore);
            scoreText.text = Mathf.RoundToInt(carTotalScore).ToString();
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }
    #endregion
    #region Collisions
    private void OnCollisionEnter(Collision collision)
    {
        
    }
    private void OnTriggerEnter(Collider collision){
        if (collision.gameObject.tag == "Starter"){
            EventManager.onTimerStart();
        }
        if (collision.gameObject.tag == "Finisher"){
            EventManager.onTimerStop();
            int scoreInt = int.Parse(scoreText.text);
            MenuController._MENUCONTROLLER.SaveHighScore(scoreInt);
        }
        if (collision.gameObject.tag == "FinisherPractice"){
            SceneManager.LoadScene("PracticeRace");
        }
    }
    #endregion
}