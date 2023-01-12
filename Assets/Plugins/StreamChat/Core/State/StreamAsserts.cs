using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamChat.Core.State
{
    internal static class StreamAsserts
    {
        /// <summary>
        /// Docs states the ID max length is 64 characters
        /// https://getstream.io/chat/docs/unity/creating_channels/?language=unity#channel-data
        /// </summary>
        /// <param name="channelId"></param>
        public static void AssertChannelIdLength(string channelId)
        {
            if (channelId.Length > 64)
            {
                throw new ArgumentException($"{nameof(channelId)} cannot be longer than 64 characters");
            }
        }

        public static void AssertChannelTypeIsValid(ChannelType channelType)
        {
            if (!channelType.IsValid)
            {
                throw new ArgumentException($"Invalid {nameof(channelType)} - internal key is empty");
            }
        }

        public static void AssertNotNull<T>(T item, string argName)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(argName));
            }
        }

        public static void AssertNotNullOrEmpty<T>(ICollection<T> items, string argName)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(argName));
            }

            if (items.Count == 0)
            {
                throw new ArgumentException($"{argName} cannot be empty");
            }
        }
        
        public static void AssertNotNullOrEmpty<T>(T[] items, string argName)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(argName));
            }

            if (items.Length == 0)
            {
                throw new ArgumentException($"{argName} cannot be empty");
            }
        }

        public static void AssertNotNullOrEmpty<T>(IEnumerable<T> items, string argName)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(argName));
            }

            if (!items.Any())
            {
                throw new ArgumentException($"{argName} cannot be empty");
            }
        }

        public static void AssertNotNullOrEmpty(string value, string argName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(argName));
            }

            if (value == string.Empty)
            {
                throw new ArgumentException($"{argName} cannot be empty");
            }
        }

        public static void AssertGreaterThanZero(int? value, string argName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(argName)} must be greater than 0");
            }
        }
        
        public static void AssertGreaterThanOrEqualZero(int? value, string argName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(argName)} must be greater than or equal to 0");
            }
        }
        
        public static void AssertWithinRange(int value, int minValue, int maxValue, string argName)
        {
            if (value < minValue)
            {
                throw new ArgumentOutOfRangeException($"{nameof(argName)} must be greater than or equal to {minValue}");
            }
            
            if (value > maxValue)
            {
                throw new ArgumentOutOfRangeException($"{nameof(argName)} must be less than or equal to {maxValue}");
            }
        }
    }
}