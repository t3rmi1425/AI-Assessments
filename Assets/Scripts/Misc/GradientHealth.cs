using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientHealth : MonoBehaviour
{

    public Image healthBar;

    /// <summary>
    /// How much health the gameObject has
    /// </summary>
    public float currentHealth;

    /// <summary>
    /// Max health that gameObject is allowed
    /// </summary>
    public float maxHealth;
    public Gradient gradient;

    void Update()
    {
        SetHealth(currentHealth);
    }

    /// <summary>
    /// Used to display and set the current health
    /// </summary>
    /// <param name="health">how much health the gameObject has</param>
    public void SetHealth(float health)
    {
        healthBar.fillAmount = Mathf.Clamp01(health / maxHealth);

        healthBar.color = gradient.Evaluate(healthBar.fillAmount); // for the health bars gradient color
    }
}
