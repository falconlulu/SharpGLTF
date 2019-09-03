﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpGLTF.Schema2
{
    [System.Diagnostics.DebuggerDisplay("{Version} {MinVersion} {Generator} {Copyright}")]
    public sealed partial class Asset
    {
        #region lifecycle

        internal Asset() { }

        internal static Asset CreateDefault(string copyright)
        {
            var av = AssemblyInformationalVersion;

            var generator = string.IsNullOrWhiteSpace(av) ? "SharpGLTF" : "SharpGLTF " + av;

            return new Asset()
            {
                _generator = generator,
                _copyright = copyright,
                _version = MINVERSION.ToString(),
                _minVersion = null
            };
        }

        #endregion

        #region properties

        public static string AssemblyInformationalVersion
        {
            get
            {
                var av = typeof(Asset).Assembly.GetCustomAttributes(true)
                .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
                .FirstOrDefault();

                return av?.InformationalVersion ?? string.Empty;
            }
        }

        private static readonly Version ZEROVERSION = new Version(0, 0);
        private static readonly Version MINVERSION = new Version(2, 0);
        private static readonly Version MAXVERSION = new Version(2, 0);

        public string Copyright { get => _copyright; set => _copyright = value.AsEmptyNullable(); }
        public string Generator { get => _generator; set => _generator = value.AsEmptyNullable(); }

        public Version Version      => Version.TryParse(   _version, out Version ver) ? ver : ZEROVERSION;

        public Version MinVersion   => Version.TryParse(_minVersion, out Version ver) ? ver : MINVERSION;

        #endregion

        #region extra properties

        // public String Title { get => _GetExtraInfo("title"); set => _SetExtraInfo("title", value); }

        // public String Author { get => _GetExtraInfo("author"); set => _SetExtraInfo("author", value); }

        // public String License { get => _GetExtraInfo("license"); set => _SetExtraInfo("license", value); }

        #endregion

        #region API

        internal override void Validate(Validation.ValidationContext result)
        {
            base.Validate(result);

            if (string.IsNullOrWhiteSpace(_version)) result.AddError(this, "version number is missing");

            if (Version < MINVERSION) result.AddError(this, $"Minimum supported version is {MINVERSION} but found:{MinVersion}");
            if (MinVersion > MAXVERSION) result.AddError(this, $"Maximum supported version is {MAXVERSION} but found:{MinVersion}");

            if (MinVersion > Version) result.AddSemanticWarning(this, Validation.ErrorCodes.ASSET_MIN_VERSION_GREATER_THAN_VERSION, MinVersion, Version);
        }

        private string _GetExtraInfo(string key)
        {
            if (this.Extras is IReadOnlyDictionary<string, Object> dict)
            {
                return dict.TryGetValue(key, out Object val) ? val as String : null;
            }
            else
            {
                return null;
            }
        }

        private void _SetExtraInfo(string key, string val)
        {
            throw new NotImplementedException();
            // if (this.Extras == null) this.Extras = new Dictionary<string, Object>();
        }

        #endregion
    }
}