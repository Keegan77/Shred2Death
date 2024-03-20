using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderRef : MonoBehaviour
{
    public Slider sensitivitySlider;

    private void Awake()
    {
        sensitivitySlider = GetComponent<Slider>();
    }
}
