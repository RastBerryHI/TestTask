using UnityEngine;

enum SoackState : byte 
{
    Soack,
    Dry
}
public class Humidity : MonoBehaviour
{
    [Space]
    [Header("Shader references")]
    [SerializeField]
    string propNameSck;
    [SerializeField]
    string propNameSmth;
    [SerializeField]
    string propNameWetness;

    [Space]
    [Header("Cloth conditions")]
    [SerializeField]
    float smoothness;
    [SerializeField]
    [Range(-1f, 1.3f)]
    float wetBlend;

    [Space]
    [Header("Blend intensities")]
    [SerializeField]
    float intensitySck;
    [SerializeField]
    float intensityDr;
    [SerializeField]
    [Range(0, 100)]
    int maxDrops = 45;

    [Space]
    [Header("FX")]
    [SerializeField]
    ParticleSystem wtFX;

    float currentDampSmth, currentDampWts;
    SoackState soackState;

    [Space]
    [Header("Proceeding material")]
    [SerializeField]
    Renderer mat;

    void OnApplicationQuit() 
    {
        mat.sharedMaterial.SetFloat(propNameSck, 0.4f);
        mat.sharedMaterial.SetFloat(propNameSmth, 0.4f);
        mat.sharedMaterial.SetFloat(propNameWetness, -1f);
    }

    [System.Obsolete]
    void Update()
    {
        // if there are material and particle system
        if (mat && wtFX) 
        {
            // Checking within enum states
            switch (soackState)
            {
                // Soack state - OnTriggerEnter
                case SoackState.Soack:
                    // increasing smoothness & "wetness" - lerp parameter in shader
                    mat.sharedMaterial.SetFloat(propNameSmth, Mathf.SmoothDamp(mat.sharedMaterial.GetFloat(propNameSmth), smoothness, ref currentDampSmth, intensitySck));  
                    mat.sharedMaterial.SetFloat(propNameWetness, Mathf.SmoothDamp(mat.sharedMaterial.GetFloat(propNameWetness), wetBlend, ref currentDampWts, intensitySck));

                    // if smoothness is valid - play water drops
                    if (mat.sharedMaterial.GetFloat(propNameSmth) >= 0.5) { 
                        wtFX.Play();
                    }
                    break;

                // Dry state - OnTriggerExit
                case SoackState.Dry:
                    // decreasing smoothness & "wetness" - lerp parameter in shader
                    mat.sharedMaterial.SetFloat(propNameSmth, Mathf.SmoothDamp(mat.sharedMaterial.GetFloat(propNameSmth), 0.4f, ref currentDampSmth, intensityDr));
                    mat.sharedMaterial.SetFloat(propNameWetness, Mathf.SmoothDamp(mat.sharedMaterial.GetFloat(propNameWetness), -1, ref currentDampWts, intensityDr));

                    // if smoothness is not valid - disable water drops
                    if (mat.sharedMaterial.GetFloat(propNameSmth) < 0.5) {
                        wtFX.Stop();
                    }
                    break;
            }
        }
    }
    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        // fields initialisation 
        Debug.LogWarning("Soack");
        soackState = SoackState.Soack;
        mat = other.GetComponentInChildren<Renderer>();
        wtFX = other.GetComponentInChildren<ParticleSystem>();

        if (wtFX)
            wtFX.maxParticles = 0;
    }

    [System.Obsolete]
    private void OnTriggerExit(Collider other)
    {
        soackState = SoackState.Dry;
        Debug.LogError("Dry");

        if (wtFX)
            wtFX.maxParticles = maxDrops;
    }
}