using UnityEngine;

public class SingleCard : Card {
    [SerializeField] SpriteRenderer animalRenderer;
    protected Animator animator;

    protected override void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void Init(int cardIndex, bool isFaceUp) {
        this.cardIndex = cardIndex;
        (int first, _) = cardManager.GetAnimalIndex(cardIndex);
        animalRenderer.sprite = cardManager.GetAnimalSprites()[first];

        if (isFaceUp) {
            FaceUp();
        }
    }

    public override void FaceUp() {
        base.FaceUp();
        isFaceUp = true;
        animalRenderer.gameObject.SetActive(true);
    }

    public void SetFly(bool isFly) {
        animator.SetBool("Fly", isFly);
    }

    protected override void SelectableHandle() {
        if (isSelected) return;

        (int first, _) = cardManager.GetAnimalIndex(cardIndex);
        if (gameController.IsValidCircle(first)) {
            SetFly(true);
        }
        else {
            if (!isSelected)
                SetFly(false);
        }

        base.SelectableHandle();
    }

    public override void Swap() { }
}