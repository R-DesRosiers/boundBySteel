using UnityEngine;

public class BouncingText : MonoBehaviour
{ // for the end of level identifiers, makes the text less boring 
    public float bounceHeight = 0.15f;
    public float bounceSpeed = 2f;

    private Vector3 startingPosition;

    private void Start()
    {
        startingPosition = transform.localPosition;
    }

    private void Update()
    {
        float verticalMovement =
            Mathf.Sin(Time.time * bounceSpeed) *
            bounceHeight;

        transform.localPosition =
            startingPosition +
            new Vector3(0f, verticalMovement, 0f);
    }
}