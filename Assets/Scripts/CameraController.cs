/* ds18635 2101128
 * ======================
 * This class handles the players movement of the view as the “overlord.”
 * ======================
 */
using UnityEngine;

public class CameraController : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject taskUI;
    public GameObject pauseUI;
    public float panSpeed = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;

    // Update is called once per frame
    private void Update() {
        if (!pauseUI.activeSelf && !taskUI.activeSelf) {
            var pos = transform.position;
            if (Input.GetKey("w")) pos.y += panSpeed * Time.deltaTime;
            if (Input.GetKey("s")) pos.y -= panSpeed * Time.deltaTime;
            if (Input.GetKey("a")) pos.x -= panSpeed * Time.deltaTime;
            if (Input.GetKey("d")) pos.x += panSpeed * Time.deltaTime;

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            pos.z -= scroll * scrollSpeed * 10f * Time.deltaTime;

            pos.x = Mathf.Clamp(pos.x, 0, panLimit.x);
            pos.y = Mathf.Clamp(pos.y, 0, panLimit.y);
            if (pos.x > 42f) pos.x = 42f;
            if (pos.x < 21.5f) pos.x = 21.5f;
            if (pos.y > 52f) pos.y = 52f;
            if (pos.y < 12f) pos.y = 12f;
            transform.position = pos;
        }
    }
} 