﻿using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Soccer.Common.Helpers
{
    public static class Settings
    {
        private const string _tournament = "tournament";
        private static readonly string _stringdDefault = string.Empty;

        private static ISettings AppSettings => CrossSettings.Current;
        public static string Tournament
        {
            get => AppSettings.GetValueOrDefault(_tournament, _stringdDefault);
            set => AppSettings.AddOrUpdateValue(_tournament, value);
        }
    }
}
