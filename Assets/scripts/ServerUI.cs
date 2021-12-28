using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerUI : MonoBehaviour{
  Text StatusMsg;
  Text ElapsedTime;
  Text VersionNumber;

  float TimeStep;
  string Status = "Disconnected";
  // Start is called before the first frame update
  void Start(){
    Text[] ListofText = gameObject.GetComponentsInChildren<UnityEngine.UI.Text>();
    for (int i = 0; i < ListofText.Length; i++){
      switch (ListofText[i].name){
        case "Status":
          StatusMsg = ListofText[i];
          break;
        case "Time":
          ElapsedTime = ListofText[i];
          break;
        case "VersionTxt":
          VersionNumber = ListofText[i];
          break;
        default:
          break;
      }
    }
    
    StatusMsg.text = "Status: Disconnected";
    ElapsedTime.text = "Elapsed Time: 00:00:00";
    VersionNumber.text = "Unity Mathworks Link v0.1";
  }

  // Update is called once per frame
  void Update(){
    if (Input.GetButtonDown("Close_Program"))
      Application.Quit();
  }
  
  //Allows for other classes to set the status
  public void SetStatus(string Message){
    this.Status = Message;
    this.StatusMsg.text = "Status: " + this.Status;
  }

  //Allows for other classes to set the elapsed time
  public void SetElapsedTime(float Time){
    TimeStep = Time;
    this.ElapsedTime.text = "Elapsed Time: " + TimeStep.ToString();
  }
}