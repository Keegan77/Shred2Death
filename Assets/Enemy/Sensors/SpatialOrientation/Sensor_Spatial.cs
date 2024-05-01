using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// Spatial sensors are systems that can point away from imminent objects provided a given direciton.
/// Their information is updated via Ping(), which requires target to be set ahead of time.
/// </summary>
public class Sensor_Spatial : Sensor
{
    [Header ("Spatial Sensor")]
    public LayerMask maskRaycast;

    [Range (2, 5)]
    [Tooltip ("How many points for each face will this sensor be made with?")]
    [SerializeField] int sensorResolution = 3;

    [Min(1)]
    [Tooltip ("How far will the sensors look for obstacles?")]
    public float sensorLength = 1;

    [Tooltip ("How wide is a forward facing ping? (used in movement coroutines)"), Min(0.1f)]
    public float sensorWidth = 1;

    [Min (0)]
    [Tooltip("How hard do the sensors push away from obstacles they run into? Resulting push is based on the sensor's length and how far away it hits a wall.")]
    public float sensorStrength = 1;

    [Header("Output")]
    public Vector3 pingResult = Vector3.zero;

    #region SETUP
    private void Awake()
    {
        constructSensors();
    }

    private void Start()
    {
    }

    /// <summary>
    /// Construct a net of cubes around the sensor
    /// </summary>
    private void constructSensors()
    {
        for (int b = 0; b < sensorResolution; b++) // For each band
        {
            //Instantiate(new GameObject($"SensorBlock_{b}"), transform);
            GameObject o = new GameObject($"SensorBlock_{b}");

            o.transform.position = transform.position;
            o.transform.parent = transform;

            for (int r = 0; r < sensorResolution; r++) //for each row
            {
                for (int c = 0; c < sensorResolution; c++) //for each column
                {
                    //If this position is on the outside of the net, spawn the sensor
                    if ((r == 0 || r == sensorResolution - 1) || (c == 0 || c == sensorResolution - 1) || (b == 0 || b == sensorResolution - 1))
                    {
                        float offset = (float)sensorResolution / (2 * sensorResolution);

                        Vector3 sensorPosition = new Vector3
                            (
                            (float) sensorResolution / 2 - c - offset,
                            (float) sensorResolution / 2 - r - offset,
                            (float) sensorResolution / 2 - b - offset
                            );

                        GameObject sensor = new GameObject($"Sensor {b}_{r}_{c}", typeof(Sensor_Raycast));
                        sensor.GetComponent<Sensor_Raycast>().raycastLength = sensorLength;
                        sensor.GetComponent<Sensor_Raycast>().maskRaycast = maskRaycast;

                        sensor.transform.parent = o.transform;
                        sensor.transform.localPosition = sensorPosition;
                        
                    }
                }
            }
        }
    }
    #endregion


    /// <summary>
    /// Pinging a spatial sensor will have it go through its raycast blocks.
    /// It will update a Vector3 value that will point away from all sensors that were tripped by something.
    /// </summary>
    /// <returns>True if any one of the sensors have been tripped</returns>
    public override bool Ping()
    {
        return updateSpatialSensor (false) == Vector3.zero ? false : true;
    }

    [ContextMenu("Ping Sensor")]
    public void buttonPing()
    {
        Ping();
        Debug.Break();
    }

    /// <summary>
    /// Sends out teh 
    /// </summary>
    /// <param name="useFull">If true, use the back half of the array in the calculation</param>
    /// <returns>The avoidance value of the sensor after updating it</returns>
    public Vector3 updateSpatialSensor (bool useFull = false)
    {
        pingResult = Vector3.zero;

        //for each band of the sensor
        for (int b = 0; b < (useFull ? sensorResolution : Mathf.Ceil ((float)sensorResolution / 2)); b++)
        {
            //Debug.Log (transform.GetChild(b).name);
            foreach (Sensor_Raycast sense in transform.GetChild(b).GetComponentsInChildren<Sensor_Raycast> ())
            {
                //Debug.Log (sense.name);
                if (sense.Ping ())
                {

                    Vector3 hitResult = (sense.hit.point - sense.transform.position);
                    hitResult = hitResult.normalized * ((1 - hitResult.magnitude / sensorLength) * sensorStrength);
                    
                    //pingResult -= sense.transform.position - transform.position;
                    pingResult -= hitResult;

                    Debug.DrawLine (
                    sense.transform.position,
                    hitResult + sense.transform.position,
                    Color.yellow
                    );
                }

            }
        }


        Debug.DrawLine (transform.position, transform.position + pingResult, Color.green);

        return pingResult;
    }


}
