using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;

public class RecordingCanvas : MonoBehaviour
{
  public Button startRecordingButton;
  public Text resultText;

  private bool isRecording = true;

  void Start()
  {
    if (SpeechRecognizer.ExistsOnDevice())
    {
      SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
      listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
      listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
      listener.onErrorDuringRecording.AddListener(OnError);
      listener.onErrorOnStartRecording.AddListener(OnError);
      listener.onFinalResults.AddListener(OnFinalResult);
      listener.onPartialResults.AddListener(OnPartialResult);
      listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
      SpeechRecognizer.RequestAccess();
      
      
      // Newly added by Mark
      isRecording = true;
      SpeechRecognizer.StartRecording(true);
      startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
      resultText.text = "Say something :-)";
      
      
    }
    else
    {
      resultText.text = "Sorry, but this device doesn't support speech recognition";
      startRecordingButton.enabled = false;
    }


  }

  public void OnFinalResult(string result)
  {
    startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    resultText.text = result;
    startRecordingButton.enabled = true;
    
    // Newly added by Mark
    SpeechRecognizer.StartRecording(true);
    startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
  }

  public void OnPartialResult(string result)
  {
    resultText.text = result;
  }

  public void OnAvailabilityChange(bool available)
  {
    startRecordingButton.enabled = available;
    if (!available)
    {
      resultText.text = "Speech Recognition not available";
    }
    else
    {
      resultText.text = "Say something :-)";
    }
  }

  public void OnAuthorizationStatusFetched(AuthorizationStatus status)
  {
    switch (status)
    {
      case AuthorizationStatus.Authorized:
        startRecordingButton.enabled = true;
        break;
      default:
        startRecordingButton.enabled = false;
        resultText.text = "Cannot use Speech Recognition, authorization status is " + status;
        break;
    }
  }

  public void OnEndOfSpeech()
  {

    // Newly added by Mark
    if (isRecording)
    {
      SpeechRecognizer.StartRecording(true);
      startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
    }
    else
    {
      startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    }
    
    
    // Newly added by Mark
    SpeechRecognizer.StartRecording(true);
    startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";


    
    
   
    
  }

  public void OnError(string error)
  {
    Debug.LogError(error);
    startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
    startRecordingButton.enabled = true;
    
    // Newly added by Mark
    SpeechRecognizer.StartRecording(true);
    startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
  }

  public void OnStartRecordingPressed()
  {
    if (SpeechRecognizer.IsRecording())
    {

      isRecording = false;
#if UNITY_IOS && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			startRecordingButton.GetComponentInChildren<Text>().text = "Stopping";
			startRecordingButton.enabled = false;
#elif UNITY_ANDROID && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
#endif
    }
    else
    {
      isRecording = true;
      SpeechRecognizer.StartRecording(true);
      startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
      resultText.text = "Say something :-)";
    }
  }
}
