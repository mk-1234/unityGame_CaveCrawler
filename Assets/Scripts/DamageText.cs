using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI damageText;
    private float duration;
    private Vector2 moveDir;
    private float speed;
    private bool isCrit;
    private float initialFontSize;
    private float fontSizeIncreaseFactor;

    public float Duration {
        get {
            return duration;
        }
    }

    public bool IsCrit { 
        get { return isCrit; }
        set { isCrit = value; } 
    }

    public float InitialFontSize {
        get { return initialFontSize; }
        set { initialFontSize = value; }
    }

    public float FontSizeIncreaseFactor {
        get { return fontSizeIncreaseFactor; }
        set { fontSizeIncreaseFactor = value; }
    }

    private void Awake() {
        damageText = GetComponent<TextMeshProUGUI>();
        moveDir = new Vector2(Random.Range(-30f, 30f) / 100, 1).normalized;
        duration = 0.8f;
        speed = 300f;
    }

    public void Move() {
        if (duration > 0.6f) {
            damageText.fontSize += initialFontSize * fontSizeIncreaseFactor * Time.deltaTime * (isCrit ? 2f : 1f);
        } else {
            damageText.fontSize = Mathf.Max(damageText.fontSize - initialFontSize * fontSizeIncreaseFactor * Time.deltaTime, 12f);
            Color tColor = damageText.color;
            damageText.color = new Color(tColor.r, tColor.g, tColor.b, tColor.a * 0.95f);
        }
        if (transform.position.y > Screen.height * 0.7f) {
            moveDir.y = (Screen.height * 0.9f - transform.position.y) / (Screen.height * 0.2f);
        }
        if (transform.position.x < Screen.width * 0.1f || transform.position.x > Screen.width * 0.9f) {
            moveDir.x *= -1f;
        }
        float tempX = transform.position.x + moveDir.x * speed * Time.deltaTime;
        float tempY = transform.position.y + moveDir.y * speed * Time.deltaTime;
        transform.position = new Vector3(tempX, tempY, transform.position.z);
        duration -= Time.deltaTime;
    }
}
