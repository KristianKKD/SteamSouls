using UnityEngine;

public class CameraControl : MonoBehaviour {

    QuickReferences qr;
    PlayerStats ps;

    public float targetFollowDistance = 10.0f;
    public float rotationSmoothing = 0.2f;
    public float followSmoothing = 0.1f;

    float inpX, inpY;
    float lastValidY;

    Vector3 lastValidRotation;
    Vector3 targetHeadRotation;
    float targetBodyRotationY;

    Vector3 targetCameraPos;

    public Camera cam;

    public float clampAngle = 75f;
    public float followY = 0f;
    public float objectCollisionOffset = 0.1f;

    void Awake() {
        qr = GetComponent<QuickReferences>();
        ps = GetComponent<PlayerStats>();
        cam = qr.camParent.GetComponent<Camera>();
    }

    void Update() {
        if (ps.localPaused || !ps.localOwner) //no input whilst paused, nor from outside inputs
            return;

        inpX += (Input.GetAxis("Mouse X") + Input.GetAxis("Gamepad Right X")) * SettingsManager.sm.sensitivity * Time.deltaTime;
        inpY -= (Input.GetAxis("Mouse Y") + Input.GetAxis("Gamepad Right Y")) * SettingsManager.sm.sensitivity  * Time.deltaTime;

        inpY = Mathf.Clamp(inpY, -clampAngle, clampAngle);

        Vector3 targetPos = qr.gameObject.transform.position + (Vector3.up * followY);
        Quaternion rot = Quaternion.Euler(inpY, inpX, 0);
        Vector3 followPos = new Vector3(0, 0, -targetFollowDistance);

        RaycastHit hit;
        if (Physics.Linecast(targetPos, targetPos + rot * followPos + Vector3.up * -objectCollisionOffset, out hit, -1, QueryTriggerInteraction.Ignore) && hit.distance < targetFollowDistance)
            followPos = Vector3.forward * -hit.distance + Vector3.up * objectCollisionOffset;

        targetCameraPos = targetPos + rot * followPos;
    }

    private void LateUpdate() {
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetCameraPos, followSmoothing);
        cam.transform.LookAt(qr.gameObject.transform);

        if (ps.moving) {
            targetHeadRotation = cam.transform.rotation.eulerAngles;
            targetBodyRotationY = cam.transform.rotation.eulerAngles.y;

            lastValidRotation = targetHeadRotation;
        } else {
            float diff = cam.transform.rotation.eulerAngles.y - lastValidRotation.y;
            if (diff < -180)
                diff += 360;
            if (diff > 180)
                diff -= 360;

            float headDifference = Mathf.Clamp(diff, -45, 45);
            targetHeadRotation = new Vector3(cam.transform.rotation.eulerAngles.x, lastValidRotation.y + headDifference, lastValidRotation.z);
        }

        Quaternion smoothedHeadRotation = Quaternion.Lerp(qr.head.transform.rotation, Quaternion.Euler(targetHeadRotation + new Vector3(-20, 0, 0)), rotationSmoothing);
        Quaternion smoothedBodyRotation = Quaternion.Lerp(qr.body.transform.rotation, Quaternion.Euler(0, targetBodyRotationY, 0), rotationSmoothing * 0.5f);

        if (!ps.climbing) {
            qr.head.transform.rotation = smoothedHeadRotation;
            qr.body.transform.rotation = smoothedBodyRotation;
        }
    }
}
