using System.Collections.Generic;
using UnityEngine;

public class TreeNode {
    public Card leftCard;
    public Card rightCard;

    public TreeNode(Card leftCard, Card rightCard) {
        this.leftCard = leftCard;
        this.rightCard = rightCard;
    }
}

public class CardManager : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] List<Sprite> animalSprites = new List<Sprite>();
    [SerializeField] TransCreater transCreater;

    [SerializeField] GameObject doubleCardPrefab;
    [SerializeField] GameObject singleCardPrefab;

    private static readonly (int first, int second)[] animalPair = new (int, int)[]
    {
        (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6),
        (1, 2), (1, 3), (1, 4), (1, 5), (1, 6),
        (2, 3), (2, 4), (2, 5), (2, 6),
        (3, 4), (3, 5), (3, 6),
        (4, 5), (4, 6),
        (5, 6),
        (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6)
    };
    
    private HashSet<int> selectedCardIndexes = new HashSet<int>();
    private Dictionary<Card, TreeNode> cardTrees = new Dictionary<Card, TreeNode>();
    private HashSet<Card> validCards = new HashSet<Card>();

    public void InitialCards() {
        // Init double cards
        List<Vector2> doubleCardTrans = transCreater.GetDoubleCardTrans();

        List<int> cardIndexes = new List<int>();
        for (int i = 0; i < GameSettings.DoubleCardCount; i++) {
            cardIndexes.Add(i);
        }
        cardIndexes = ShuffleTool.ArrangeList(cardIndexes);

        Card[,] array = new Card[GameSettings.MaxBottomCount, GameSettings.MaxBottomCount];
        int maxNumberCardOfRow = 1;
        int numberCardOfRow = 0;

        for (int i = 0; i < doubleCardTrans.Count; i++) {
            GameObject obj = Instantiate(doubleCardPrefab, transform);
            obj.transform.position = doubleCardTrans[i];

            Card card = obj.GetComponent<Card>();
            card.SetManager(this, gameController);
            bool isSelectable = i > GameSettings.DoubleCardCount - GameSettings.MaxBottomCount - 1 ? true : false;
            card.Init(cardIndexes[i], isSelectable);

            array[maxNumberCardOfRow - 1, numberCardOfRow] = card;
            if (numberCardOfRow == maxNumberCardOfRow - 1) {
                maxNumberCardOfRow++;
                numberCardOfRow = 0;
            }
            else numberCardOfRow++;
        }

        // Init single cards
        List<Vector2> singleCardTrans = transCreater.GetSingleCardTrans();

        for (int i = 0; i < singleCardTrans.Count; i++) {
            GameObject obj = Instantiate(singleCardPrefab, transform);
            obj.transform.position = singleCardTrans[i];

            Card card = obj.GetComponent<Card>();
            card.SetManager(this, gameController);
            card.Init(GameSettings.DoubleCardCount + i, true);
        }

        // Init card tree
        maxNumberCardOfRow = 6;
        for (int i = GameSettings.MaxBottomCount - 1; i >= 1; i--) {
            for (int j = maxNumberCardOfRow - 1; j >= 0; j--) {
                Card leftCard = null, righCard = null;
                int l = j - 1, r = j;
                if (l >= 0) {
                    leftCard = array[i - 1, l];
                }
                if (r < maxNumberCardOfRow - 1) {
                    righCard = array[i - 1, r];
                }
                cardTrees.Add(array[i, j], new TreeNode(leftCard, righCard));
            }
            maxNumberCardOfRow--;
        }
    }
    
    public void AddSelectedCard(int cardIndex) {
        selectedCardIndexes.Add(cardIndex);
    }

    public bool IsSelectAllCard() {
        return selectedCardIndexes.Count == GameSettings.SingleCardCount + GameSettings.DoubleCardCount;
    }

    public void OpenNewCard(Card selectedCard) {
        if (cardTrees.ContainsKey(selectedCard)) {
            TreeNode treeNode = cardTrees[selectedCard];
            if (treeNode != null) {
                if (treeNode.leftCard != null) {
                    if (!treeNode.leftCard.IsFaceUp) {
                        treeNode.leftCard.FaceUp();
                        return;
                    }
                }
                else if (treeNode.rightCard != null) {
                    if (!treeNode.rightCard.IsFaceUp) {
                        treeNode.rightCard.FaceUp();
                        return;
                    }
                }
            }
        }
    }

    public List<Sprite> GetAnimalSprites() => animalSprites;

    public (int, int) GetAnimalIndex(int index) {
        if (index < 0 || index >= animalPair.Length)
            return (-1,  -1);
        return animalPair[index];
    }

    public void AddValidCard(Card card) {
        if (!validCards.Contains(card)) {
            validCards.Add(card);
        }
    }

    public void RemoveValidCard(Card card) {
        if (validCards.Contains(card)) {
            validCards.Remove(card);
        }
    }

    public bool HasValidCard() {
        return validCards.Count > 0;
    }
}
