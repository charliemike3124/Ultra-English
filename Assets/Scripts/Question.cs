using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/new question")]
public class Question : ScriptableObject
{

    [System.Serializable]
    public struct Answer
    {
        [SerializeField] private string _info;
        public string Info { get { return _info; } }

        [SerializeField] private bool isCorrect;
        public bool IsCorrect { get { return isCorrect; } }

        


    }

    public enum AnswerType { Multi, Single}

    [SerializeField] private string _info = string.Empty;
    public string Info { get { return _info; } }

    [SerializeField] Answer[] _answers = null;
    public Answer[] Answers { get { return _answers; } }

    // Parameters
    [SerializeField] private bool _useTimer = false;
    public bool UseTimer { get { return _useTimer;} }


    [SerializeField] private int _timer = 0;
    public int Timer { get { return _timer; } }

    [SerializeField] private AnswerType _answerType = AnswerType.Multi;
    public AnswerType GetAnswerType { get { return _answerType; } }

    [SerializeField] private int _addScore = 50;
    public int AddScore { get { return _addScore; } }

    public List<int> GetCorrectAnswers()
    {
        List<int> CorrectAnswers = new List<int>();
        for (int i = 0; i < Answers.Length; i++)
        {
            if (Answers[i].IsCorrect)
            {
                if (Answers[i].IsCorrect)
                {
                    CorrectAnswers.Add(i);
                }
            }
        }
        return CorrectAnswers;
    }

}
