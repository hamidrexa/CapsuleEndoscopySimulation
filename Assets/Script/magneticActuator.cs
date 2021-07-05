using System;
using UnityEngine;
using Unity.MLAgents;
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
    private ExternalFieldSolenoidScript Fieldcontroller1;
    private ExternalFieldSolenoidScript Fieldcontroller2;
    private ExternalFieldSolenoidScript Fieldcontroller3;
    private ExternalFieldSolenoidScript Fieldcontroller4;

    [Header("magnetic ball")]
    public GameObject ball;
    Rigidbody m_BallRb;

    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        
        controller = electromagneticFieldController.GetComponent<ElectromagneticFieldControllerScript>();
        // Debug.Log("MagneticField="+controller.MagneticField(ball.transform.position));

        Fieldcontroller1 = SolenoidField1.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller2 = SolenoidField2.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller3 = SolenoidField3.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller4 = SolenoidField4.GetComponent<ExternalFieldSolenoidScript>();

        m_BallRb = ball.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // sensor.AddObservation(controller.MagneticField(ball.transform.position));
        sensor.AddObservation(ball.transform.position.x);
        sensor.AddObservation(ball.transform.position.z);
        sensor.AddObservation(m_BallRb.velocity.magnitude);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        int alpha = 100;

        int SolenoidFieldStrength1 = 0;
        int SolenoidFieldStrength2 = 0;
        int SolenoidFieldStrength3 = 0;
        int SolenoidFieldStrength4 = 0;

        int SolenoidFieldAction1 = Mathf.FloorToInt(vectorAction[0]);
        int SolenoidFieldAction2 = Mathf.FloorToInt(vectorAction[1]);
        int SolenoidFieldAction3 = Mathf.FloorToInt(vectorAction[2]);
        int SolenoidFieldAction4 = Mathf.FloorToInt(vectorAction[3]);

        if (SolenoidFieldAction1 == 0) {  SolenoidFieldStrength1 = -1; }
        if (SolenoidFieldAction1 == 1) {  SolenoidFieldStrength1 = 1; }
        if (SolenoidFieldAction1 == 2) {  SolenoidFieldStrength1 = 0; }

        if (SolenoidFieldAction2 == 0) {  SolenoidFieldStrength2 = -1; }
        if (SolenoidFieldAction2 == 1) {  SolenoidFieldStrength2 = 1; }
        if (SolenoidFieldAction2 == 2) {  SolenoidFieldStrength2 = 0; }

        if (SolenoidFieldAction3 == 0) {  SolenoidFieldStrength3 = -1; }
        if (SolenoidFieldAction3 == 1) {  SolenoidFieldStrength3 = 1; }
        if (SolenoidFieldAction3 == 2) {  SolenoidFieldStrength3 = 0; }

        if (SolenoidFieldAction4 == 0) {  SolenoidFieldStrength4 = -1; }
        if (SolenoidFieldAction4 == 1) {  SolenoidFieldStrength4 = 1; }
        if (SolenoidFieldAction4 == 2) {  SolenoidFieldStrength4 = 0; }

        Fieldcontroller1.strength += alpha * SolenoidFieldStrength1;
        Fieldcontroller2.strength += alpha * SolenoidFieldStrength2;
        Fieldcontroller3.strength += alpha * SolenoidFieldStrength3;
        Fieldcontroller4.strength += alpha * SolenoidFieldStrength4;

        // Debug.Log("Act1="+vectorAction[0] +
        //         "  ;  Act2="+vectorAction[1] + 
        //         "  ;  Act3="+vectorAction[2] + 
        //         "  ;  Act4="+vectorAction[3]);

        if( Mathf.Abs(ball.transform.position.x) > 1f ||
            Mathf.Abs(ball.transform.position.z) > 1f)
        {
            Debug.Log("END  Field_S1="+Fieldcontroller1.strength +
            " ; Field_S2="+Fieldcontroller2.strength + 
            " ; Field_S3="+Fieldcontroller3.strength + 
            " ; Field_S4="+Fieldcontroller4.strength);

            SetReward(-1f);
            EndEpisode();
        }
        else if( Mathf.Abs(ball.transform.position.x) < 0.2f ||
                 Mathf.Abs(ball.transform.position.z) < 0.2f)
        {
            SetReward(0.5f);
        }
    }

    public override void OnEpisodeBegin()
    {
        Fieldcontroller1.strength = 10000 + UnityEngine.Random.Range(-1000f, 1000f);
        Fieldcontroller2.strength = 10000 + UnityEngine.Random.Range(-1000f, 1000f);
        Fieldcontroller3.strength = 10000 + UnityEngine.Random.Range(-1000f, 1000f);
        Fieldcontroller4.strength = 10000 + UnityEngine.Random.Range(-1000f, 1000f);
        Debug.Log("START  Field_S1="+Fieldcontroller1.strength +
        " ; Field_S2="+Fieldcontroller2.strength + 
        " ; Field_S3="+Fieldcontroller3.strength + 
        " ; Field_S4="+Fieldcontroller4.strength);

        ball.transform.localPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0.1f, UnityEngine.Random.Range(-1f, 1f));
        // Debug.Log("ball.localPosition="+ball.transform.localPosition);

        SetResetParameters();
    }

    // public override void Heuristic(float[] actionsOut)
    // {
    //     if (Input.GetAxis("Vertical")>0)
    //         actionsOut[0] = 2;
    //     else if (Input.GetAxis("Vertical")<=0)
    //         actionsOut[1] = 2;
    //     if (Input.GetAxis("Horizontal")>0)
    //         actionsOut[2] = 2;
    //     else if (Input.GetAxis("Horizontal")<=0)
    //         actionsOut[3] = 2;
    // }

    public void SetBall()
    {
        //Set the attributes of the ball by fetching the information from the academy
        m_BallRb.mass = m_ResetParams.GetWithDefault("mass", 4.0f);
        // var scale = m_ResetParams.GetWithDefault("scale", 0.2f);
        // ball.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetResetParameters()
    {
        SetBall();
    }
}
