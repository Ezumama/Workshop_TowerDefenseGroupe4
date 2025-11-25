using UnityEngine;

public class CameraControl : MonoBehaviour
{

    [Header ("Camera Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _zoomSpeed;

    private Vector3 _cameraPosition;

    private void Start()
    {
        _cameraPosition = this.transform.position;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            _cameraPosition.y += _moveSpeed / 10;
        }

        if (Input.GetKey(KeyCode.S))
        {
            _cameraPosition.y -= _moveSpeed / 10;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            _cameraPosition.x -= _moveSpeed / 10;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _cameraPosition.x += _moveSpeed / 10;
        }

        this.transform.position = _cameraPosition;
    }
}
