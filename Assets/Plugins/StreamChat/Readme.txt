About:
Stream Chat Unity SDK allows for easy integration with Stream API Chat services - https://getstream.io/chat/

GitHub repository:
https://github.com/GetStream/stream-chat-unity

First steps tutorial:
https://getstream.io/chat/unity/tutorial/

Documentation:
https://getstream.io/chat/docs/unity/?language=unity

Plugin directory structure:
- Core - the main logic of the Stream Chat SDK
- *Editor Tools - Editor helpers tools (e.g. top menu option to enable debug mode)
- Libs - libraries that the Stream Chat SDK is using, Core module depends on interfaces, so you can provide you're own implementation for Websocket, logging, http client, etc. if you'd need to
- *SampleProject - example project
- *Samples - code sample for example scenarios
- *Tests - unit & integration tests

* Directories prefixed with * are optional and can be safely removed