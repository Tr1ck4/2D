using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the tree
    private int currentHealth;
    public AudioClip chopSound;

    void Start()
    {
        currentHealth = maxHealth; // Initialize the tree's health
    }

    public void Chop(int damage)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && chopSound != null)
        {
            audioSource.PlayOneShot(chopSound);
        }
        currentHealth -= damage;
        Debug.Log($"Tree chopped! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            DestroyTree();
        }
    }

    private void DestroyTree()
    {
        Debug.Log("Tree destroyed!");
        Destroy(gameObject); // Destroy the tree GameObject
    }
}
