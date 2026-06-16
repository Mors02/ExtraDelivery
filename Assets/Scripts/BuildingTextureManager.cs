using System.Collections;
using UnityEngine;

public class BuildingTextureManager : MonoBehaviour
{

 [Header("Fade Settings")]
    public float fadedAlpha = 0.2f;
    public float fadeDuration = 0.25f;

    private Renderer _renderer;
    private Material _material;
    private float _originalAlpha;
    private Coroutine _fadeRoutine;

    /// <summary>
    /// Referneces the shader BaseColor
    /// </summary>
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();

        _material = _renderer.material;

         Color c = _material.GetColor(BaseColorID);
        _originalAlpha = c.a;
    }

    private void StartFade(float targetAlpha)
    {
        Debug.Log("StartedFade");
        Debug.Log(targetAlpha);
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

         _fadeRoutine = StartCoroutine(FadeCoroutine(targetAlpha));
    }

    public IEnumerator FadeCoroutine(float targetAlpha)
    {   
        //get the color of the material
        Color c = _material.GetColor(BaseColorID);
        float startAlpha = c.a;
        float t = 0f;

        //while transitioning
        while (t < 1f)
        {
            //update time
            t += Time.deltaTime / fadeDuration;
            //lerp between the current alpha and the target by t
            float a = Mathf.Lerp(startAlpha, targetAlpha, t);
            c.a = a;
            Debug.Log("A: " + a );
            //set the material color
            _material.SetColor(BaseColorID, c);
            //wait for end of frame
            yield return null;
        }

        //set the alpha and the color
        c.a = targetAlpha;
        _material.SetColor(BaseColorID, c);
        _fadeRoutine = null;
    }

    public void FadeOut()
    {
        StartFade(_originalAlpha * fadedAlpha);
    }

    public void FadeIn()
    {
        StartFade(_originalAlpha);
    }
}
