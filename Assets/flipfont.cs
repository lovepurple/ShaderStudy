using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class flipfont : MonoBehaviour
{

    Text myText; //You can also make this public and attach your UI text here.

    string individualLine = ""; //Control individual line in the multi-line text component.

    int numberOfAlphabetsInSingleLine = 20;

    string sampleString = "I am trying to write text in Hebrew," +
                          "But there is a problem. Unity allows me to write text in direction from Left to Right, " +
                          "But I can't write from Right to Left. And I can't to do something good with the code, " +
                          "Because I think It is impossible.";

    void Awake()
    {
        myText = GetComponent<Text>();
    }

    void Start()
    {

        List<string> listofWords = sampleString.Split(' ').ToList(); //Extract words from the sentence

        foreach (string s in listofWords)
        {

            if (individualLine.Length >= numberOfAlphabetsInSingleLine)
            {
                myText.text += Reverse(individualLine) + "\n"; //Add a new line feed at the end, since we cannot accomodate more characters here.
                individualLine = ""; //Reset this string for new line.
            }

            individualLine += s + " ";

        }

    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

}