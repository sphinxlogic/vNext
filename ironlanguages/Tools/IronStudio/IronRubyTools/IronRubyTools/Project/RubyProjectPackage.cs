/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * ironruby@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.IronRubyTools.Navigation;
using Microsoft.IronStudio.Project;
using Microsoft.VisualStudio.Project;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.IronRubyTools.Project {
    [PackageRegistration(UseManagedResourcesOnly = true)]
    //Set the projectsTemplatesDirectory to a non-existant path to prevent VS from including the working directory as a valid template path
    [ProvideProjectFactory(typeof(RubyProjectFactory), RubyConstants.LanguageName,
        "IronRuby Project Files (*.rbproj);*.rbproj", "rbproj", "rbproj", "non-existant path", LanguageVsTemplate = RubyConstants.LanguageName)]
    //[SingleFileGeneratorSupportRegistration(typeof(RubyProjectFactory))]
    [ProvideObject(typeof(RubyGeneralPropertyPage))]
    [ProvideEditorExtension(typeof(RubyEditorFactory), ".rb", 32)]
    [ProvideEditorExtension(typeof(RubyEditorFactory), ".ru", 32)]
    [ProvideEditorExtension(typeof(RubyEditorFactory), ".gemspec", 32)]
    [ProvideEditorLogicalView(typeof(RubyEditorFactory), "{7651a702-06e5-11d1-8ebd-00a0c90f26ea}")]  //LOGVIEWID_Designer
    [ProvideEditorLogicalView(typeof(RubyEditorFactory), "{7651a701-06e5-11d1-8ebd-00a0c90f26ea}")]  //LOGVIEWID_Code
    /*[ProvideLoadKey(CommonConstants.PLKMinEdition, RubyProjectConstants.PLKProductVersion,
        RubyProjectConstants.PLKProductName, CommonConstants.PLKCompanyName, CommonConstants.PLKResourceID)]*/
    [Guid(RubyConstants.ProjectSystemPackageGuid)]
    //[WebSiteProject(RubyProjectConstants.IronRubyLanguageName, RubyProjectConstants.IronRubyLanguageName)]
    [ProvideService(typeof(IRubyStarter))]
    //[ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]   // load w/ and w/o a solution so we can initialize our filetypes
    //[ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]
    public class RubyProjectPackage : CommonProjectPackage {
        public override ProjectFactory CreateProjectFactory() {
            return new RubyProjectFactory(this);
        }

        public override CommonEditorFactory CreateEditorFactory() {
            return new RubyEditorFactory(this);
        }

        /// <summary>
        /// This method is called to get the icon that will be displayed in the
        /// Help About dialog when this package is selected.
        /// </summary>
        /// <returns>The resource id corresponding to the icon to display on the Help About dialog</returns>
        public override uint GetIconIdForAboutBox() {
            return RubyConstants.IconIdForAboutBox;
        }
        /// <summary>
        /// This method is called during Devenv /Setup to get the bitmap to
        /// display on the splash screen for this package.
        /// </summary>
        /// <returns>The resource id corresponding to the bitmap to display on the splash screen</returns>
        public override uint GetIconIdForSplashScreen() {
            return RubyConstants.IconIfForSplashScreen;
        }
        /// <summary>
        /// This methods provides the product official name, it will be
        /// displayed in the help about dialog.
        /// </summary>
        public override string GetProductName() {
            return RubyConstants.LanguageName;
            //return Resources.ProductName;
        }
        /// <summary>
        /// This methods provides the product description, it will be
        /// displayed in the help about dialog.
        /// </summary>
        public override string GetProductDescription() {
            return "IronRuby";
            //return Resources.ProductDescription;
        }
        /// <summary>
        /// This methods provides the product version, it will be
        /// displayed in the help about dialog.
        /// </summary>
        public override string GetProductVersion() {
            return this.GetType().Assembly.GetName().Version.ToString();
        }
       
        /// <summary>
        /// Creates an instance of a language specific service that 
        /// allows to start a project or a file with or without debugging.
        /// </summary>
        protected override IStarter/*!*/ CreateStarter() {
            return new RubyStarter((IServiceProvider)this);
        }
    }
}
