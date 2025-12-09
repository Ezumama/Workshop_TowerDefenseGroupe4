using UnityEngine;

public class UI_RadarSweep : MonoBehaviour
{
    [SerializeField] private RectTransform sweepTransform;
    [SerializeField] private float rotationSpeed = 120f;

    void Update()
    {
        sweepTransform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }
}