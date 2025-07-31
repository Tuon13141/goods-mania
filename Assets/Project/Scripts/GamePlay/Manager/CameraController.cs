using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject m_Bg;

    bool _canScale = false;

    public void SetUp()
    {
        _canScale = true;
        ScaleBackgroundToFitScreen();
    }

    void ScaleBackgroundToFitScreen()
    {
        if (!_canScale) return;
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Camera.main.aspect;

        m_Bg.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, m_Bg.transform.position.z);
        m_Bg.transform.localScale = new Vector3(screenWidth / 4, .1f, screenHeight / 4);
    }

    private void Update()
    {
        //ScaleBackgroundToFitScreen();
    }

    public void OnReset()
    {
        _canScale = false;
    }

    public void FitChildrenToBottomPartOfCamera(GameObject parent, float heightRatio = 0.65f, float widthRatio = 0.8f)
    {
        Camera camera = Camera.main;
        if (parent == null || camera == null)
        {
            Debug.LogError("Parent or Camera is null");
            return;
        }

        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found in children");
            return;
        }

        Bounds totalBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            totalBounds.Encapsulate(renderers[i].bounds);
        }

        Transform parentTransform = parent.transform;

        // Tính camera size
        float camHeight = camera.orthographicSize * 2f;
        float camWidth = camHeight * camera.aspect;

        float targetHeight = camHeight * heightRatio;
        float targetWidth = camWidth * widthRatio;

        float scaleX = targetWidth / totalBounds.size.x;
        float scaleY = targetHeight / totalBounds.size.y;
        float scale = Mathf.Min(scaleX, scaleY);

        parentTransform.localScale = Vector3.one * scale;

        Vector3 offsetFromParent = totalBounds.center - parentTransform.position;
        Vector3 scaledOffset = offsetFromParent * scale;
        Vector3 actualCenter = parentTransform.position + scaledOffset;

        Vector3 camPos = camera.transform.position;

        float centerY = 3;

        Vector3 targetCenter = new Vector3(0, centerY, 0);

        Vector3 delta = targetCenter - actualCenter;
        parentTransform.position += delta;
    }
}
