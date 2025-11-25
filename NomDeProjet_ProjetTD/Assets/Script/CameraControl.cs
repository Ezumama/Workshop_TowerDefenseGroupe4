    using UnityEngine;

    public class CameraControl : MonoBehaviour
    {

        [Header ("Camera Settings")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _zoomSpeed;
        [SerializeField] private float _normalFieldOfView;

        [Header("Screen's Boundaries")]
        [SerializeField] private float _boundaryXMin;
        [SerializeField] private float _boundaryXMax;
        [SerializeField] private float _boundaryYMin;
        [SerializeField] private float _boundaryYMax;

        [Header ("Zoom Boundaries")]
        [SerializeField] private float _zoomMax;
        [SerializeField] private float _zoomMin;
        
        private Vector3 _cameraPosition;
        private Camera _camera;

        private void Start()
        {
            _cameraPosition = this.transform.position;
            _camera = GetComponent<Camera>();
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

            // Clamp camera position
            _cameraPosition.y = Mathf.Clamp(_cameraPosition.y, _boundaryYMin, _boundaryYMax);
            _cameraPosition.x = Mathf.Clamp(_cameraPosition.x, _boundaryXMin, _boundaryXMax);

            // Zoom forward and backward
            if (Input.GetKey(KeyCode.Q))
            {
                _camera.fieldOfView += _zoomSpeed / 10;
            }

            if (Input.GetKey(KeyCode.E))
            {
                _camera.fieldOfView -= _zoomSpeed / 10;
            }

            // Clamp camera's FOV
            _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView, _zoomMin, _zoomMax);

            // Reset camera's field of view
            //if (Input.GetKey(KeyCode.R))
            //{
            //    _camera.fieldOfView = _normalFieldOfView;
            //}

            this.transform.position = _cameraPosition;
        
        }
    }
