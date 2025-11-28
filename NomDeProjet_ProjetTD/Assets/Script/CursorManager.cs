using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _defaultCursorTexture;
    [SerializeField] private Texture2D _highlightCursorTexture;

    public CursorMode CursorMode = CursorMode.Auto;
    public Vector2 HotSpotMouse = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.SetCursor(_defaultCursorTexture, HotSpotMouse, CursorMode);
    }

    public void OnMouseEnter()
    {
        Cursor.SetCursor(_highlightCursorTexture, HotSpotMouse, CursorMode);
    }

    public void OnMouseExit()
    {
        Cursor.SetCursor(_defaultCursorTexture, HotSpotMouse, CursorMode);
    }
}
