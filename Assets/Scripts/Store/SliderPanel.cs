using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SliderPanel : MonoBehaviour
{
    private RectTransform panel;
    public float slideDuration = 0.5f;
    public float offscreenOffset = 370f;

    private bool isVisible = true;
    private Vector2 onScreenPos;
    private Vector2 offScreenPos;

    void Start()
    {
        panel = GetComponent<RectTransform>();
        onScreenPos = panel.anchoredPosition;
        offScreenPos = onScreenPos + new Vector2(offscreenOffset, 0);
    }

    public void TogglePanel()
    {
        StopAllCoroutines();
        StartCoroutine(Slide(isVisible ? offScreenPos : onScreenPos));
        isVisible = !isVisible;
    }

    IEnumerator Slide(Vector2 target)
    {
        Vector2 start = panel.anchoredPosition;
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            panel.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        panel.anchoredPosition = target;
    }
}
