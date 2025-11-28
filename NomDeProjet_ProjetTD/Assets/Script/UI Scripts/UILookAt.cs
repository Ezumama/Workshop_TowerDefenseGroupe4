using UnityEngine;

public class UILookAt : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * (Vector3.forward * -1f),Camera.main.transform.rotation * Vector3.up);

    }
}
