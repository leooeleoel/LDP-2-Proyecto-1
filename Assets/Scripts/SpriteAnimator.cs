using UnityEngine;

[System.Serializable]
public class AnimationClipData
{
    public string name;
    public Sprite[] frames;
    public float frameRate = 10f;
    public bool loop = true;
}

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    public AnimationClipData[] animations;

    private SpriteRenderer spriteRenderer;
    private AnimationClipData currentClip;
    private int currentFrameIndex;
    private float timer;
    private string currentClipName = "";

    public string CurrentAnimationName => currentClipName;
    public bool IsFinished { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Play(string clipName)
    {
        if (currentClipName == clipName) return;

        AnimationClipData clip = FindClip(clipName);
        if (clip == null)
        {
            Debug.LogWarning($"SpriteAnimator: no se encontro '{clipName}' en {gameObject.name}");
            return;
        }

        currentClip = clip;
        currentClipName = clipName;
        currentFrameIndex = 0;
        timer = 0f;
        IsFinished = false;

        if (currentClip.frames.Length > 0)
            spriteRenderer.sprite = currentClip.frames[0];
    }

    public void PlayStatic(Sprite sprite)
    {
        currentClip = null;
        currentClipName = "";
        spriteRenderer.sprite = sprite;
    }

    private void Update()
    {
        if (currentClip == null || currentClip.frames.Length == 0 || IsFinished) return;

        timer += Time.deltaTime;
        float frameDuration = 1f / currentClip.frameRate;

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrameIndex++;

            if (currentFrameIndex >= currentClip.frames.Length)
            {
                if (currentClip.loop)
                {
                    currentFrameIndex = 0;
                }
                else
                {
                    currentFrameIndex = currentClip.frames.Length - 1;
                    IsFinished = true;
                }
            }

            spriteRenderer.sprite = currentClip.frames[currentFrameIndex];
        }
    }

    private AnimationClipData FindClip(string clipName)
    {
        foreach (AnimationClipData clip in animations)
        {
            if (clip.name == clipName) return clip;
        }
        return null;
    }

    public void SetFacingDirection(bool facingRight)
    {
        spriteRenderer.flipX = !facingRight;
    }
}