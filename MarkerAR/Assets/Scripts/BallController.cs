using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    public float resetTime = 3.0f;
    public float captureRate = 0.3f;
    public TMP_Text result;
    public GameObject effect;

    Rigidbody rb;
    bool isReady = true;
    bool isPressed = false;
    Vector2 startPos, currentPos;

    // Start is called before the first frame update
    void Start()
    {
        result.text = "";

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isReady)
        {
            return;
        }

        SetBallPosition(Camera.main.transform);

        /*if(Input.touchCount > 0 && isReady )
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began )
            {
                startPos = touch.position;
            }
            else if(touch.phase == TouchPhase.Ended )
            {
                float dragDistance = touch.position.y - startPos.y;

                Vector3 throwAngle = (Camera.main.transform.forward + Camera.main.transform.up).normalized;

                rb.isKinematic = false;
                isReady = false;

                rb.AddForce(throwAngle * dragDistance * 0.005f, ForceMode.VelocityChange);

                Invoke("ResetBall", resetTime);
            }
        }*/
    }
    void SetBallPosition(Transform anchor)
    {
        Vector3 offset = anchor.forward * 0.5f + anchor.up * -0.2f;
        transform.position = anchor.position + offset;
    }
    void ResetBall()
    {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        isReady = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(isReady)
        {
            return;
        }
        float draw = Random.Range(0, 1.0f);
        if(draw <= captureRate)
        {
            result.text = "Successful capture!";

            DB_Manager.instance.UpdateCaptured();
        }
        else
        {
            result.text = "You failed and it ran away...";
        }
        Instantiate(effect, collision.transform.position, Camera.main.transform.rotation);

        Destroy(collision.gameObject);
        gameObject.SetActive(false);
    }

    public void TouchBall(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            gameObject.SetActive(true);

            isPressed = false;

            float dragDistance = currentPos.y - startPos.y;
            Vector3 throwAngle = (Camera.main.transform.forward + Camera.main.transform.up).normalized;

            isReady = false;
            rb.isKinematic = false;
            rb.AddForce(throwAngle * dragDistance * 0.005f, ForceMode.VelocityChange);

            Invoke("ResetBall", resetTime);
        }
    }

    public void DragBall(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = context.ReadValue<Vector2>();
        if(touchPosition != null)
        {
            if(isReady == false)
            {
                isPressed = true;
                startPos = touchPosition;
            }
            else
            {
                currentPos = touchPosition;
            }
        }
    }
}
