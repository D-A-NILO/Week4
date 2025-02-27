using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class Dialogue
{
    [Tooltip("The name displayed with the dialogue")]
    public string name;

    [Tooltip("The icon displayed with the dialogue")]
    public Sprite icon;

    [Tooltip("The text of the dialogue")]
    [TextArea] public string text;

    [Tooltip("The Layout of the dialogue buttons")]
    public Layout layout = Layout.Horizontal; //default to horizontal
    
    [Tooltip("Options available to dialogue")]
    public DialogueOption[] options;

    [Tooltip("Events executed after user continues, does not execute if an option changes stream")]
    public UnityEvent OnDialogueComplete;

    [Tooltip("If the player can simply click to continue to next dialogue, automatically set to false if dialogue has options")]
    public bool clickToContinue = true;

    //unimplemented
    //public bool waitForInput = true;




    public Dialogue(string name = null, Sprite icon = null, string text = null,params DialogueOption[] options)
    {
        this.name = name;
        this.icon = icon;
        this.text = text;
        this.options = options;
        if(options.Length > 0) clickToContinue = false;
    }

    #region Dialogue Option

    [Serializable]
    /// <summary>
    /// Container for the text and event of an option/button. Can be left empty to have no options.
    /// </summary>
    public class DialogueOption
    {
        [Tooltip("Text displayed on option button")]
        public string optionText;

        [Tooltip("Switch to stream if option chosen, leave empty to not change stream")]
        public DialogueStream optionStream;

        [Tooltip("Events executed if option chosen")]
        public UnityEvent optionEvent = new UnityEvent();

        public DialogueOption(string Text, UnityAction action = null)
        {
            optionText = Text;

            if (action != null)
                optionEvent.AddListener(action);
        }


        /// <summary>
        /// Attaches a new delegate to the dialgue option
        /// </summary>
        /// <param name="Event">The new delegate to use</param>
        public void addAction(UnityAction action)
        {
            optionEvent.AddListener(action);
        }

        
        /// <summary>
        /// calls the event attached to this dialogue option and play the stream assossiated if not empty
        /// </summary>
        public void Invoke()
        {
            optionEvent.Invoke();
            if (optionStream != null)
                DialogueManager.Instance.PlayStream(optionStream);
        }
    }

    #endregion

    public enum Layout
    {
        Horizontal,
        Vertical
    }


    public void setAll(string Name, string Text, Sprite Icon, params DialogueOption[] Options)
    {
        name = Name;
        text = Text;
        icon = Icon;
        options = Options;
        if(options.Length > 0) clickToContinue = false;
    }

}
