using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Name: Ross Hutchins
//ID: HUT18001284

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject projectileTemplate;

    [SerializeField] private float velocity = 30.0f;
    [SerializeField] private float spread = 45.0f;
    [SerializeField] private int shellCount = 1;


    [SerializeField] private AudioSource audio;
    [SerializeField] private List<AudioClip> gunshotSounds;

    public void Fire()
    {

        //This will play a random audio clip from a list of gunshotSounds, if there is any.
        if (gunshotSounds.Count > 0)
            audio.PlayOneShot(gunshotSounds[Random.Range(0, gunshotSounds.Count - 1)]);

        for (int i = 0; i < shellCount; i++)
        {
            float scalar = Mathf.Tan(spread * Mathf.Deg2Rad * 2.0f);
            float angle = Random.Range(0.0f, 360.0f);

            Vector3 offset = (muzzlePoint.forward * Mathf.Sin(angle) + muzzlePoint.right * Mathf.Cos(angle)) * Random.Range(-scalar, scalar);
            Vector3 velocityVec = (offset + muzzlePoint.up).normalized;

            GameObject projectile = Instantiate(projectileTemplate);
            projectile.transform.position = muzzlePoint.position;
            projectile.transform.rotation = Quaternion.LookRotation(velocityVec);
            projectile.GetComponent<Rigidbody>().velocity = velocityVec * velocity;
            projectile.SetActive(true);
        }
    }

}
