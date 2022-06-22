using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    [SerializeField] private GameManager gameManager;
    private float speed = 6f;

    private void Start()
    {
        Camera.transform.position += new Vector3(gameManager.Width, 0, 0);
        Camera.orthographicSize += -5 + 1.25f * gameManager.Width;
    }

    void Update()
    {
        var deltaTime = Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
            Camera.transform.position += new Vector3(0, 0, deltaTime * speed);
        if (Input.GetKey(KeyCode.A))
            Camera.transform.position += new Vector3(-deltaTime * speed, 0, 0);
        if (Input.GetKey(KeyCode.S))
            Camera.transform.position += new Vector3(0, 0, -deltaTime * speed);
        if (Input.GetKey(KeyCode.D))
            Camera.transform.position += new Vector3(deltaTime * speed, 0, 0);

        if (Input.GetKey(KeyCode.Q))
            Camera.orthographicSize += Camera.fieldOfView * 0.05f * deltaTime;
        if (Input.GetKey(KeyCode.E))
            Camera.orthographicSize -= Camera.fieldOfView * 0.05f * deltaTime;

        if (Input.GetKey(KeyCode.Z))
            speed -= speed * 1f * deltaTime;
        if (Input.GetKey(KeyCode.X))
            speed += speed * 1f * deltaTime;
    }
}
