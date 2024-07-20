using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public AudioClip chopSound;
    public GameObject logPrefab;
    public AnimationClip dropClip; // Reference to the drop animation clip

    void Start()
    {
        currentHealth = maxHealth;
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
        SpawnLogItem();
        Destroy(gameObject);
    }

    private void SpawnLogItem()
    {
        if (logPrefab != null)
        {
            // Instantiate the log prefab at the tree's position
            GameObject log = Instantiate(logPrefab, transform.position, Quaternion.identity);

            // Modify the drop animation clip at runtime
            Animator logAnimator = log.GetComponent<Animator>();
            Item logItem = log.GetComponent<Item>();

            if (logAnimator != null && logItem != null && logItem.data != null && logItem.data.dropClip != null)
            {
                // Modify the position keyframes
                ModifyAnimationClip(logItem.data.dropClip, log.transform.position);

                // Play the modified animation
                logAnimator.Play(logItem.data.dropClip.name);
            }
        }
    }

    private void ModifyAnimationClip(AnimationClip clip, Vector3 startPosition)
    {

        Keyframe[] posX = {
            new Keyframe(0, startPosition.x),
            new Keyframe(0.5f, startPosition.x + 0.5f), // Move right by 0.5 units
            new Keyframe(1, startPosition.x + 1f) // End position
        };
        Keyframe[] posY = {
            new Keyframe(0, startPosition.y),
            new Keyframe(0.5f, startPosition.y + 0.3f), // Peak of parabola
            new Keyframe(1, startPosition.y - 0.8f) // End position below start position
        };
        Keyframe[] posZ = {
            new Keyframe(0, startPosition.z),
            new Keyframe(0.5f, startPosition.z),
            new Keyframe(1, startPosition.z)
        };


        AnimationCurve curveX = new AnimationCurve(posX);
        AnimationCurve curveY = new AnimationCurve(posY);
        AnimationCurve curveZ = new AnimationCurve(posZ);

        clip.ClearCurves();

        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);
    }
}
