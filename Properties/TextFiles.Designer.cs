﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MYRIAM.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TextFiles {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TextFiles() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MYRIAM.Properties.TextFiles", typeof(TextFiles).Assembly);
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
        ///   Looks up a localized string similar to ! ============== Default Parameters ===============================================
        ///
        ///!-Output label for folders and input parameters report
        ///OUTPUT_LABEL = &quot;&quot;
        ///
        ///
        ///!-Resolution for plate&apos;s base [degrees]
        ///GRID_RES = 1
        ///
        ///
        ///!-Depth of Lithosphere-Asthenosphere boundary [km]
        ///HL_km = 180
        ///
        ///
        ///!-Geographic region for viscosity averaging [degrees]
        ///REGION_muA_LV = [-180, 180, -90, 90]
        ///
        ///
        ///!-Vertical fraction of asthenospheric channel to use
        ///FRACTION_HA = 1
        ///
        ///
        ///!-Viscosity average values for the asthenosphe [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DefaultParameters {
            get {
                return ResourceManager.GetString("DefaultParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ! ============== dEVdM Parameters Report ===============================================
        ///
        ///!-Output directory
        ///DIR_OUTPUTS =
        ///
        ///
        ///!-Output label
        ///OUTPUT_LABEL =
        ///
        ///
        ///!-Plate label
        ///PLT_LABEL =
        ///
        ///
        ///!-Stage indexes
        ///STG_IDXs =
        ///
        ///
        ///!-Euler vector young path
        ///EVy_PATH =
        ///
        ///
        ///!-Euler vector old path
        ///EVo_PATH =
        ///
        ///
        ///!-Plate contour coordinates [degrees]
        ///CTR_PATH =
        ///
        ///
        ///!-Resolution for plate&apos;s base [degrees]
        ///GRID_RES =
        ///
        ///
        ///!-Depth of Lithosphere-Asthenosphere boundary [km]
        ///HL_km =
        ///
        ///
        ///!-Viscosity average [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string InputFileReport_Template {
            get {
                return ResourceManager.GetString("InputFileReport_Template", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !radius(km) alpha(m/s) beta(m/s) rho(kg/m**3) Ks(GPa) mu(GPa) gamma  Pres(Gpa) g(m/s**2)
        ///6371.0  	 1450.00  	 0.00  	     1020.00  	 2.1  	     0.0  	     0.5000  	 0.000  	 9.8156
        ///6368.01  	 1450.00  	 0.00        1020.00  	 2.1  	     0.0  	     0.5000  	 0.300  	 9.8222
        ///6368.0  	 5800.00  	 3200.00  	 2600.00  	 52.0  	     26.6  	     0.2812  	 0.300  	 9.8222
        ///6356.01  	 5800.00  	 3200.00  	 2600.00  	 52.0  	     26.6  	     0.2812  	 0.337  	 9.8332
        ///6356.0  	 6800.00  	 3900.00  	 2900.00  	 75. [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Prem_MantleModel {
            get {
                return ResourceManager.GetString("Prem_MantleModel", resourceCulture);
            }
        }
    }
}
