// -----------------------------------------------------------------------
//  <copyright file="BooleanSetting.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven35.Database.Config.Settings
{
    internal class BooleanSetting : Setting<bool>
    {
        public BooleanSetting(string value, bool defaultValue) : base(value, defaultValue)
        {
        }

        public override bool Value
        {
            get
            {
                return string.IsNullOrEmpty(value) == false ? bool.Parse(value) : defaultValue;
            }
        }
    }
}
