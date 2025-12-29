using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour
{   public ParticleSystem muzzleFlashParticles;  // Assign in Inspector
    public Light muzzleLight;                    // Optional, assign if using light burst
    public float lightDuration = 0.05f;

    public void PlayMuzzleFlash()
    {
        if (muzzleFlashParticles != null)
        {
            muzzleFlashParticles.Play();
        }

        if (muzzleLight != null)
        {
            StartCoroutine(LightFlash());
        }
    }

    private IEnumerator LightFlash()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleLight.enabled = false;
    }
}
