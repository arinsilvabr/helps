﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Help_Generator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Help_Generator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon Help_Generator {
            get {
                object obj = ResourceManager.GetObject("Help_Generator", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Title=&lt;helps title&gt;
        ///DefaultTopic=&lt;filename of the default topic&gt;
        ///DefinitionsFile=&lt;optional path with definitions that can be retrieved using the definition tag&gt;
        ///ResourceFile=&lt;optional path pointing to the header files used to define context sensitive helps&gt;
        ///ResourceFile=&lt;multiple ResourceFile entries are allowed&gt;
        ///# Comment.
        /// </summary>
        internal static string SettingsHelp {
            get {
                return ResourceManager.GetString("SettingsHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!doctype html&gt;
        ///&lt;html&gt;
        ///&lt;head&gt;&lt;meta http-equiv=&apos;Content-Type&apos; content=&apos;text/html; charset=UTF-8&apos; /&gt;&lt;title&gt;CSPro Help Generator Syntax&lt;/title&gt;&lt;/head&gt;
        ///&lt;body&gt;
        ///&lt;div style=&apos;word-wrap:break-word;margin:0px;padding:0px;border:0px;background-color:#ffffff;color:#000000;font-family:Courier New;font-size:10pt;&apos;&gt;
        ///
        ///&lt;p&gt;&lt;strong&gt;Page Title:&lt;/strong&gt; &amp;lt;title&amp;gt;page title&amp;lt;/title&amp;gt;&lt;/p&gt;
        ///
        ///&lt;p&gt;&lt;strong&gt;Context Sensitive Help:&lt;/strong&gt; &amp;lt;context HIDD_FIELD_COLORS_DLG /&amp;gt; &amp;lt;context HIDD_FIELD_COLORS_DLG,500121  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SyntaxHelp {
            get {
                return ResourceManager.GetString("SyntaxHelp", resourceCulture);
            }
        }
    }
}