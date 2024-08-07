﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float radius = 2.0f;
    public float damage = 100f;
    public float coolDown = 1.0f;
    public LayerMask targetMask;

    public GameObject sunPrefab;
    public GameObject audioSourcePrefab;
    public AudioClip fireSound;

    private Gamemanager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<Gamemanager>();
        StartCoroutine(CountdownAndExplode());
    }

    public void StartExplosionCountdown()
    {
        StartCoroutine(CountdownAndExplode());
    }

    private IEnumerator CountdownAndExplode()
    {
        yield return new WaitForSeconds(coolDown);
        Explode();
    }

    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        foreach (Collider2D collider in colliders)
        {
            Zombie targetZombie = collider.GetComponent<Zombie>();
            if (targetZombie != null)
            {
                targetZombie.Hit(damage, false, true); // true vì bị giết bởi bomb
                targetZombie.OnZombieKilled += HandleZombieKilled;
            }
        }

        if (gameManager != null)
        {
            gameManager.NotifyPlantRemoved(transform.position);
        }

        // Phát âm thanh nổ bằng một đối tượng AudioSource riêng biệt
        if (fireSound != null && audioSourcePrefab != null)
        {
            GameObject audioObject = Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
            AudioSource audioSource = audioObject.GetComponent<AudioSource>();
            audioSource.PlayOneShot(fireSound);
            Destroy(audioObject, fireSound.length);
        }

        Destroy(gameObject);
    }

    void HandleZombieKilled(bool killedByBomb, Vector3 position)
    {
        if (killedByBomb && Random.value <= 0.3f)
        {
            SpawnSun(position);
        }
    }

    void SpawnSun(Vector3 position)
    {
        GameObject mySun = Instantiate(sunPrefab, position, Quaternion.identity);
        mySun.GetComponent<Sun>().dropToYpos = position.y - 1;
    }
}
