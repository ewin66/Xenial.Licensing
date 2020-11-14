//using DevExpress.ExpressApp.Editors;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Xenial.Framework.ModelBuilders;

//namespace Xenial.Platform.Licensing.Module.BusinessObjects.Dialogs
//{
//    public class EnterPassPhraseTextDialogModelBuilder : ModelBuilder<EnterPassPhraseTextDialog>
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="EnterPassPhraseTextDialogModelBuilder"/> class.
//        /// </summary>
//        /// <param name="typeInfo">The type information.</param>
//        public EnterPassPhraseTextDialogModelBuilder(DevExpress.ExpressApp.DC.ITypeInfo typeInfo) : base(typeInfo) { }

//        public override void Build()
//        {
//            base.Build();

//            this
//                .HasCaption("Please enter a pass phrase");

//            For(m => m.Text)
//                .UsingLabelPropertyEditor();

//            For(m => m.Name)
//                .HasCaption("License key name");

//            For(m => m.PassPhrase1)
//                .HasCaption("Enter pass phrase")
//                .ImmediatePostsData()
//                .IsPassword();

//            For(m => m.PassPhrase2)
//                .HasCaption("Repeat pass phrase")
//                .ImmediatePostsData()
//                .IsPassword();
//        }
//    }
//}
