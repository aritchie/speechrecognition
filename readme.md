# ACR Speech Recognition Plugin for Xamarin & Windows

_Easy to use cross platform speech recognition (speech to text) plugin for Xamarin & UWP_

_September 8, 2016 - you will need the current Xamarin Beta with XCode 8 GM Seed to work with this on iOS_


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

### Easy Use

	SpeechRecognizer.Instance.Listen().Subscribe(x => 
	{
		// you will get each individual word the user speaks here
	});

### Command Based

	SpeechRecognizer
		.Instance
		.Listen()
		.Take(1)
		.Where(x => x.Equals("yes") || x.Equals("no"))
		.Subscribe(answer => 
		{
			// do something
		});

### Stop the thing!

	var token = SpeechRecognizer.Instance.Listen().Subscribe(...);
	token.Dispose(); // call this whenever you're done and it will clean up after itself!

## FAQ

Q. Why use reactive extensions and not async?
A. Speech is very event stream oriented which fits well with RX

Q. Should I use SpeechRecognizer.Instance?
A. Hell NO!  DI that sucker using the Instance