// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleControlDisplay.ascx.cs" company="US Naval Academy Alumni Association & Foundation">
//   2016
// </copyright>
// <summary>
//   Defines the ExampleControlDisplay type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BBISCustomizationTemplates.custom
{
    using System;

    /// <summary>
    /// The example control display.
    /// </summary>
    public partial class ExampleControlDisplay : BBNCExtensions.Parts.CustomPartDisplayBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var properties = (BlackbaudArguments)this.Content.GetContent(typeof(BlackbaudArguments));

            if (properties != null)
            {
                this.lblExampleProperty.Text = properties.ExampleProperty;
            }
        }
    }
}