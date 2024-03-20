using UnityEngine;

public class FillBar : MonoBehaviour
{
    [Header ("Values")]
    public float maxValue = 1;
    public float currentValue = 0;


    private UnityEngine.UI.Image img;

    private void Awake ()
    {
        img = GetComponent<UnityEngine.UI.Image>();
    }
    private void Update ()
    {
        float fillAmount = currentValue / maxValue;

        img.fillAmount = Mathf.Clamp (fillAmount, 0, 1);
    }
}
