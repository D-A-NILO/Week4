using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Dialogue Stream", menuName = "Dialogue Stream")]

[Serializable]
public class DialogueStream : MonoBehaviour
{
    [Tooltip("List if dialogue that will be played in order if uninteruppted")]
    public Dialogue[] dialogue;
    [Tooltip("The stream to be played next, leave empty to close dialogue")]
    public DialogueStream nextStream; 
}
