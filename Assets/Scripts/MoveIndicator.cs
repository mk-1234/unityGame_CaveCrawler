using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIndicator : MonoBehaviour
{
    private float scaleX;
    private float scaleZ;
    private Material material;
    private Color color = new Color(1f, 1f, 0f, 1f);

    void Awake() {
        scaleX = transform.localScale.x;
        scaleZ = transform.localScale.z;
        material = GetComponent<MeshRenderer>().material;
        material.color = color;
    }
    void Update() {
        scaleX += 0.05f;
        scaleZ += 0.05f;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, scaleZ);
        if (scaleX > 0.5f) {
            Destroy(gameObject);
        }
    }
}
