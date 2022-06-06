<p align="center">
  <img src="ReadmeAssets/256px.png"/>
</p>

<p align="center">
    The official Unity SDK for <a href="https://getstream.io/chat/sdk/unity/">Stream Chat</a>.
</p>

<p align="center">
    <a href="https://getstream.io/chat/sdk/unity/">Website</a>
    |
    <a href="https://getstream.io/chat/unity/tutorial/">Tutorial</a>
    |
    <a href="https://getstream.io/chat/docs/unity/?language=unity">SDK Documentation</a>
    |
    <a href="https://getstream.io/chat/trial/">Register for API Key</a>
</p>

---

The **Stream Chat SDK** is the official Unity SDK for [Stream Chat](https://getstream.io/chat/sdk/unity/), a service for building chat and messaging games and applications.

- **Unity Engine 2021.x & 2020.x Support**
- **IL2CPP** Support
- **Realtime** Highly responsive events using Websockets
- **Messaging:** Send direct or group messages, and have the messages persist across sessions
- **Channels:** group channels, private messages, etc.
- **Reactions:** Every message can be reacted to by multiple users
- **Moderation:** Banning (temporary, permanent, shadow), Muting (users, channels), Flagging (messages, users)
- **Roles & Permissions:** Admins, Moderators, custom roles with custom permission
- **Moderation Dashboard** Dedicated web panel to handle moderation
- **Fully open-source**: Complete access to the SDK source code here on GitHub

## Free for Indie Developers

Stream is free for most side and hobby projects. You can use Stream Chat for free if you have less than five team members and no more than $10,000 in monthly revenue. Visit our website and [apply for the Makers Account](https://getstream.io/maker-account/).

## Getting Started

1. Download the [latest release](https://github.com/GetStream/stream-chat-unity/releases/latest) and copy it into your project.
2. Launch Unity Engine and ensure the **Stream Chat** Plugin is imported into your project.
3. Resolve [dependencies](https://github.com/GetStream/stream-chat-unity#dependencies)
4. Start integrating Chat into your project! Check out the [Tutorial](https://getstream.io/chat/unity/tutorial/) to get yourself going.

## Sample Project

In the `StreamChat/SampleProject` you'll find a fully working chat example featuring:
- Browsing channels and messages
- Sending, Editing, Deleting a message
- Message right-click context menu
- Reactions
- Sending video attachments (works only in Editor)

It is created with Unity's uGUI UI system and supports both legacy and the new Unity's Input System. 

**How to run it?**
1. [Register](https://getstream.io/try-for-free/) an account and go to [Stream Dasboard](https://getstream.io/dashboard/)
2. Create App and go to its **Chat Explorer** throught the Dashboard
3. Create new chat user and save its **id**
4. Use our [online token generator](https://getstream.io/chat/docs/unity/tokens_and_authentication/?language=unity#manually-generating-tokens) to create user token
5. In Unity, provide: **Api Key**, **User Id** and **User Token** into `StreamChat/SampleProject/Config/DemoCredentials.asset`
6. Open `StreamChat/SampleProject/Scenes/ChatDemo.scene` and hit play

How to enable Unity's **[new Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html)**?:
1. Make sure that the UnityEngine.InputSystem package is available in the project
2. Add **UnityEngine.InputSystem** dll reference to the **StreamChat.Unity** assembly definition asset

## IL2CPP
SDK runs out of the box with IL2CPP

## Dependencies

- **TextMeshPro** StreamChat/SampleProject requires a TextMeshPro package
- **Newtonsoft.Json** SDK uses [Unity's package for Newtonsoft Json]([com.unity.nuget.newtonsoft-json@3.0](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html)) Library for serialization.

:warning: In case you already have the Newtonsoft Json dll or package in your project and encounter the following error:<br>
`Multiple precompiled assemblies with the same name Newtonsoft.Json.dll included or the current platform. Only one assembly with the same name is allowed per platform.`
<br>you can remove the `StreamChat\Libs\Serialization\com.unity.nuget.newtonsoft-json@3.0.2` directory. Please note however, that Unity's package for Newtonsoft Json has IL2CPP support. If you wish to replace it and still use IL2CPP, make sure that the Json implementaion of your choice does support IL2CPP as well.

## Missing any features?
Go ahead and open GitHub Issue with your request and we'll respond as soon as possible.

---

## We are hiring

We've recently closed a [\$38 million Series B funding round](https://techcrunch.com/2021/03/04/stream-raises-38m-as-its-chat-and-activity-feed-apis-power-communications-for-1b-users/) and we keep actively growing.
Our APIs are used by more than a billion end-users, and you'll have a chance to make a huge impact on the product within a team of the strongest engineers all over the world.
Check out our current openings and apply via [Stream's website](https://getstream.io/team/#jobs).
