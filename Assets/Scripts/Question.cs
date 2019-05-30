using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnswerType { Multi, Single }

[System.Serializable]
public class Answer
{
    public string Info = string.Empty;
    public bool IsCorrect = false;

    public Answer()
    {
        //constructor para el xml
    }
}

[System.Serializable]
public class Question
{

    public string Info = null;
    public Answer[] Answers = null;
    public bool UseTimer = false;
    public int Timer = 0;
    public AnswerType Type = AnswerType.Single;
    public int AddScore = 0;

    public Question()
    {
        //constructor para el xml
    }

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
