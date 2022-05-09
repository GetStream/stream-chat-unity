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


- **Unity Engine 2020.x:** Supports the latest version of Unity Engine.
- **Realtime events** using websockets.
- **Supports querying channels and members** with optional filtering and sorting.
- **Fully open-source implementation:** You have access to the complete source code of the SDK here on GitHub.

## Main features

- **Messaging:** CRUD operations, flagging
- **Channels:** CRUD operations, watching, truncating, muting, querying channels, querying members
- **Users:**

## Free for Indie Developers

Stream is free for most side and hobby projects. You can use Stream Chat for free if you have less than five team members and no more than $10,000 in monthly revenue. Visit our website and apply for the Makers Account.

## Getting Started

1. Download the [latest release](https://github.com/GetStream/stream-chat-unity/releases/latest) and copy it into your project.
2. Launch Unity Engine and ensure the **Stream Chat** Plugin is imported into your project.
3. Resolve [dependencies](https://github.com/GetStream/stream-chat-unity#dependencies)
4. Start integrating Chat into your project! Check out the [Tutorial](https://getstream.io/chat/unity/tutorial/) to get yourself going.

## Sample Project

This repo contains a fully working chat example featuring:
- Browsing channels and messages
- Sending, Editing, Deleting a message
- Message right-click context menu

Just open `StreamChat/SampleProject/Scenes/ChatDemo.scene` and hit play.

It is created with Unity's uGUI UI system and supports both legacy and the new Unity's Input System. 

For Unity's new Input System please:
1. Make sure that the UnityEngine.InputSystem package is available in the project
2. Add **UnityEngine.InputSystem** dll reference to the **StreamChat.Unity** assembly definition asset

## Dependencies

- **TextMeshPro** StreamChat/SampleProject requires a TextMeshPro package
- **Newtonsoft.Json** SDK uses Newtonsoft Json Library for serialization.

:warning: In case you already have the Newtonsoft Json dll or package in your project and encounter the following error:<br>
`Multiple precompiled assemblies with the same name Newtonsoft.Json.dll included or the current platform. Only one assembly with the same name is allowed per platform.`
<br>you can remove the `StreamChat\Libs\Serialization\Newtonsoft.Json.dll`

## Missing any features?
Go ahead and open GitHub Issue with your request and we'll respond as soon as possible.

---

## We are hiring

We've recently closed a [\$38 million Series B funding round](https://techcrunch.com/2021/03/04/stream-raises-38m-as-its-chat-and-activity-feed-apis-power-communications-for-1b-users/) and we keep actively growing.
Our APIs are used by more than a billion end-users, and you'll have a chance to make a huge impact on the product within a team of the strongest engineers all over the world.
Check out our current openings and apply via [Stream's website](https://getstream.io/team/#jobs).
