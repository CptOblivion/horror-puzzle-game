using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class CutsceneMoveHelper
{
  public GameObject Obj;
  public Vector3 Movement;
  public float Time = 0;
  public float Progress = 0;
}
class CutsceneMoveToHelper
{
  public GameObject Obj;
  public Vector3 Origin;
  public Vector3 Destination;
  public bool Local = false;
  public float Time = 0;
  public float Progress = 0;
  public bool SnapToGround = false;
  public string Animation = null;
}
class CutsceneTurnToHelper
{
  public GameObject Obj;
  public Quaternion Origin = new Quaternion();
  public Quaternion Destination = new Quaternion();
  public bool Local = false;
  public float Time = 0;
  public float Progress = 0;
}

public class CutscenePlayer : MonoBehaviour
{
  public TextAsset CutsceneFile;
  public GameObject[] ControlledObjects;
  public MoveCameraToPosition[] CameraPositions;
  public bool ActivateWithObject = false;
  public bool verbose = false;

  public static bool CutscenePlaying;
  public static CutscenePlayer currentCutsceneManager;



  string[] ControlledObjectNames;
  string[] CameraNames;

  List<string> CutsceneScriptParsed;
  List<CutsceneMoveHelper> ActiveMoves;
  List<CutsceneMoveToHelper> ActiveMoveTos;
  List<CutsceneTurnToHelper> ActiveTurnTos;

  int CurrentLine; //the command number the "playhead" is on
  float CutsceneStartTime; //game time when the cutscene started (probably won't use)
  float CutsceneTime; //current timestamp for the cutscene
  int DialogueStartLine; //we'll jump back to here when we loop the dialogue
  float DialogueStartTime; //storing the time here is easier than finding it in the line
  bool DialogueOpen;
  int DialogueIteration; //track the number of times we've looped on the current dialogue
  bool DialogueCleared; //track if the player has hit confirm on the dialogue
  bool Paused; //dialogue has its own special pause screen, so we'll track pauses separately from the global pause
  bool LoadingLevel = false;
  bool WasPaused = false;
  bool SkipToEnd = false; //when true, parse commands instantly (skip loops, dialogue, anything that doesn't affect the final state of objects)

  char[] charWhiteSpace = new char[] { ' ', '\t' };
  char[] charEquals = new char[] { '=' };
  char[] charColon = new char[] { ':' };
  char[] charComma = new char[] { ',' };
  string[] newLine = new string[] { "\r\n", "\n" };
  string[] GlobalCommands = new string[] { "Dialogue", "EndDialogue", "CameraChange", "PlaySound", "LoadLevel", "EndCutscene" };
  string[] ObjectCommands = new string[] { "Move", "MoveTo", "TurnTo", "Animation", "Animator", "SetActive" };

  //SkipToEnd commands: LoadLevel, EndCutscene, MoveTo, TurnTo, SetActive
  //Try to avoid using Move without then following up with a MoveTo, to ensure final position doesn't get wonky

