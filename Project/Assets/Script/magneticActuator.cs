using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static System.Math;

public class magneticActuator : Agent
{
    [Header("electromagneticFieldController")]
    public GameObject electromagneticFieldController;
    private ElectromagneticFieldControllerScript controller;

    [Header("Solenoids' Field to control")]
    public GameObject SolenoidField1;
    public GameObject SolenoidField2;
    public GameObject SolenoidField3;
    public GameObject SolenoidField4;
    ExternalFieldSolenoidScript Fieldcontroller1;
    ExternalFieldSolenoidScript Fieldcontroller2;
    ExternalFieldSolenoidScript Fieldcontroller3;
    ExternalFieldSolenoidScript Fieldcontroller4;

    [Header("Magnetic Capsule")]
    public GameObject Capsule;
    public Material towardMaterial;
    Material[] capsuleMaterial;
    Rigidbody m_CapsuleRb;
    MeshRenderer m_CapsuleMr;
    // Transform m_CapsuleTr;

    EnvironmentParameters m_ResetParams;

    [Header("Target")]
    public Transform target;

    public override void Initialize()
    {
        
        controller = electromagneticFieldController.GetComponent<ElectromagneticFieldControllerScript>();
        // Debug.Log("MagneticField="+controller.MagneticField(Capsule.transform.position));

        Fieldcontroller1 = SolenoidField1.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller2 = SolenoidField2.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller3 = SolenoidField3.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller4 = SolenoidField4.GetComponent<ExternalFieldSolenoidScript>();

        m_CapsuleRb = Capsule.GetComponent<Rigidbody>();
        m_CapsuleMr = Capsule.GetComponent<MeshRenderer>();
        capsuleMaterial = m_CapsuleMr.materials;
        // m_CapsuleTr = Capsule.GetComponent<Transform>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;

        // m_CapsuleRb.mass = m_ResetParams.GetWithDefault("mass", 1.0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition-Capsule.transform.localPosition);
        sensor.AddObservation(Capsule.transform.localPosition.x);
        sensor.AddObservation(Capsule.transform.localPosition.z);
        // sensor.AddObservation(SolenoidField3.transform.parent.localPosition-Capsule.transform.localPosition);

        sensor.AddObservation(Capsule.transform.localRotation); // dim=4 ; Quaternion 

        Vector3 m_DirToTarget = target.position - m_CapsuleRb.position; //Capsule.transform.localPosition
        float m_MovingTowardsDot = Vector3.Dot(m_CapsuleRb.velocity, m_DirToTarget.normalized);

        sensor.AddObservation(m_MovingTowardsDot);
        sensor.AddObservation(m_CapsuleRb.velocity);
        // sensor.AddObservation(m_CapsuleRb.angularVelocity);

        sensor.AddObservation(Fieldcontroller1.strength);
        sensor.AddObservation(Fieldcontroller2.strength);
        sensor.AddObservation(Fieldcontroller3.strength);
        sensor.AddObservation(Fieldcontroller4.strength);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // vectorAction is between -1 & 1
        // if( distanceToTarget > 0.05f ) {
            Fieldcontroller1.strength +=  10000f * actionBuffers.ContinuousActions[0]; 
            Fieldcontroller2.strength +=  10000f * actionBuffers.ContinuousActions[1];
            Fieldcontroller3.strength +=  10000f * actionBuffers.ContinuousActions[2];
            Fieldcontroller4.strength +=  10000f * actionBuffers.ContinuousActions[3];
        // }
    }

