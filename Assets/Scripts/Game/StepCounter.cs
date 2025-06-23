using System.Collections.Generic;
using UnityEngine;

public class StepCounter : MonoBehaviour
{
    [SerializeField] List<Sprite> handStepSprites = new List<Sprite>();

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private int maxStep;
    private int currentStep;

    private void Awake() {
        maxStep = handStepSprites.Count;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start() {
        ResetStep();
    }

    public void ResetStep() {
        currentStep = 0;
        UpdateHandVisual();
    }

    public void IncreaseStep() {
        currentStep += 1;
        if (currentStep < maxStep) {
            UpdateHandVisual();
        }
    }

    private void UpdateHandVisual() {
        spriteRenderer.sprite = handStepSprites[currentStep];
        animator.SetTrigger("HandChange");
    }

    public bool IsOutOfStep() {
        return currentStep == maxStep;
    }
}
