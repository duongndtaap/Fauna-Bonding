using System.Collections.Generic;
using UnityEngine;

public class DoubleCard : Card {
    [SerializeField] List<SpriteRenderer> animalRenderers;

    public override void Init(int cardIndex, bool isFaceUp) {
        this.cardIndex = cardIndex;
        (int animalIndex1, int animalIndex2) = cardManager.GetAnimalIndex(cardIndex);
        animalRenderers[0].sprite = cardManager.GetAnimalSprites()[animalIndex1];
        animalRenderers[1].sprite = cardManager.GetAnimalSprites()[animalIndex2];

        SetScale(selectableScale, true);

        if (isFaceUp) {
            FaceUp();
        }
    }

    public override void FaceUp() {
        base.FaceUp();
        isFaceUp = true;
        animalRenderers[0].gameObject.SetActive(true);
        animalRenderers[1].gameObject.SetActive(true);
    }

    public override void Swap() {
        Sprite sprite = animalRenderers[0].sprite;
        animalRenderers[0].sprite = animalRenderers[1].sprite;
        animalRenderers[1].sprite = sprite;
    }
}