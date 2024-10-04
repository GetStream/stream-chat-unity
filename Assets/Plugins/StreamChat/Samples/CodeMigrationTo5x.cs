using StreamChat.Core.Models;

namespace StreamChat.Samples
{
    internal class CodeMigrationTo5x
    {
        internal void VariableDeclarationAndAssignment()
        {
            // Old (enum)
            //StreamMessageType messageType = StreamMessageType.Regular;

            // New (struct)
            StreamMessageType messageType = StreamMessageType.Regular;
        }
        
        internal void MethodParametersAndAssignment()
        {
            // Old (enum)
            // public void ProcessMessage(StreamMessageType type)
            // {
            //     
            // }

            // New (struct)
            // public void ProcessMessage(StreamMessageType type)
            // {
            //
            // }
        }
        
        // New (struct)
        public void ProcessMessage(StreamMessageType type)
        {
            
        }

        internal void Equality()
        {
            StreamMessageType messageType = StreamMessageType.Regular;
            
            // Old (enum)
            if (messageType == StreamMessageType.Regular)
            {
                // Handle regular message
            }

            // New (struct)
            if (messageType == StreamMessageType.Regular)
            {
                // Handle system message
            }
        }

        internal void SwitchStatements()
        {
            // Old (enum)
            // switch (messageType)
            // {
            //     case StreamMessageType.Regular:
            //         // Handle regular message
            //         break;
            //     case StreamMessageType.System:
            //         // Handle system message
            //         break;
            //     // ...
            // }

            StreamMessageType messageType = StreamMessageType.Regular;
            
            // New (struct) - Using if-else statements
            if (messageType == StreamMessageType.Regular)
            {
                // Handle regular message
            }
            else if (messageType == StreamMessageType.System)
            {
                // Handle system message
            }
            
            switch (messageType)
            {
                case var type when type == StreamMessageType.Regular:
                    // Handle regular message
                    break;
                case var type when type == StreamMessageType.System:
                    // Handle system message
                    break;
                // ...
            }
        }

        internal void ExplicitInitialization()
        {
            StreamMessageType messageType = StreamMessageType.Regular;
        }
    
        // Const-assignment alternative
        public static readonly StreamMessageType DefaultMessageType = StreamMessageType.Regular;
    }
}