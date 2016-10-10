# ACR Speech Recognition Plugin for Xamarin & Windows

_Easy to use cross platform speech recognition (speech to text) plugin for Xamarin & UWP_

[![NuGet](https://img.shields.io/nuget/v/Acr.SpeechRecognizer.svg?maxAge=2592000)](https://www.nuget.org/packages/Acr.SpeechRecognizer/)
[![NuGet](https://img.shields.io/nuget/v/Acr.SpeechDialogs.svg?maxAge=2592000)](https://www.nuget.org/packages/Acr.SpeechDialogs/)


## PLATFORMS

* iOS 10+
* Android
* Windows UWP

## SETUP

iOS
	Add the following to your 
    <key>NSSpeechRecognitionUsageDescription</key>  
    <string>Say something useful here</string>  
    <key>NSMicrophoneUsageDescription</key>  
    <string>Say something useful here</string> 

Android

	Add the following to your AndroidManifest.xml
	<uses-permission android:name="android.permission.INTERNET" />
	<uses-permission android:name="android.permission.RECORD_AUDIO" />

UWP 

	Add the following to your app manifest
	<Capabilities>
		<Capability Name="internetClient" />
 		<DeviceCapability Name="microphone" />
 	</Capabilities>


## HOW TO USE

### Request Permission

    var granted = await SpeechRecognizer.Instance.RequestPermission();
    if (granted) 
    {
        // go!
    }

### Easy Use

	SpeechRecognizer.Instance.Listen().Subscribe(x => 
	{
		// you will get each individual word the user speaks here
	});


### Listen for a phrase (good for a web search)

    SpeechRecognizer
        .Instance
        .Listen(true) // passing true will complete this observable when the end of speech is detected
        .Subscribe(phrase => {})


### Stop the thing!

	var token = SpeechRecognizer.Instance.Listen().Subscribe(...);
	token.Dispose(); // call this whenever you're done and it will clean up after itself!

## Speech Dialogs Addin

_Speech dialogs is an additional nuget you can install via nuget to add easy question based prompts.  It will prompt the user with questions using Text-to-Speech and you can reply with a selection of answers_


### Confirm

    var answer = await SpeechDialogs.Instance.Confirm("Are you sure you want to do this?", "yes", "no");

### Prompt (great for searches)

    var prompt = await SpeechDialogs.Instance.Prompt("How was your day?");

### Actions

    SpeechDialogs.Instance.Actions(new ActionsConfig("Choose your destiny!") 
        .Choice("Fatalitiy", () => 
        { 
            // do something here
        })
        .Choice("Friendship", () => 
        {
        
        })
        .SetShowActionSheet(true) // this will decide if you also want to include the UI dialog
        .SetSpeakChoices(true)    // this will read the choices out that you make available
    )

## FAQ

Q. Why use reactive extensions and not async?
A. Speech is very event stream oriented which fits well with RX

Q. Should I use SpeechRecognizer.Instance?
A. Hell NO!  DI that sucker using the Instance