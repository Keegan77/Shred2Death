using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    private static Camera _camera;
    
    /// <summary>
    /// Use this to reference the main camera during gameplay. Using Camera.main is expensive and should be avoided.
    /// </summary>
    public static Camera MainCamera
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }
}
