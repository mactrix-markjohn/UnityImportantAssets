using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class ContactUsScript : MonoBehaviour
{

    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_InputField phoneInput;
    public TMP_InputField messageInput;
    
    //[SerializeField] private TMPro.TextMeshProUGUI txtData;
    //[SerializeField] private UnityEngine.UI.Button btnSubmit;
    //[SerializeField] private CollectionOption option;
 
    private enum CollectionOption { openEmailClient, openGFormLink, sendGFormData };
 
    private const string kReceiverEmailAddress = "me@gmail.com";
 
    //private const string kGFormBaseURL = "https://docs.google.com/forms/d/e/1FAIpQLSdtvxa-yqO445ZFGER2gK8FNGPvxYAzRRTs0Gpgz4pQD0LRRA/";

    private const string kGFormBaseURL = "https://docs.google.com/forms/d/e/1FAIpQLSfkQ2o-i3SXwlAJlDrtzn8QA4ZXFitj75O3BCj2oxRVRwQgbQ/";

    private string k = "https://docs.google.com/forms/d/e/1FAIpQLSdtvxa-yqO445ZFGER2gK8FNGPvxYAzRRTs0Gpgz4pQD0LRRA/viewform?usp=sf_link";
    private const string kGFormEntryID = "entry.1330939643";

    
    /*// Love Preferred Google Form Entry IDs
    private const string kGFormNameEntryID = "entry.1088701138";
    private const string kGFormEmailEntryID = "entry.114595502";
    private const string kGFormPhoneNumberEntryID = "entry.496519102";
    private const string kGFormMessageEntryID = "entry.1546419194";*/
    // Love Preferred Google Form Entry IDs
    private const string kGFormNameEntryID = "entry.1120455879";
    private const string kGFormEmailEntryID = "entry.480602812";
    private const string kGFormPhoneNumberEntryID = "entry.549549312";
    private const string kGFormMessageEntryID = "entry.536503275";
    
 
    void Start() {
        /*UnityEngine.Assertions.Assert.IsNotNull( txtData );
        UnityEngine.Assertions.Assert.IsNotNull( btnSubmit );
        btnSubmit.onClick.AddListener( delegate {
            switch ( option ) {
                case CollectionOption.openEmailClient:
                    OpenEmailClient( txtData.text );
                    break;
                case CollectionOption.openGFormLink:
                    OpenGFormLink();
                    break;
                case CollectionOption.sendGFormData:
                    StartCoroutine( SendGFormData( txtData.text ) );
                    break;
            }
        } );*/
    }
 
    private static void OpenEmailClient( string feedback ) {
        string email = kReceiverEmailAddress;
        string subject = "Feedback";
        string body = "<" + feedback + ">";
        OpenLink( "mailto:" + email + "?subject=" + subject + "&body=" + body );
    }
 
    private static void OpenGFormLink() {
        string urlGFormView = kGFormBaseURL + "viewform";
        OpenLink( urlGFormView );
    }
 
    private static IEnumerator SendGFormData<T>( T dataContainer ) {
        bool isString = dataContainer is string;
        string jsonData = isString ? dataContainer.ToString() : JsonUtility.ToJson(dataContainer);
 
        WWWForm form = new WWWForm();
        form.AddField( kGFormEntryID, jsonData );
        string urlGFormResponse = kGFormBaseURL + "formResponse";
        using ( UnityWebRequest www = UnityWebRequest.Post( urlGFormResponse, form ) ) {
            yield return www.SendWebRequest();
        }
    }
    
    private static IEnumerator SendGoogleFormData<T>( T nameContainer,T emailContainer, T phoneContainer, T messageContainer ) {
        bool isNameString = nameContainer is string;
        bool isEmailString = emailContainer is string;
        bool isPhoneString = phoneContainer is string;
        bool isMessageString = messageContainer is string;
        
        string namejsonData = isNameString ? nameContainer.ToString() : JsonUtility.ToJson(nameContainer);
        string emailjsonData = isEmailString ? emailContainer.ToString() : JsonUtility.ToJson(emailContainer);
        string phonejsonData = isPhoneString ? phoneContainer.ToString() : JsonUtility.ToJson(phoneContainer);
        string messagejsonData = isMessageString ? messageContainer.ToString() : JsonUtility.ToJson(messageContainer);
        
 
        WWWForm form = new WWWForm();
        
        form.AddField( kGFormNameEntryID, namejsonData );
        form.AddField( kGFormEmailEntryID, emailjsonData );
        form.AddField( kGFormPhoneNumberEntryID, phonejsonData );
        form.AddField( kGFormMessageEntryID, messagejsonData );
        
        
        
        string urlGFormResponse = kGFormBaseURL + "formResponse";
        using ( UnityWebRequest www = UnityWebRequest.Post( urlGFormResponse, form ) ) {
            yield return www.SendWebRequest();
        }


    }
 
    // We cannot have spaces in links for iOS
    public static void OpenLink( string link ) {
        bool googleSearch = link.Contains( "google.com/search" );
        string linkNoSpaces = link.Replace( " ", googleSearch ? "+" : "%20");
        Application.OpenURL( linkNoSpaces );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Button click handlers

    public UnityEvent successfulGoogleForm;

    public void SendMessageToGoogleFormClick()
    {

        if (String.IsNullOrEmpty(nameInput.text))
        {
            nameInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Please enter your name";
            return;
        }

        if (String.IsNullOrEmpty(emailInput.text))
        {
            emailInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Don't forget to write your mail";
            return;
        }

        if (String.IsNullOrEmpty(messageInput.text))
        {
            messageInput.placeholder.GetComponent<TextMeshProUGUI>().text = "Try again!, Make sure to Enter a Message";
            return;
        }


        StartCoroutine( SendGoogleFormData(nameInput.text, emailInput.text, phoneInput.text, messageInput.text) );

        successfulGoogleForm.Invoke();

        /*string urlGFormView = kGFormBaseURL + "viewform";
        OpenLink( urlGFormView );*/
    }

    public void ResetInputs(){
        nameInput.text = "";
        emailInput.text = "";
        phoneInput.text = "";
        messageInput.text = "";
    }

    public void SendEmailClick()
    {
        SendEmail("farmerfund@lovepreferredcoffee.com"," "," ");
    }
    
    void SendEmail(string emailAg, string subjectAg, string bodyAg)
    {
        //Application.OpenURL("http://www.google.com");
        
        //email Id to send the mail to
        string email = emailAg;
        //subject of the mail
        string subject = MyEscapeURL(subjectAg);
        //body of the mail which consists of Device Model and its Operating System
        string body = "";
        int k = 1;
        
        body = MyEscapeURL(bodyAg);
        
        Application.OpenURL ("mailto:" + email + "?subject=" + subject + "&body=" + body);
        
        //Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }
}
