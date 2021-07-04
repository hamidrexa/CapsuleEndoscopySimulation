using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class magneticActuator : Agent
{
    [Header("Solenoids' Field to control")]
    public GameObject SolenoidField1;
    public GameObject SolenoidField2;
    public GameObject SolenoidField3;
    public GameObject SolenoidField4;
    private ExternalFieldSolenoidScript Fieldcontroller1;
    private ExternalFieldSolenoidScript Fieldcontroller2;
    private ExternalFieldSolenoidScript Fieldcontroller3;
    private ExternalFieldSolenoidScript Fieldcontroller4;
    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        Fieldcontroller1 = SolenoidField1.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller2 = SolenoidField2.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller3 = SolenoidField3.GetComponent<ExternalFieldSolenoidScript>();
        Fieldcontroller4 = SolenoidField4.GetComponent<ExternalFieldSolenoidScript>();
        Debug.Log("SolenoidField1.strength="+controller.strength);

        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

        public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameObject.transform.rotation.z);
        sensor.AddObservation(gameObject.transform.rotation.x);
        sensor.AddObservation(ball.transform.position - gameObject.transform.position);
        sensor.AddObservation(m_BallRb.velocity);
    }
}
