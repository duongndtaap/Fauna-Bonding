using System.Collections;
using TMPro;
using UnityEngine;

public class FinishGameDialog : Dialog
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text bestScoreText;

    [SerializeField] int maxScore;

    float timer;

    protected override void Awake() {
        base.Awake();
        GameManager.Instance.OnGameFinished += FinishGame;

        timer = Time.time;
    }

    private void FinishGame(bool isWin) {
        StartCoroutine(FinishHandle(isWin));
    }

    private IEnumerator FinishHandle(bool isWin) {
        yield return new WaitForSeconds(1f);

        int bestScore = PlayerPrefs.GetInt("BestScore");
        if (isWin) {
            float totalTime = Time.time - timer;
            int score = (int)(maxScore / totalTime);
            if (score > bestScore) {
                bestScore = score;
                PlayerPrefs.SetInt("BestScore", bestScore);
            }

            scoreText.text = score.ToString();
        }
        else {
            scoreText.text = "0";
        }
        bestScoreText.text = bestScore.ToString();
        Show();
    }

    private void OnDisable() {
        GameManager.Instance.OnGameFinished -= FinishGame;
    }
}
