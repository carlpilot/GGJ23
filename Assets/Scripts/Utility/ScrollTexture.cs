using UnityEngine;

public class ScrollTexture : MonoBehaviour {

    public Vector2 scrollSpeed;
    public float minRandom = 0.8f;
    public float maxRandom = 1.2f;

    Material mat;

    private void Start () {
        mat = GetComponent<Renderer> ().material;
        scrollSpeed *= Mathf.Lerp (minRandom, maxRandom, Random.value);
    }

    void Update () {        
        mat.mainTextureOffset += scrollSpeed * Time.deltaTime; // can't directly change the renderer material
        GetComponent<Renderer> ().material = mat;
    }
}
