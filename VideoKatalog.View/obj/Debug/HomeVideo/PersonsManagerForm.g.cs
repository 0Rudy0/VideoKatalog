﻿#pragma checksum "..\..\..\HomeVideo\PersonsManagerForm.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "AF2993FAF57AABBAAB8DD4745D69AFCEA562455AB9345E4FC468D37B0F20C710"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Video_katalog {
    
    
    /// <summary>
    /// PersonsManagerForm
    /// </summary>
    public partial class PersonsManagerForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox personsListBox;
        
        #line default
        #line hidden
        
        
        #line 7 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button addNewPersonBTN;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button removePersonBtn;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button editPersonBTN;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button acceptedButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VideoKatalog.View;component/homevideo/personsmanagerform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 4 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
            ((Video_katalog.PersonsManagerForm)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.personsListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 3:
            this.addNewPersonBTN = ((System.Windows.Controls.Button)(target));
            
            #line 7 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
            this.addNewPersonBTN.Click += new System.Windows.RoutedEventHandler(this.addNewPersonBTN_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.removePersonBtn = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
            this.removePersonBtn.Click += new System.Windows.RoutedEventHandler(this.removePersonBtn_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.editPersonBTN = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
            this.editPersonBTN.Click += new System.Windows.RoutedEventHandler(this.editPersonBTN_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.acceptedButton = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\..\HomeVideo\PersonsManagerForm.xaml"
            this.acceptedButton.Click += new System.Windows.RoutedEventHandler(this.acceptedButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

