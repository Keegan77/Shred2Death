using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamteck.Splines.Primitives;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class BounceUI : MonoBehaviour
{
    SpringUtils.tDampedSpringMotionParams springParams;
    
    private Vector2 referenceResolution = new Vector2(1920, 1080);

    [Header("Spring Settings")]
    [SerializeField] private float frequency;
    [SerializeField] private float dampingRatio;
    
    [Header("Transformation Settings")]
    [Tooltip("Will be used as the starting value if dontUseStartValue is not checked.")]
    [SerializeField] private Vector3 startValue;
    [FormerlySerializedAs("dontUseStartValue")]
    [Tooltip("if checked will use the starting scene position of the object instead of the inputted start value.")]
    [SerializeField] bool useSceneStartValueInstead;
    [SerializeField] Vector3 targetLocalDisplacement;
    
    private Vector3 homeValues;
    
    private Vector3 currentTransformValue;
    private Vector3 targetTransformValue; //used as the current value to spring towards
    
    private Vector3 endingTransformValue; //used to hold the end value, calculated as start value + displacement
    private Vector3 vel;

    public bool isMask;
    private bool disableHover;
    

    public enum UITransformationType
    {
        Position,
        Scale,
        Rotation,
    }
    
    [SerializeField] UITransformationType currentTransformationType;
    private Dictionary<UITransformationType, (Action<Vector3>, Func<Vector3>)> transformationTypeToAction;
    //use item1 to set and item2 to get
    private void Awake()
    {
        transformationTypeToAction = new Dictionary<UITransformationType, (Action<Vector3>, Func<Vector3>)>
        {
            {
                UITransformationType.Position, 
                (
                    (value) => { transform.position = value; }, //item1 setter
                    () => transform.position //item2 getter
                )
            },
            {
                UITransformationType.Scale, 
                (
                    (value) => { transform.localScale = value; }, 
                    () => transform.localScale
                )
            },
            {
            UITransformationType.Rotation, 
            (
                (value) => { transform.eulerAngles = value; }, 
                () => transform.eulerAngles
            )
        }
        };
        
        springParams = new SpringUtils.tDampedSpringMotionParams();
        
        // calculate scaling factor based on current screen size and reference screen size
        float scalingFactor = Mathf.Min(Screen.width / referenceResolution.x, Screen.height / referenceResolution.y);

        // adjust targetLocalDisplacement based on scaling factor
        
        if (currentTransformationType == UITransformationType.Position) targetLocalDisplacement *= scalingFactor;
        
        //this scaling factor stuff is necessary because the bigger the screen is, the more units our UI has to cross
        //to get to the intended value. because the game is intended at an aspect ratio of 16:9, it doesnt matter what our 
        //reference resolution is, as long as it's 16:9 (all intended values are based off of 1920x1080 so pls dont change)
    }
    
    //use dictionary to map enum to vector3
    

    private void Start()
    {
        if (useSceneStartValueInstead) startValue = transformationTypeToAction[currentTransformationType].Item2();
        homeValues = startValue;
        transformationTypeToAction[currentTransformationType].Item1(homeValues);
        endingTransformValue = homeValues + targetLocalDisplacement;
        currentTransformValue = homeValues;
        targetTransformValue = homeValues;
    }


    private void OnEnable()
    {
        if (isMask) ActionEvents.StartedGameplayCutscene += BounceInAndOut;
    }

    private void OnDisable()
    {
        if (isMask) ActionEvents.StartedGameplayCutscene -= BounceInAndOut;
    }
    
    private async void BounceInAndOut()
    {
        targetTransformValue = endingTransformValue;
        await Task.Delay(TimeSpan.FromSeconds(7));
        targetTransformValue = homeValues;
    }
    
    public void MoveToEndValue()
    {
        if (disableHover) return;
        targetTransformValue = endingTransformValue;
    }
    
    public void MoveToStartValue()
    {
        if (disableHover) return;
        targetTransformValue = homeValues;
    }

    public void SetSpringValue(Vector3 newValue)
    {
        targetTransformValue = newValue;
    }
    
    public UITransformationType GetCurrentTransformationType()
    {
        return currentTransformationType;
    }
    
    public void DisableHover()
    {
        disableHover = true;
    }

    private void Update()
    {
        endingTransformValue = homeValues + targetLocalDisplacement;
        
        transformationTypeToAction[currentTransformationType].Item1(currentTransformValue);
        
        SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.unscaledDeltaTime, frequency, dampingRatio);
        SpringUtils.UpdateDampedSpringMotion(ref currentTransformValue.x, ref vel.x, targetTransformValue.x, springParams);
        SpringUtils.UpdateDampedSpringMotion(ref currentTransformValue.y, ref vel.y, targetTransformValue.y, springParams);
        SpringUtils.UpdateDampedSpringMotion(ref currentTransformValue.z, ref vel.z, targetTransformValue.z, springParams);
    }
}
