using System;
using System.Collections.Generic;
using UnityEngine;

public enum HeadType {
    Left, Right
}

public class GameController : MonoBehaviour
{
    [SerializeField] CardManager cardManager;
    [SerializeField] StepCounter stepCounter;
    [SerializeField] CircleRopeManager circleObject;

    public Action OnCardChanged;

    private Card currentLeftCard = null, currentRightCard = null;
    private int currentLeftIndex = -1, currentRightIndex = -1;
    private Card currentCard = null;

    private bool isFirstValid = true, isFirstPlace = true;
    private bool isDouble = false;

    private List<SingleCard> singleCards = new List<SingleCard>();

    private void Awake() {
        Reset();
    }

    private void Start() {
        cardManager.InitialCards();
        OnCardChanged?.Invoke();
    }

    private void Reset() {
        stepCounter.ResetStep();
        currentLeftCard = null;
        currentRightCard = null;
        currentLeftIndex = -1;
        currentRightIndex = -1;
        currentCard = null;
        isDouble = false;
        singleCards.Clear();
    }

    public void AddCard(Card card, bool isChooseCard = false) {
        (int first, int second) = cardManager.GetAnimalIndex(card.CardIndex);
        bool isBegin = false;

        if (currentLeftIndex == -1 && currentRightIndex == -1) {
            currentLeftIndex = first;
            currentRightIndex = second;
            currentLeftCard = card;
            currentRightCard = card;
        }
        else {
            if (card is SingleCard) {
                if (first == currentLeftIndex) {
                    currentLeftCard = card;
                    isBegin = true;
                }
                else {
                    currentRightCard = card;
                    isBegin = false;
                }
            }
            else {
                currentLeftCard.SetChoosable(false);
                currentRightCard.SetChoosable(false);

                if (isChooseCard) {
                    AddCardWithChoose(ref card, ref isBegin);
                }
                else {
                    CalculateValidCard(card);

                    if (isDouble) {
                        currentCard = card;
                        currentLeftCard.SetChoosable(true);
                        currentRightCard.SetChoosable(true);
                        return;
                    }

                    if (isFirstValid) {
                        if (isFirstPlace) {
                            SetHeadIndex(HeadType.Left, card, second);
                            card.Swap();
                            isBegin = true;
                        }
                        else {
                            SetHeadIndex(HeadType.Right, card, second);
                        }
                    }
                    else {
                        if (isFirstPlace) {
                            SetHeadIndex(HeadType.Left, card, first);
                            isBegin = true;
                        }
                        else {
                            SetHeadIndex(HeadType.Right, card, first);
                            card.Swap();
                        }
                    }
                }
            }
        }

        if (card is SingleCard singleCard) {
            singleCards.Add(singleCard);
        }
        else {
            cardManager.OpenNewCard(card);
        }

        cardManager.AddSelectedCard(card.CardIndex);

        (first, second) = cardManager.GetAnimalIndex(card.CardIndex);
        bool isFullCircle = false;
        if (currentLeftIndex == currentRightIndex && singleCards.Count > 0)
            isFullCircle = true;
        circleObject.AddObjectToCircle(card.gameObject, isBegin, isFullCircle);

        foreach (SingleCard tmpCard in singleCards) {
            tmpCard.SetFly(true);
        }

        if (isFullCircle) {
            Reset();
        }
        else stepCounter.IncreaseStep();

        card.SetSelectable(false);
        card.SetSelected();
        OnCardChanged?.Invoke();

        CheckWin();
    }

    private void AddCardWithChoose(ref Card card, ref bool isBegin) {
        (int currentFirst, int currentSecond) = cardManager.GetAnimalIndex(currentCard.CardIndex);

        if (card == currentLeftCard) {
            if (currentFirst == currentLeftIndex) {
                SetHeadIndex(HeadType.Left, currentCard, currentSecond);
                currentCard.Swap();
            }
            else {
                SetHeadIndex(HeadType.Left, currentCard, currentFirst);
            }
            isBegin = true;
        }
        else {
            if (currentFirst == currentRightIndex) {
                SetHeadIndex(HeadType.Right, currentCard, currentSecond);
            }
            else {
                SetHeadIndex(HeadType.Right, currentCard, currentFirst);
                currentCard.Swap();
            }
        }

        card = currentCard;
    }

    private void SetHeadIndex(HeadType headType, Card card, int index) {
        switch (headType) {
            case HeadType.Left:
                currentLeftCard = card;
                currentLeftIndex = index;
                break;
            case HeadType.Right:
                currentRightCard = card;
                currentRightIndex = index;
                break;
        }
    }

    public bool IsCardValid(Card card) {
        SingleCard singleCard = card as SingleCard;

        if (currentLeftIndex == -1 && currentRightIndex == -1) {
            if (singleCard != null) {
                return false;
            }
            return true;
        }

        (int firstIndex, int secondIndex) = cardManager.GetAnimalIndex(card.CardIndex);

        bool firstValid = firstIndex == currentLeftIndex || firstIndex == currentRightIndex;
        bool secondValid = secondIndex == currentLeftIndex || secondIndex == currentRightIndex;

        return firstValid || secondValid;
    }

    public void CalculateValidCard(Card card) {
        (int firstIndex, int secondIndex) = cardManager.GetAnimalIndex(card.CardIndex);

        bool firstValid = firstIndex == currentLeftIndex || firstIndex == currentRightIndex;
        bool secondValid = secondIndex == currentLeftIndex || secondIndex == currentRightIndex;

        if (firstValid && secondValid) {
            isDouble = true;
        }
        else {
            isDouble = false;
            if (firstValid) {
                isFirstValid = true;
                if (firstIndex == currentLeftIndex) {
                    isFirstPlace = true;
                }
                else {
                    isFirstPlace = false;
                }
            }
            else if (secondValid) {
                isFirstValid = false;
                if (secondIndex == currentLeftIndex) {
                    isFirstPlace = true;
                }
                else {
                    isFirstPlace = false;
                }
            }
        }
    }

    public bool IsValidCircle(int index) {
        return currentLeftIndex == currentRightIndex && currentLeftIndex == index;
    }

    public void CheckWin() {
        if (cardManager.IsSelectAllCard())
            GameManager.Instance.FinishGame(true);
        else if (stepCounter.IsOutOfStep()) {
            if (currentLeftIndex != -1 && currentLeftIndex != -1)
                GameManager.Instance.FinishGame(false);
        }
        else if (!cardManager.HasValidCard()) {
            GameManager.Instance.FinishGame(false);
        }
    }
}
