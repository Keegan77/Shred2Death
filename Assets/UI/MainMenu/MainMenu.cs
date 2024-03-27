using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MainMenu : MonoBehaviour
{
    [Header("Parameters")]
    public GameObject firstMenu;

    [Tooltip("How long does it take for the camera to pan between menus?")]
    public float cameraPanTime = 0.5f;

    public SliderRef sliderObj;
    Camera cam;
    Coroutine panner;
    private void Awake()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.None;
        SaveSensitivity();
    }

    public void LoadLevel(Scene s)
    {
        SceneManager.LoadScene(s.name);
    }
    public void testLoadLevel(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void moveToSubMenu(GameObject m)
    {
        foreach (Transform g in transform)
        {
            g.gameObject.SetActive(false);
        }

        m.SetActive(true);
        m.GetComponent<SubMenu>().defaultButtonSelected.Select();

        StopAllCoroutines();
        StartCoroutine(PanCamera(m.GetComponent<SubMenu>()));
    }
    
    public void SaveSensitivity()
    {
        float newSens = Mathf.Lerp(25, 300, sliderObj.sensitivitySlider.value);
        PlayerPrefs.SetFloat("Sensitivity", newSens);
        Debug.Log("Sensitivity saved as " + newSens);
    }
    

    IEnumerator PanCamera(SubMenu m)
    {
        GameObject target = m.cameraAngle;
        float startTime = Time.time;
        Vector3 startPos = cam.transform.position;
        Quaternion startRot = cam.transform.rotation;

        while (cam.transform.position != target.transform.position && cam.transform.rotation != target.transform.rotation)
        {
            float amount = (Time.time - startTime) / cameraPanTime;
            cam.transform.position = Vector3.Lerp(startPos, target.transform.position, amount);
            cam.transform.rotation = Quaternion.Lerp(startRot, target.transform.rotation, amount);
            yield return null;
        }

        //Debug.Log("Done");
    }

    private void Start()
    {
        moveToSubMenu(firstMenu);
    }
}

