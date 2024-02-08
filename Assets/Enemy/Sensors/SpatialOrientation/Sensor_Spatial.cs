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
    public GameObject target;

    [Header("Spatial Sensor")]


    [Range(2, 5)]
    [Tooltip("How many points for each face will this sensor be made with?")]
    [SerializeField] int netResolution = 3;

    [Range(1, 10)] //10 is way overkill but eh
    [Tooltip("How far will the sensors look for obstacles?")]
    [SerializeField] float sensorLength = 1;


    #region SETUP
    private void Awake()
    {
        constructSensors();
    }

    /// <summary>
    /// Construct a net of cubes around the sensor
    /// </summary>
    private void constructSensors()
    {
        for (int b = 0; b < netResolution; b++) // For each band
        {
            //Instantiate(new GameObject($"SensorBlock_{b}"), transform);
            GameObject o = new GameObject($"SensorBlock_{b}");

            o.transform.position = transform.position;
            o.transform.parent = transform;

            for (int r = 0; r < netResolution; r++) //for each row
            {
                for (int c = 0; c < netResolution; c++) //for each column
                {
                    //If this position is on the outside of the net, spawn the sensor
                    if ((r == 0 || r == netResolution - 1) || (c == 0 || c == netResolution - 1) || (b == 0 || b == netResolution - 1))
                    {
                        float offset = (float)netResolution / (2 * netResolution);

                        Vector3 sensorPosition = new Vector3
                            (
                            (float) netResolution / 2 - c - offset,
                            (float) netResolution / 2 - r - offset,
                            (float) netResolution / 2 - b - offset
                            );

                        GameObject sensor = new GameObject($"Sensor {b}_{r}_{c}", typeof(Sensor_Raycast));
                        sensor.GetComponent<Sensor_Raycast>().raycastLength = sensorLength;

                        sensor.transform.parent = o.transform;
                        sensor.transform.localPosition = sensorPosition;
                    }
                }
            }
        }
    }
    #endregion



    public override bool Ping()
    {
        return false;
    }

    //bool checkSensorBlock()
    //{

    //}
}
