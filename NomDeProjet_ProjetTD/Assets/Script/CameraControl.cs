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
            // Move camera up,down,left,right

            if (Input.GetKey(KeyCode.W))
            {
                _cameraPosition.y += _moveSpeed / 10;
            }

            if (Input.GetKey(KeyCode.S))
            {
                _cameraPosition.y -= _moveSpeed / 10;
            }

            if (Input.GetKey(KeyCode.A))
            {
                _cameraPosition.x -= _moveSpeed / 10;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _cameraPosition.x += _moveSpeed / 10;
            }

            // Zoom forward and backward

            if (Input.GetKey(KeyCode.Q))
            {
                _cameraPosition.z += _zoomSpeed / 10;
            }

            if (Input.GetKey(KeyCode.E))
            {
                _cameraPosition.z -= _zoomSpeed / 10;
            }

            this.transform.position = _cameraPosition;
        }
    }
