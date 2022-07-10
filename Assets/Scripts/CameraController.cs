using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private PlayerController playerController;
    private Vector3 currentPos;
    public Vector3 offset;
    public bool isRotationOn;
    public Vector3 effectRotation;
    int direction = 1;
    Quaternion initialRotation;
    private Vector3 offsetWithZoom;

    private PostProcessVolume postProcVolume;

    void Awake() {
        postProcVolume = GameObject.Find("PostProcessing").GetComponent<PostProcessVolume>();
        initialRotation = Quaternion.identity;
        currentPos = player.position + offset;
        offsetWithZoom = offset;
    }

    private void Update() {
        if (GameManager.instance.postProcOn && !postProcVolume.enabled) {
            postProcVolume.enabled = true;
        }
    }

    void FixedUpdate() {
        if (isRotationOn) {
            transform.Rotate(effectRotation * direction * Time.deltaTime);
            float helpRotZ = transform.rotation.eulerAngles.z;
            if (helpRotZ > 180) {
                helpRotZ = (360 - helpRotZ) * -1;
            }
            if (helpRotZ < -180) {
                helpRotZ = 360 + helpRotZ;
            }
            if (Mathf.Abs(helpRotZ) > 120) {
                direction *= -1;
            }
        }
    }

    void LateUpdate() {
        transform.position = Vector3.Lerp(currentPos, player.position + offsetWithZoom, 5f * Time.deltaTime);
        currentPos = transform.position;
    }

    public void AddZoomToOffset(float zoom) {
        if (zoom < 0.1f || zoom > 4f) return;
        offsetWithZoom = offset * zoom;
    }
}
