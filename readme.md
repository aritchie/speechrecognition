# ACR Speech Recognition Plugin for Xamarin & Windows

## PLATFORMS

|Platform|Version|
| ------------------- |:------------------: |
|Xamarin.iOS|iOS 10+|
|Xamarin.Android|API 23+|
|Windows UWP|10+|

## SETUP
    

## HOW TO USE

iOS
    <key>NSSpeechRecognitionUsageDescription</key>  
    <string>Say something useful here</string>  
    <key>NSMicrophoneUsageDescription</key>  
    <string>Say something useful here</string> 

Android
	<uses-sdk android:minSdkVersion="23" />
	<uses-permission android:name="android.permission.INTERNET" />
	<uses-permission android:name="android.permission.RECORD_AUDIO" />

UWP
<Capabilities>
 <Capability Name="internetClient" />
 <DeviceCapability Name="microphone" />
 /Capabilities>
 