  private void Awake()
  {
    Disable();
  }
  private void Start()
  {
    CutscenePlayerObjects.cutscenePlayerObjects.cutscenePause.SetActive(false);
    //parsing the cutscene script file into pieces
    {
      CutsceneScriptParsed = new List<string>();
      ControlledObjectNames = new string[ControlledObjects.Length];
      CameraNames = new string[CameraPositions.Length];
      ActiveMoves = new List<CutsceneMoveHelper>();
      ActiveMoveTos = new List<CutsceneMoveToHelper>();
      ActiveTurnTos = new List<CutsceneTurnToHelper>();

      //break the script into an array of strings
      //should we remove the empty lines? Probably
      string[] CutsceneScriptLines = CutsceneFile.text.Split(newLine, System.StringSplitOptions.None);
      int LineIndex = 0;
      string CurrentLine;

      //first, the metadata portion of the script file
      while (!CutsceneScriptLines[LineIndex].Contains("SCRIPT"))
      {
        CurrentLine = CutsceneScriptLines[LineIndex];

        //assign script-writer friendly names to the objects
        if (CurrentLine.StartsWith("ObNames:"))
        {
          CurrentLine = CurrentLine.Trim(charWhiteSpace);
          CurrentLine = CurrentLine.Substring(8); //trim "ObNames:"
          string[] ObNames = CurrentLine.Split(charComma);
          for (int i = 0; i < ObNames.Length; i++)
          {
            //check the assigned object names to make sure they're safe
            string TestName = ObNames[i].Trim(charWhiteSpace);
            bool pass = true;
            foreach (string command in GlobalCommands)
            {
              if (command == TestName)
              {
                pass = false;
                break;
              }
            }
            foreach (string command in ObjectCommands)
            {
              if (command == TestName)
              {
                pass = false;
                break;
              }
            }

            if (pass)
            {
              ObNames[i] = TestName;
            }
            else
            {
              Debug.LogError("Invalid object name in cutscene script " + CutsceneFile.name + ": " + TestName + " is a command!", gameObject);
              ObNames[i] = i.ToString();
            }
          }
          for (int i = 0; i < ControlledObjectNames.Length; i++)
          {
            if (i < ObNames.Length)
            {
              ControlledObjectNames[i] = ObNames[i];
            }
            else
            {
              ControlledObjectNames[i] = i.ToString();
            }
          }
        }

        //assign script-writer friendly names to the camera angles
        else if (CurrentLine.StartsWith("CamNames"))
        {
          CurrentLine = CurrentLine.Trim(charWhiteSpace);
          CurrentLine = CurrentLine.Substring(9); //trim "CamNames:"
          string[] CamNames = CurrentLine.Split(charComma);
          for (int i = 0; i < CamNames.Length; i++)
          {
            //check the assigned object names to make sure they're safe
            string TestName = CamNames[i].Trim(charWhiteSpace);
            bool pass = true;
            foreach (string command in GlobalCommands)
            {
              if (command == TestName)
              {
                pass = false;
                break;
              }
            }
            foreach (string command in ObjectCommands)
            {
              if (command == TestName)
              {
                pass = false;
                break;
              }
            }

            if (pass)
            {
              CamNames[i] = TestName;
            }
            else
            {
              Debug.LogError("Invalid camera name in cutscene script " + CutsceneFile.name + ": " + TestName + " is a command!", gameObject);
              CamNames[i] = i.ToString();
            }
          }
          for (int i = 0; i < CameraNames.Length; i++)
          {
            if (i < CamNames.Length)
            {
              CameraNames[i] = CamNames[i];
            }
            else
            {
              CameraNames[i] = i.ToString();
            }
          }
        }


        LineIndex++;
      }
      LineIndex++;//skip the "SCRIPT" line

      //now, the script portion of the script:
      float[] CutsceneTimeTemp = new float[] { 0, 0, 0, 0 };
      int IndentLevel;
      int LastIndentLevel = 0;
      bool InDialogueTemp = false;

      for (; LineIndex < CutsceneScriptLines.Length; LineIndex++)
      {
        CurrentLine = CutsceneScriptLines[LineIndex];

        if (CurrentLine.Contains("//"))
        {
          int commentIndex = CurrentLine.IndexOf("//");
          CurrentLine = CurrentLine.Substring(0, commentIndex);
        }

        CurrentLine = CurrentLine.TrimEnd(charWhiteSpace);

        //if the line is empty now, it was blank or just a comment- skip it
        if (CurrentLine != "")
        {
          //now we convert the line to absolute time

          IndentLevel = 0;
          while (CurrentLine.StartsWith("\t"))
          {
            IndentLevel++;
            CurrentLine = CurrentLine.Remove(0, 1);//cut out one leading tab
          }

          string[] SplitLine;

          if (IndentLevel > 0 || CurrentLine.StartsWith("+")) //if this line is relative time
          {
            if (CurrentLine.StartsWith("+"))
            {
              CurrentLine = CurrentLine.Remove(0, 1); //trim the +
            }
            else
            {
              //any indented line is treated as relative time even if it doesn't have a +, but we'll warn the developer that they should have a + for clarity
              Debug.Log("don't forget that leading +, buddy! Line " + (LineIndex + 1));
            }

            //if indentlevel has changed, we work our way down the levels in cutscenetime
            while (IndentLevel < LastIndentLevel)
            {
              for (int i = 0; i <= LastIndentLevel; i++)
              {
                CutsceneTimeTemp[i] -= CutsceneTimeTemp[LastIndentLevel];
              }
              LastIndentLevel--;
            }
            LastIndentLevel = IndentLevel;

            SplitLine = CurrentLine.Split(charWhiteSpace, 2);

            for (int i = 0; i <= IndentLevel; i++)
            {
              CutsceneTimeTemp[i] += float.Parse(SplitLine[0]);
            }

            SplitLine[0] = CutsceneTimeTemp[0].ToString();

          }
          else //if the line is absolute time
          {
            LastIndentLevel = 0; //no indents allowed!
            for (int i = 0; i < CutsceneTimeTemp.Length; i++) CutsceneTimeTemp[i] = 0; //zero out cutscene time
            CutsceneTimeTemp[0] = float.Parse(CurrentLine.Split(charWhiteSpace)[0]); //set the tracked time to the line time
            SplitLine = CurrentLine.Split(charWhiteSpace, 2);
            //no need to assign absolute time to the line since it already is absolute
          }

          if (SplitLine[1].StartsWith("Dialogue"))
          {
            InDialogueTemp = true;
          }
          else if (SplitLine[1].StartsWith("EndDialogue"))
          {
            InDialogueTemp = false;
          }
          else if (InDialogueTemp)
          {
            SplitLine[0] = "D " + SplitLine[0];
          }
          CurrentLine = SplitLine[0] + " " + SplitLine[1];

          //Debug.Log(CurrentLine + "\n" + CutsceneTime[0] + ", " + CutsceneTime[1] + ", " + CutsceneTime[2] + ", " + CutsceneTime[3]);
          //debug.log(CurrentLine);

          //add the line number to the line (for debug purposes)
          CurrentLine = (LineIndex + 1).ToString() + " " + CurrentLine;
          CutsceneScriptParsed.Add(CurrentLine);

        }
      }

      //now we rearrange the lines based on their absolute time
      //this is about as naive as a sorting algorithm can get, but it does ensure that in the case of a timestamp tie, the order of the lines is maintained
      //that's very important since a lot of instructions can happen in the same frame and they need to happen in order
      List<string> ScriptLinesUnsorted = CutsceneScriptParsed;
      CutsceneScriptParsed = new List<string>();
      float LowestTime;
      int LowestTimeLine;
      int breakout = 100;
      while (ScriptLinesUnsorted.Count > 0)
      {
        LowestTime = 9999;
        LowestTimeLine = 0;
        for (int i = 0; i < ScriptLinesUnsorted.Count; i++)
        {
          string[] LineSplit = ScriptLinesUnsorted[i].Split(charWhiteSpace, 3); //LineSplit is [LineNumber, TimeStamp, RestOfTheLine]
          if (LineSplit[1] == "D")//if we get an "in dialogue" flag instead of a timestamp:
          {
            //the timestamp is at the start of RestOfTheLine, so we'll just replace the middle index with the timestamp
            LineSplit[1] = LineSplit[2].Split(charWhiteSpace, 2)[0];
          }
          if (float.Parse(LineSplit[1]) < LowestTime)
          {
            LowestTime = float.Parse(LineSplit[1]);
            LowestTimeLine = i;
          }
        }
        CutsceneScriptParsed.Add(ScriptLinesUnsorted[LowestTimeLine]);
        ScriptLinesUnsorted.RemoveAt(LowestTimeLine);

        breakout--;
        if (breakout == 0)
        {
          Debug.Log("infinite loop!");
          break;
        }
      }

      /*
      foreach(string line in CutsceneScriptParsed)
      {
          Debug.Log(line);
      }
      */
    }

  }
  void Update()
  {
    if (!Paused)
    {
      if (GlobalTools.inputsGameplay.FindAction("Cancel").triggered)
      {
        Paused = true;
        Time.timeScale = 0;
        CutscenePlayerObjects.cutscenePlayerObjects.cutscenePause.SetActive(true);
      }
    }
    else
    {

      if (GlobalTools.inputsGameplay.FindAction("Cancel").triggered)
      {
        ResumeFromPause();
      }
    }
    if (!Paused)
    {

      if (DialogueOpen && !DialogueCleared)
      {
        if (GlobalTools.inputsGameplay.FindAction("Submit").triggered && !WasPaused)
        {
          //Debug.Log(WasPaused);
          DialogueCleared = true;
          CutscenePlayerObjects.cutscenePlayerObjects.dialogueText.transform.parent.gameObject.SetActive(false);
        }
      }

      string[] Line;
      int LinePos;

      int FileLineNumber; //for debug purposes
      bool DialogueFlag; //whether or not this line should repeat during dialogue
      float Timestamp;
      GameObject obj;
      string Command;

      //if we've reached the end of the cutscene
      if (CurrentLine >= CutsceneScriptParsed.Count)
      {
        Disable();
      }

      else
      {
        while (true)
        {
          if (SkipToEnd)
          {
            CurrentLine++;
            if (CurrentLine >= CutsceneScriptParsed.Count)
            {
              Debug.Log("cutscene " + CutsceneFile.name + " skipped");
              break;
            }
          }
          Line = CutsceneScriptParsed[CurrentLine].Split(charWhiteSpace);
          LinePos = 0;

          FileLineNumber = int.Parse(Line[LinePos]);
          LinePos++;

          DialogueFlag = false;
          if (Line[LinePos] == "D")
          {
            DialogueFlag = true;
            LinePos++;
          }

          Timestamp = float.Parse(Line[LinePos]);
          LinePos++;

          if (!SkipToEnd && Timestamp > CutsceneTime) //we haven't reached the next command yet
          {
            break;
          }

          //establish the object to be acted upon, if it exists:
          obj = null;
          //check the first line of the command against the objects list (which has already been filtered to not to contain commands, so no need to also check that it's not a command)
          if (System.Array.Exists(ControlledObjectNames, s => s.Equals(Line[LinePos])))
          {
            obj = ControlledObjects[System.Array.IndexOf(ControlledObjectNames, Line[LinePos])];
            LinePos++;
          }

          Command = Line[LinePos];
          LinePos++;

          //if we're currently in a dialogue line and the current line is not flagged to loop during dialogue AND we've already looped the dialogue commands once, skip to the next line
          if (DialogueOpen && !DialogueFlag && DialogueIteration > 0 && Command != "EndDialogue")
          {
            if (!SkipToEnd) CurrentLine++;
            continue;
          }

          int ArgsCount = Line.Length - LinePos;
          string[] args = new string[] { };
          if (ArgsCount > 0)
          {
            args = new string[ArgsCount];
            for (int i = 0; i < args.Length; i++, LinePos++)
            {
              args[i] = Line[LinePos];
            }
          }

          int ArgIndex = 0;

          //check for and execute command
          if (Command == "Move")
          {
            if (SkipToEnd) continue;
            CutsceneMoveHelper newMove = new CutsceneMoveHelper();
            newMove.Obj = obj;
            string[] MoveAxes = args[ArgIndex].Split(charComma);
            newMove.Movement = new Vector3(float.Parse(MoveAxes[0]), float.Parse(MoveAxes[1]), float.Parse(MoveAxes[2]));
            ArgIndex++;

            newMove.Time = float.Parse(args[ArgIndex]);
            ArgIndex++;

            ActiveMoves.Add(newMove);

            if (verbose) Debug.Log($"{FileLineNumber} moving {obj}");
          }

          else if (Command == "MoveTo")
          {
            CutsceneMoveToHelper newMoveTo = new CutsceneMoveToHelper();
            newMoveTo.Obj = obj;

            string[] PostitionAxes = args[ArgIndex].Split(charComma);
            newMoveTo.Destination = new Vector3(float.Parse(PostitionAxes[0]), float.Parse(PostitionAxes[1]), float.Parse(PostitionAxes[2]));
            ArgIndex++;

            newMoveTo.Time = float.Parse(args[ArgIndex]);
            ArgIndex++;

            string[] OptionalArgs = new string[args.Length - ArgIndex];
            System.Array.Copy(args, ArgIndex, OptionalArgs, 0, args.Length - ArgIndex);
            string CurrentArg;
            for (int i = 0; i < OptionalArgs.Length; i++)
            {
              CurrentArg = OptionalArgs[i];
              if (CurrentArg.StartsWith("Animation"))
              {
                newMoveTo.Animation = CurrentArg.Split(charEquals)[1];
              }
              else if (CurrentArg.StartsWith("GroundSnap"))
              {
                newMoveTo.SnapToGround = bool.Parse(CurrentArg.Split(charEquals)[1]);
              }
              else if (OptionalArgs[i].StartsWith("Local"))
              {
                newMoveTo.Local = bool.Parse(CurrentArg.Split(charEquals)[1]);
              }
              else
              {
                Debug.LogError("Unrecognized optional argument in " + Command + ": " + OptionalArgs[i].Split(charEquals)[0] + "! Cutscene script: " + CutsceneFile.name + ", line: " + FileLineNumber, gameObject);
              }
            }

            if (newMoveTo.Local)
            {
              newMoveTo.Origin = obj.transform.localPosition;
            }
            else
            {
              newMoveTo.Origin = obj.transform.position;
            }

            ActiveMoveTos.Add(newMoveTo);
            if (verbose) Debug.Log($"{FileLineNumber} Moving {obj} to {newMoveTo.Destination}");
          }
          else if (Command == "TurnTo")
          {
            CutsceneTurnToHelper newTurnTo = new CutsceneTurnToHelper();
            newTurnTo.Obj = obj;

            string[] RotationAxes = args[ArgIndex].Split(charComma);
            newTurnTo.Destination = Quaternion.Euler(float.Parse(RotationAxes[0]), float.Parse(RotationAxes[1]), float.Parse(RotationAxes[2]));
            ArgIndex++;

            newTurnTo.Time = float.Parse(args[ArgIndex]);
            ArgIndex++;

            string[] OptionalArgs = new string[args.Length - ArgIndex];
            System.Array.Copy(args, ArgIndex, OptionalArgs, 0, args.Length - ArgIndex);
            string CurrentArg;
            for (int i = 0; i < OptionalArgs.Length; i++)
            {
              CurrentArg = OptionalArgs[i];
              if (CurrentArg.StartsWith("Local"))
              {
                newTurnTo.Local = bool.Parse(CurrentArg.Split(charEquals)[1]);
              }
              else
              {
                Debug.LogError("Unrecognized optional argument in " + Command + ": " + OptionalArgs[i].Split(charEquals)[0] + "! Cutscene script: " + CutsceneFile.name + ", line: " + FileLineNumber, gameObject);
              }
            }

            if (newTurnTo.Local)
            {
              newTurnTo.Origin = obj.transform.localRotation;
            }
            else
            {
              newTurnTo.Origin = obj.transform.rotation;
            }

            ActiveTurnTos.Add(newTurnTo);

            if (verbose) Debug.Log($"{FileLineNumber} Turning {obj} to {newTurnTo.Destination.eulerAngles}");
          }
          else if (Command == "Animation")
          {
            if (SkipToEnd) continue;

            if (verbose) Debug.Log($"{FileLineNumber} Playing [animation] on {obj}");
          }

          else if (Command == "Animator")
          {
            if (SkipToEnd) continue;

            string AnimName = args[ArgIndex];
            ArgIndex++;

            float BlendTime = float.Parse(args[ArgIndex]);
            ArgIndex++;

            Animator anim = obj.GetComponentInChildren<Animator>();
            if (anim)
            {
              anim.CrossFadeInFixedTime(AnimName, BlendTime);
            }
            else
            {
              Debug.LogError("No animator in " + obj + "! cutscene script: " + CutsceneFile.name + ", line: " + FileLineNumber, gameObject);
            }
            if (verbose) Debug.Log($"{FileLineNumber} Playing {AnimName} on {obj}");
          }
          else if (Command == "SetActive")
          {

            bool active = bool.Parse(args[ArgIndex]);
            ArgIndex++;
            obj.SetActive(active);
            if (verbose)
            {
              if (active) Debug.Log($"{FileLineNumber} enabling {obj}");
              else Debug.Log($"{FileLineNumber} disabling {obj}");
            }
          }
          else if (Command == "Dialogue")
          {
            if (SkipToEnd) continue;

            if (!DialogueOpen)
            {
              DialogueCleared = false;
              DialogueIteration = 0;
              DialogueOpen = true;
              DialogueStartLine = CurrentLine;
              DialogueStartTime = CutsceneTime;

              string dialogueLine = "";
              string CurrentWord;

              for (; ArgIndex < args.Length; ArgIndex++)
              {
                CurrentWord = args[ArgIndex];
                if (ArgIndex == 0)
                {
                  CurrentWord = CurrentWord.TrimStart('\"');
                }
                if (CurrentWord.EndsWith("\""))
                {
                  dialogueLine += " " + CurrentWord.TrimEnd('\"');
                  ArgIndex++;//make sure to move on to the next arg still
                  break;
                }
                dialogueLine += " " + CurrentWord;
              }

              CutscenePlayerObjects.cutscenePlayerObjects.dialogueText.text = dialogueLine;
              RectTransform textFrame = (RectTransform)CutscenePlayerObjects.cutscenePlayerObjects.dialogueText.transform.parent;
              textFrame.sizeDelta = new Vector2(CutscenePlayerObjects.cutscenePlayerObjects.dialogueText.preferredWidth + 120, textFrame.sizeDelta.y);
              Transform dialogueArrow = CutscenePlayerObjects.cutscenePlayerObjects.DialogueContinueArrow.transform;
              Vector3 DialogueArrowPosition = dialogueArrow.localPosition;
              DialogueArrowPosition.x = CutscenePlayerObjects.cutscenePlayerObjects.dialogueText.preferredWidth / 2 + 30;
              dialogueArrow.localPosition = DialogueArrowPosition;

              textFrame.gameObject.SetActive(true);
              if (verbose) Debug.Log($"{FileLineNumber} Displaying dialogue: \"{dialogueLine}\"");
            }
            else//if dialogue is already open it means we've looped around
            {
              DialogueIteration++;
            }

          }
          else if (Command == "EndDialogue")
          {
            if (SkipToEnd)
            {
              DialogueOpen = false;
              continue;
            }

            bool Loop = true;

            string[] OptionalArgs = new string[args.Length - ArgIndex];
            System.Array.Copy(args, ArgIndex, OptionalArgs, 0, args.Length - ArgIndex);
            string CurrentArg;
            for (int i = 0; i < OptionalArgs.Length; i++)
            {
              CurrentArg = OptionalArgs[i];
              if (CurrentArg.StartsWith("Loop"))
              {
                Loop = bool.Parse(CurrentArg.Split(charEquals)[1]);
              }
              else
              {
                Debug.LogError("Unrecognized optional argument in " + Command + ": " + OptionalArgs[i].Split(charEquals)[0] + "! Cutscene script: " + CutsceneFile.name + ", line: " + FileLineNumber, gameObject);
              }
            }

            if (!DialogueCleared)
            {
              if (Loop)
              {
                CutsceneTime = DialogueStartTime;
                CurrentLine = DialogueStartLine;
              }
              else
              {
                CutsceneTime = Timestamp;
              }
              break;
            }
            else
            {
              DialogueOpen = false;
            }
            if (verbose) Debug.Log($"{FileLineNumber} Resuming from dialogue");
          }
          else if (Command == "CameraChange")
          {
            if (SkipToEnd) continue;

            string CameraAngle = args[ArgIndex];
            CameraPosition camPos = null;
            ArgIndex++;
            if (CameraAngle != "null")
            {
              if (System.Array.Exists(CameraNames, s => s.Equals(CameraAngle)))
              {
                camPos = CameraPositions[System.Array.IndexOf(CameraNames, CameraAngle)].GetComponentInChildren<CameraPosition>();
                //Debug.Log("setting camera position to " + CameraAngle);
              }
              else
              {
                Debug.LogError("Invalid Camera angle name: " + CameraAngle + " in cutscene script: " + CutsceneFile.name + ", line: " + FileLineNumber);
              }
            }
            else
            {
              //Debug.Log("Re-rendering camera at current position");
            }
            GlobalTools.currentCam.GetComponent<UpdateBG>().UpdateCamera(camPos);
            if (verbose) Debug.Log($"{FileLineNumber} Switching to camera {camPos}");
          }
          else if (Command == "PlaySound")
          {
            if (SkipToEnd) continue;

            if (verbose) Debug.Log($"{FileLineNumber} Playing sound [sound]");
          }
          else if (Command == "LoadLevel")
          {
            string LevelName = args[0];
            string[] LevelTags = null;
            LoadingLevel = true;
            if (args.Length > 1)
            {
              LevelTags = args[1].Split(charComma);
            }
            LevelLoader.LoadLevel(LevelName, LevelTags);
            if (verbose) Debug.Log($"{FileLineNumber} Loading level {LevelName}");
          }
          else if (Command == "EndCutscene")
          {
            //skip to the end of the cutscene
            CurrentLine = CutsceneScriptParsed.Count;
            if (verbose) Debug.Log($"{FileLineNumber} Cutscene end command reached");
          }
          else
          {
            Debug.Log("Command " + Command + " not valid! Script: " + CutsceneFile.name + ", Line: " + FileLineNumber, gameObject);
          }

          CurrentLine++; //on to the next line

          if (CurrentLine >= CutsceneScriptParsed.Count) //we've reached the end of the script
          {
            Debug.Log("cutscene " + CutsceneFile.name + " finished");
            break;
          }
        }
      }

      //execute over-time functions

      for (int i = 0; i < ActiveMoves.Count; i++)
      {
        if (SkipToEnd)
        {
          ActiveMoves = new List<CutsceneMoveHelper>();
          break;
        }
        CutsceneMoveHelper currentMove = ActiveMoves[i];
        float MoveMult = Time.deltaTime;
        if (currentMove.Progress + MoveMult > currentMove.Time)
        {
          MoveMult = currentMove.Time - currentMove.Progress;
        }

        Vector3 MoveVec = currentMove.Movement * MoveMult;
        currentMove.Obj.transform.Translate(MoveVec, Space.Self);


        currentMove.Progress += Time.deltaTime;
        if (currentMove.Progress > currentMove.Time)
        {
          ActiveMoves.RemoveAt(i);
          i--;
        }
      }

      for (int i = 0; i < ActiveMoveTos.Count; i++)
      {
        CutsceneMoveToHelper CurrentMoveTo = ActiveMoveTos[i];

        if (CurrentMoveTo.Time == 0 || SkipToEnd)
        {
          CurrentMoveTo.Progress = 1;
        }
        else
        {
          CurrentMoveTo.Progress += Time.deltaTime * (1 / CurrentMoveTo.Time);
        }
        Vector3 CurrentPosition = Vector3.Lerp(CurrentMoveTo.Origin, CurrentMoveTo.Destination, CurrentMoveTo.Progress);

        if (CurrentMoveTo.Local)
        {
          CurrentMoveTo.Obj.transform.localPosition = CurrentPosition;
        }
        else
        {
          CurrentMoveTo.Obj.transform.position = CurrentPosition;
        }
        if (CurrentMoveTo.SnapToGround)
        {
          CharacterController characterController = CurrentMoveTo.Obj.GetComponent<CharacterController>();
          if (characterController)
          {
            GlobalTools.SnapToGround(characterController);
          }
        }
        if (CurrentMoveTo.Progress >= 1)
        {
          ActiveMoveTos.RemoveAt(i);
          i--;
        }
      }

      for (int i = 0; i < ActiveTurnTos.Count; i++)
      {
        CutsceneTurnToHelper CurrentTurnTo = ActiveTurnTos[i];

        if (CurrentTurnTo.Time == 0 || SkipToEnd)
        {
          CurrentTurnTo.Progress = 1;
        }
        else
        {
          CurrentTurnTo.Progress += Time.deltaTime * (1 / CurrentTurnTo.Time);
        }
        Quaternion CurrentRotation = Quaternion.Lerp(CurrentTurnTo.Origin, CurrentTurnTo.Destination, CurrentTurnTo.Progress);

        if (CurrentTurnTo.Local)
        {
          CurrentTurnTo.Obj.transform.localRotation = CurrentRotation;
        }
        else
        {
          CurrentTurnTo.Obj.transform.rotation = CurrentRotation;
        }
        if (CurrentTurnTo.Progress >= 1)
        {
          ActiveTurnTos.RemoveAt(i);
          i--;
        }
      }


      CutsceneTime += Time.deltaTime;

    }

    WasPaused = false;
  }
  private void OnEnable()
  {
    if (CutscenePlaying)
    {
      Debug.Log("already playing a cutscene: " + currentCutsceneManager, gameObject);
      //gameObject.SetActive(false);

      Disable();
    }
    else
    {
      GlobalTools.Paused = true;
      CutscenePlaying = true;
      currentCutsceneManager = this;
      CutsceneStartTime = Time.unscaledTime;
      CurrentLine = 0;
      CutsceneTime = 0;
      DialogueOpen = false;
      Paused = false;
      CutscenePlayerObjects.cutscenePlayerObjects.HUD.SetActive(false);
      CutscenePlayerObjects.cutscenePlayerObjects.gameObject.SetActive(true); //enable the cutscene overlay
      CutscenePlayerObjects.cutscenePlayerObjects.dialogueText.transform.parent.gameObject.SetActive(false); //make sure the dialogue text (and the textbox) is off
    }
  }

