using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Card : MonoBehaviour
{
    [SerializeField] protected Sprite normalSprite;
    [SerializeField] protected Sprite selectableSprite;
    [SerializeField] protected float selectableScale;
    [SerializeField] protected Color chooseColor;
    [SerializeField] protected float chooseScale;
    [SerializeField] protected float scaleEffectTime;

    protected int cardIndex;
    protected GameController gameController;
    protected CardManager cardManager;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D col;
    protected Color normalColor;
    protected float normalScale;
    protected bool isSelected = false;
    protected bool isFaceUp = false;
    protected bool canSelect = false;

    protected virtual void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        normalColor = spriteRenderer.color;
        normalScale = transform.localScale.x;
    }

    public abstract void Init(int cardIndex, bool isFaceUp);

    public virtual void SetManager(CardManager cardManager, GameController gameController) {
        this.cardManager = cardManager;
        this.gameController = gameController;
    }

    protected virtual void SelectableHandle() {
        if (!isSelected) {
            if (isFaceUp) {
                if (gameController.IsCardValid(this)) {
                    SetSelectable(true);
                }
                else {
                    SetSelectable(false);
                }
            }
        }
    }

    public void SetSelectable(bool isSelectable) {
        canSelect = isSelectable;
        col.enabled = isSelectable;
        if (isSelectable) {
            SetScale(selectableScale, true);
            spriteRenderer.sprite = selectableSprite;
            cardManager.AddValidCard(this);
        }
        else {
            SetScale(normalScale, false);
            spriteRenderer.sprite = normalSprite;
            cardManager.RemoveValidCard(this);
        }
    }

    public void SetChoosable(bool canChoose) {
        col.enabled = canChoose;

        if (canChoose) {
            SetScale(chooseScale, true);
            spriteRenderer.color = chooseColor;
        }
        else {
            SetScale(normalScale, false);
            spriteRenderer.color = normalColor;
        }
    }

    protected void SetScale(float scale, bool hasEffect) {
        float startScale = transform.localScale.x;

        if (hasEffect) {
            StopAllCoroutines();
            StartCoroutine(ScaleEffect(startScale, scale, scaleEffectTime));
        }
        else {
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public virtual void FaceUp() {
        gameController.OnCardChanged += SelectableHandle;
    }

    public abstract void Swap();

    private void OnMouseDown() {
        if (GameManager.Instance.IsPaused()) return;

        SelectedHandle();
    }

    public virtual void SelectedHandle() {
        gameController.AddCard(this, isSelected ? true : false);
        SetScale(normalScale, false);
    }

    public void SetSelected() {
        isSelected = true;
    }

    protected IEnumerator ScaleEffect(float startScale, float endScale, float time) {
        if (Mathf.Approximately(startScale, endScale)) {
            transform.localScale = new Vector3(endScale, endScale, endScale);
            yield break;
        }

        float middleScale = startScale > endScale ? endScale * 0.8f : endScale * 1.2f;
        float timer = 0, scale = startScale;

        while (timer < time / 2 * 3) {
            timer += Time.deltaTime;
            scale = Mathf.Lerp(startScale, middleScale, timer / (time / 2 * 3));
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        timer = 0;
        while (timer < time / 3) {
            timer += Time.deltaTime;
            scale = Mathf.Lerp(middleScale, endScale, timer / (time / 3));
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        transform.localScale = new Vector3(endScale, endScale, endScale);
    }

    public int CardIndex => cardIndex;
    public bool IsSelected => isSelected;
    public bool IsFaceUp => isFaceUp;
    public bool CanSelect => canSelect;
}