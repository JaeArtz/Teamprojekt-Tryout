/* --------------------------------------------------
                Source: Tutorial from
                unitycodemonkey.com
            Code Monkey Youtube Tutorial
        "How to make Text Writing Effect in Unity"
    --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWriter : MonoBehaviour
{

    private static TextWriter instance;
    private List<TextWriterSingle> textWriterSingleList;

    private void Awake()
    {
        instance = this;
        textWriterSingleList = new List<TextWriterSingle>();
    }

    public static TextWriterSingle AddWriter_Static(TextMeshProUGUI uiText, string textToWrite, float timePerCharacter, bool invisibleCharacters, bool removeWriterBeforeAdd, Action onComplete)
    {
        if (removeWriterBeforeAdd)
        {
            instance.RemoveWriter(uiText);
        }
        return instance.AddWriter(uiText, textToWrite, timePerCharacter, invisibleCharacters, onComplete);
    }

    private TextWriterSingle AddWriter(TextMeshProUGUI uiText, string textToWrite, float timePerCharacter, bool invisibleCharacters, Action onComplete)
    {
        TextWriterSingle textWriterSingle = new TextWriterSingle(uiText, textToWrite, timePerCharacter, invisibleCharacters, onComplete);
        textWriterSingleList.Add(textWriterSingle);
        return textWriterSingle;
    }

    public static void RemoveWriter_Static(TextMeshProUGUI uiText)
    {
        instance.RemoveWriter(uiText);
    }

    private void RemoveWriter(TextMeshProUGUI uiText)
    {
        for (int i = 0; i < textWriterSingleList.Count; i++)
        {
            if (textWriterSingleList[i].GetUIText() == uiText)
            {
                textWriterSingleList.RemoveAt(i);
                i--;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < textWriterSingleList.Count; i++)
        {
            bool destroyInstance = textWriterSingleList[i].Update();
            if (destroyInstance)
            {
                textWriterSingleList.RemoveAt(i);
                i--;
            }
        }
    }

    public class TextWriterSingle
    {

        private TextMeshProUGUI uiText;
        private string textToWrite;
        private int characterIndex;
        private float timePerCharacter;
        private float timer;
        private Action onComplete;
        private int totalVisibleCharacters;

        public TextWriterSingle(TextMeshProUGUI uiText, string textToWrite, float timePerCharacter, bool invisibleCharacters, Action onComplete)
        {
            this.uiText = uiText;
            this.textToWrite = textToWrite;
            this.timePerCharacter = timePerCharacter;
            this.onComplete = onComplete;

            // 1. SCHRITT: Den Text komplett an TMP übergeben (Hintergrund-Verarbeitung)
            uiText.text = textToWrite;
            uiText.ForceMeshUpdate(); // Erzwingt die sofortige Berechnung von Layout & Tags

            // 2. SCHRITT: Alles unsichtbar machen
            this.totalVisibleCharacters = uiText.textInfo.characterCount;
            uiText.maxVisibleCharacters = 0;
            characterIndex = 0;
        }

        public bool Update()
        {
            if (uiText == null) return true;

            timer -= Time.deltaTime;
            while (timer <= 0f)
            {
                timer += timePerCharacter;
                characterIndex++;

                // 3. SCHRITT: Zeichen für Zeichen sichtbar machen
                uiText.maxVisibleCharacters = characterIndex;

                if (characterIndex >= totalVisibleCharacters)
                {
                    if (onComplete != null) onComplete();
                    return true;
                }
            }
            return false;
        }

        public TextMeshProUGUI GetUIText() { return uiText; }

        public bool IsActive() { return characterIndex < totalVisibleCharacters; }

        public void WriteAllAndDestroy()
        {
            uiText.maxVisibleCharacters = totalVisibleCharacters;
            characterIndex = totalVisibleCharacters;
            if (onComplete != null) onComplete();
            TextWriter.RemoveWriter_Static(uiText);
        }
    }
}

/*
 * 
So kann man Text formattieren, indem man ihn passend in die String-Arrays eingibt:

Fett: <b>Dein Text</b> → Dein Text

Kursiv: <i>Dein Text</i> → Dein Text

Unterstrichen: <u>Dein Text</u> → <u>Dein Text</u>

Durchgestrichen: <s>Dein Text</s> → ~~Dein Text~~

Zeilenumbruch: <br>, z.B. Press A - Move Left <br> Press B - Move Right

Standard-Farben: <color=red>Roter Text</color>

Funktioniert mit: red, green, blue, yellow, orange, black, white.

Hex-Codes (Präzise): <color=#FF00FF>Pinker Text</color>

Transparenz (Alpha): <alpha=#88>Halbdurchsichtiger Text



Absolute Größe: <size=24>Kleiner Text</size>

Relative Größe: <size=150%>Großer Text</size>

Tipp: Perfekt für Überschriften oder Tasten-Symbole.

Hochgestellt: E=mc<sup>2</sup>

Tiefgestellt: H<sub>2</sub>O
 * 
 * 
 * 
 * Das hier funktioniert: Im TExt TMP => Autosize, Min 6, Max 12 (normale Fontgröße 12)
 <i>Press</i> <color=red>A</color> <i>- Move Left<br>Press</i> <color=red>D</color><i> - Move Right<br>Press </i><color=red>SPACE</color> <i>- Jump</i>
 
 
 */