using System;

namespace StreamChat.SampleProject.Utils
{
    public static class TextUtils
    {
        public static string TimeAgo(this DateTime dateTime)
        {
            var result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = $"{timeSpan.Seconds} seconds ago";
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ? $"{timeSpan.Minutes} minutes ago" : "a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ? $"{timeSpan.Hours} hours ago" : "an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ? $"{timeSpan.Days} days ago" : "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ? $"{timeSpan.Days / 30} months ago" : "a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ? $"{timeSpan.Days / 365} years ago" : "a year ago";
            }

            return result;
        }
    }
}