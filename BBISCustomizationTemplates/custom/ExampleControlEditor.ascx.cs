// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleControlEditor.ascx.cs" company="US Naval Academy Alumni Association & Foundation">
//   2016
// </copyright>
// <summary>
//   Defines the ExampleControlEditor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BBISCustomizationTemplates.custom
{
    using System;

    /// <summary>
    /// The example control editor.
    /// </summary>
    public partial class ExampleControlEditor : BBNCExtensions.Parts.CustomPartEditorBase
    {
        public override void OnLoadContent()
        {
            if (!this.Page.IsPostBack)
            {
                if (this.Content.GetContent(typeof(BlackbaudArguments)) != null)
                {
                    var blackbaudArgs = this.Content.GetContent(typeof(BlackbaudArguments)) as BlackbaudArguments;
                    if (blackbaudArgs != null) this.tbExampleProperty.Text = blackbaudArgs.ExampleProperty;
                }
            }
        }

        public override bool OnSaveContent(bool dialogIsClosing = true)
        {
            var blackbaudArgs = new BlackbaudArguments();
            blackbaudArgs.ExampleProperty = this.tbExampleProperty.Text;
            this.Content.SaveContent(blackbaudArgs);
            return true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}