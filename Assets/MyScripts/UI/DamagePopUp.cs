using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float lifetime = 1f;
    public Vector3 offset = new Vector3(0, 1f, 0);
    public Vector3 randomizeIntensity = new Vector3(0.5f, 0.5f, 0);

    private TextMeshProUGUI textMesh;
    private Color startColor;
    private float timer;

    public void Setup(float damageAmount)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshProUGUI>();

            textMesh.text = Mathf.RoundToInt(damageAmount).ToString();
            startColor = textMesh.color;
            transform.localPosition += offset;

            transform.localPosition += new Vector3(
            Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
            Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            Random.Range(-randomizeIntensity.z, randomizeIntensity.z)
            );
    }

    private void Update()
    {
        if (textMesh == null) return; // 🔒 avoid null reference

        // Face the camera
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform);

        // Float upward
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime, Space.World);

        // Fade out
        timer += Time.deltaTime;
        float fade = 1f - (timer / lifetime);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fade);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
