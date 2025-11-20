using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVisualEffects : MonoBehaviour
{
    [Header("Particle System")]
    public ParticleSystem collectParticles;
    public bool createParticlesIfNull = true;

    [Header("Scale Animation")]
    public bool useScaleEffect = true;
    public float scaleSpeed = 5f;
    public float maxScale = 1.5f;

    [Header("Fade Out")]
    public bool useFadeOut = true;
    public float fadeSpeed = 2f;

    [Header("Float Up Effect")]
    public bool floatUp = true;
    public float floatSpeed = 2f;
    public float floatDistance = 1f;

    private Collectible collectible;
    private bool isCollecting = false;
    private float collectionTimer = 0f;
    private Vector3 originalScale;
    private Vector3 startPosition;
    private Renderer[] renderers;

    void Start()
    {
        collectible = GetComponent<Collectible>();
        originalScale = transform.localScale;
        startPosition = transform.position;
        renderers = GetComponentsInChildren<Renderer>();

        // Subscribe to collection event
        if (collectible != null)
        {
            // We'll trigger effects before the object is destroyed
        }

        // Create particles if needed
        if (collectParticles == null && createParticlesIfNull)
        {
            CreateDefaultParticles();
        }
    }

    void Update()
    {
        if (isCollecting)
        {
            AnimateCollection();
        }
    }

    public void PlayCollectionEffect()
    {
        isCollecting = true;
        collectionTimer = 0f;

        // Play particles
        if (collectParticles != null)
        {
            collectParticles.transform.parent = null; // Unparent so it stays when coin is destroyed
            collectParticles.Play();
            Destroy(collectParticles.gameObject, collectParticles.main.duration + collectParticles.main.startLifetime.constantMax);
        }
    }

    void AnimateCollection()
    {
        collectionTimer += Time.deltaTime;

        // Scale up effect
        if (useScaleEffect)
        {
            float scale = Mathf.Lerp(1f, maxScale, collectionTimer * scaleSpeed);
            transform.localScale = originalScale * scale;
        }

        // Float up effect
        if (floatUp)
        {
            float upAmount = collectionTimer * floatSpeed;
            transform.position = startPosition + Vector3.up * upAmount;
        }

        // Fade out effect
        if (useFadeOut && renderers.Length > 0)
        {
            float alpha = Mathf.Lerp(1f, 0f, collectionTimer * fadeSpeed);

            foreach (Renderer rend in renderers)
            {
                if (rend.material.HasProperty("_Color"))
                {
                    Color color = rend.material.color;
                    color.a = alpha;
                    rend.material.color = color;
                }
            }
        }

        // Destroy after animation
        if (collectionTimer >= 0.5f)
        {
            Destroy(gameObject);
        }
    }

    void CreateDefaultParticles()
    {
        GameObject particleObj = new GameObject("CollectParticles");
        particleObj.transform.parent = transform;
        particleObj.transform.localPosition = Vector3.zero;

        collectParticles = particleObj.AddComponent<ParticleSystem>();

        // Configure particle system
        var main = collectParticles.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 3f;
        main.startSize = 0.2f;
        main.startColor = Color.yellow;
        main.maxParticles = 20;
        main.loop = false;

        var emission = collectParticles.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 15)
        });

        var shape = collectParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;

        var colorOverLifetime = collectParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.yellow, 0.0f),
                new GradientColorKey(Color.yellow, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorOverLifetime.color = gradient;

        var renderer = collectParticles.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
    }
}

/*
SETUP INSTRUCTIONS:

METHOD 1 - Simple Setup (Automatic):
1. Add this script to your coin prefab
2. It will create default particles automatically
3. Adjust settings in Inspector
4. Done!

METHOD 2 - Custom Particle System:
1. Select your coin
2. Add Component > Effects > Particle System
3. Configure particles (color, size, speed, etc.)
4. Add this script to coin
5. Drag the ParticleSystem to "Collect Particles" field
6. Disable "Create Particles If Null"

IMPORTANT:
To make this work with ItemCollector, you need to modify 
the Collectible script's Collect() method to trigger the effect
before the object is destroyed.
*/