  private void OnDisable()
  {
    if (currentCutsceneManager == this)
    {
      GlobalTools.Unpause();
      CutscenePlaying = false;
      currentCutsceneManager = null;
      if (CutscenePlayerObjects.cutscenePlayerObjects) //this can be null if the disable is because of the scene being unloaded
      {
        CutscenePlayerObjects.cutscenePlayerObjects.gameObject.SetActive(false);
      }

      if (!LoadingLevel) CutscenePlayerObjects.cutscenePlayerObjects.HUD.SetActive(true);
    }
  }

  public void PlayCutscene()
  {
    Debug.Log("Playing Cutscene: " + CutsceneFile.name, gameObject);
    //gameObject.SetActive(true);
    this.enabled = true;
  }

  void Disable()
  {
    if (ActivateWithObject)
    {
      gameObject.SetActive(false);
    }
    else
    {
      this.enabled = false;
    }
    //resuming gameplay and whatnot happens in OnDisable()
  }

  public void ResumeFromPause()
  {
    Paused = false;
    Time.timeScale = 1;
    CutscenePlayerObjects.cutscenePlayerObjects.cutscenePause.SetActive(false);
    WasPaused = true;
  }

  public void SkipCutscene()
  {
    SkipToEnd = true;
    if (verbose) Debug.Log($"initiating cutscene skip, line {CurrentLine}");
    CurrentLine--; //compensate for the initial currentline++ when the parsing loop starts next frame
    DialogueCleared = true;
    CutscenePlayerObjects.cutscenePlayerObjects.dialogueText.transform.parent.gameObject.SetActive(false);
    ResumeFromPause();
  }
}
