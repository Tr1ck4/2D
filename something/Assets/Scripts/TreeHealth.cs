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

        // Remove tree from TreeSpawner's data
        if (TreeSpawner.Instance != null)
        {
            TreeSpawner.Instance.RemoveTreePosition(transform.position);
        }
        
        Destroy(gameObject);
    }

    private void DelayedDestroy()
    {
        Destroy(gameObject);
        Debug.Log("Tree game object destroyed.");
    }

    private void SpawnLogItem()
    {
        if (logPrefab != null)
        {
            GameObject log = Instantiate(logPrefab, transform.position, Quaternion.identity);

            Animator logAnimator = log.GetComponent<Animator>();
            Item logItem = log.GetComponent<Item>();

            if (logAnimator != null && logItem != null && logItem.data != null && logItem.data.dropClip != null)
            {
                ModifyAnimationClip(logItem.data.dropClip, log.transform.position);
                //logAnimator.Play(logItem.data.dropClip.name);
            }
        }
        else
        {
            Debug.LogWarning("Log Prefab is not assigned.");
        }
    }

    private void ModifyAnimationClip(AnimationClip clip, Vector3 startPosition)
    {
        Keyframe[] posX = {
            new Keyframe(0, startPosition.x),
            new Keyframe(0.5f, startPosition.x + 0.5f),
            new Keyframe(1, startPosition.x + 1f)
        };
        Keyframe[] posY = {
            new Keyframe(0, startPosition.y),
            new Keyframe(0.5f, startPosition.y + 0.3f),
            new Keyframe(1, startPosition.y - 0.8f)
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

        Debug.Log("Animation clip modified.");
    }
}