    void FixedUpdate()
    {
        float distanceToTarget = Vector3.Distance(Capsule.transform.localPosition, target.localPosition);

        if( distanceToTarget < 0.1f ) {
            if (Mathf.Abs(m_CapsuleRb.velocity.magnitude) > 0.02f)
                AddReward(0.01f);
            else {
                AddReward(3.0f);
                // Debug.Log(" =>  m_CapsuleRb.velocity.magnitude = " + m_CapsuleRb.velocity.magnitude + 
                // // " ;  mag = " + new Vector2(Capsule.transform.localPosition.x, Capsule.transform.localPosition.z).magnitude + 
                // // " ;  z = " + Capsule.transform.localPosition.z +
                // // " ;  x = " + Capsule.transform.localPosition.x +
                // " ; Field_S1= " + Fieldcontroller1.strength +
                // " ; Field_S2= " + Fieldcontroller2.strength + 
                // " ; Field_S3= " + Fieldcontroller3.strength + 
                // " ; Field_S4= " + Fieldcontroller4.strength );
                EndEpisode();
            }
            // if( Capsule.transform.parent.name == "GameObject (10)"  )
            //     Debug.Log(" =>  distanceToTarget = " + distanceToTarget +
            //         " ; Field_S1= " + Fieldcontroller1.strength +
            //         " ; Field_S2= " + Fieldcontroller2.strength + 
            //         " ; Field_S3= " + Fieldcontroller3.strength + 
            //         " ; Field_S4= " + Fieldcontroller4.strength +
            //         " ; CumulativeReward= " + GetCumulativeReward());
            EndEpisode();
        } 
        else if( distanceToTarget > 0.1f )
            AddReward(-distanceToTarget * 0.001f);
            // AddReward(0.00005f/distanceToTarget);
        // else if( distanceToTarget < 0.7f ) {
        //     AddReward(0.001f/distanceToTarget);
        //     // if( Capsule.transform.parent.name == "GameObject (10)"  )
        //     //     Debug.Log(" +  AddReward = " + 0.001f/distanceToTarget );
        // }
        // else if( distanceToTarget >= 0.7f ) {
        //     AddReward(-distanceToTarget/100);
        //     // Debug.Log(" -  AddReward = " + distanceToTarget/100 );
        // }

        RewardFunctionMovingTowards();

        if( Mathf.Abs(Capsule.transform.localPosition.x) > 0.9f ||
            Mathf.Abs(Capsule.transform.localPosition.z) > 0.9f ) {
            AddReward(-2.0f);
            EndEpisode();
        }
       
        if( Capsule.transform.parent.name == "GameObject (10)" )
            Debug.Log(" =>  distance REWARD = " + -distanceToTarget * 0.001f +
                " ; CumulativeReward= " + GetCumulativeReward());
        
        // Debug.Log(" =>  Capsule.transform.localPosition.x = " + Capsule.transform.localPosition +
        //     // " ; SolenoidField1.transform.parent.localPosition = " + SolenoidField1.transform.parent.localPosition +
        //     //     " ; m_CapsuleTr.position.x = " + m_CapsuleTr.position.x + 
        //     //     " ; m_CapsuleTr.localPosition.x = " + m_CapsuleTr.localPosition.x + 
        //     //     " ; m_CapsuleRb.position.x = " + m_CapsuleRb.position.x + 
        //         // " ; m_CapsuleRb.localPosition.x = " + m_CapsuleRb.localPosition.x + 
        //         " ; CumulativeReward= " + GetCumulativeReward());


    }

    void RewardFunctionMovingTowards()
    {
        Vector3 m_DirToTarget = target.position - m_CapsuleRb.position; //Capsule.transform.localPosition
        float m_MovingTowardsDot = Vector3.Dot(m_CapsuleRb.velocity, m_DirToTarget.normalized);
        AddReward(0.1f * m_MovingTowardsDot);

        var materials = m_CapsuleMr.materials;
        materials[1] = (m_MovingTowardsDot>0) ? towardMaterial : capsuleMaterial[1];
        m_CapsuleMr.materials = materials;

        if( Capsule.transform.parent.name == "GameObject (10)"  )
            Debug.Log(" =>  Reward Towards = " + 0.1f * m_MovingTowardsDot );
        
    }

    public override void OnEpisodeBegin()
    {
        Fieldcontroller1.strength = UnityEngine.Random.Range(-1000f, 1000f);
        Fieldcontroller2.strength = UnityEngine.Random.Range(-1000f, 1000f) ;
        Fieldcontroller3.strength = UnityEngine.Random.Range(-1000f, 1000f) ;
        Fieldcontroller4.strength = UnityEngine.Random.Range(-1000f, 1000f) ;

        Capsule.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.65f, 0.65f), 0.1f, UnityEngine.Random.Range(-0.65f, 0.65f));
        Capsule.transform.Rotate(new Vector3(1, 0, 1), UnityEngine.Random.Range(0f, 360f));

        target.localPosition = new Vector3(UnityEngine.Random.Range(-0.65f, 0.65f), 0.001f, UnityEngine.Random.Range(-0.65f, 0.65f));

        // m_CapsuleRb.mass = m_ResetParams.GetWithDefault("mass", 1.0f);

        // Debug.Log("  START ======>  rot.x = " + Capsule.transform.localRotation.x + 
        //         " ;  z = " + Capsule.transform.localPosition.z + 
        //         " ;  x = " + Capsule.transform.localPosition.x +
        //         " ;  y = " + Capsule.transform.localPosition.y );
    }

}
