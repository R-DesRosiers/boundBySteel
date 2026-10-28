using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{ // 
    public float hoverScale = 1.08f;
    public float speed = 10f;

    private Vector3 normalScale;
    private Vector3 targetScale;

    void Start()
    {
        normalScale = transform.localScale;
        targetScale = normalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * speed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    { // enlarge the button a little when hovering over it 
        targetScale = normalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    { // then return button size back to normal when exiting 
        targetScale = normalScale;
    }
}