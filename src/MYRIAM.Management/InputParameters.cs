using Cartography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Linq;

namespace MYRIAM
{
    /// <summary>
    /// Stores all the necessary input parameters required to run the MYRIAM methods.
    /// </summary>
    public class InputParameters
    {
        /// <summary>
        /// Path to an existing directory in which all the output folders and files are stored.
        /// </summary>
        public string DIR_OUTPUTS { get; set; }

        /// <summary>
        /// String that is added to all output directories and input report file to help the 
        /// user setting apart iterations of the software without risking overwrite.
        /// </summary>
        public string OUTPUT_LABEL { get; set; }

        /// <summary>
        /// String that serves as plate label, and is added to all output directories and 
        /// file names. It may also serve as plate identifier, which allow MYRIAM to use 
        /// build--in plate contours from Matthews et al., 2016.
        /// </summary>
        public string PLT_LABEL { get; set; }

        /// <summary>
        /// Array of two integers, which serve as indexes to identify the two particular 
        /// Euler-vector stages used. File labeling prevents overwriting, e.g., when 
        /// using the same plate but different Euler vectors.
        /// </summary>
        public int[] STG_IDXs { get; set; }

        /// <summary>
        /// First integer element of <see cref="STG_IDXs"/>.
        /// </summary>
        public int STG_IDX_1 { get; set; }

        /// <summary>
        /// Second integer element of <see cref="STG_IDXs"/>.
        /// </summary>
        public int STG_IDX_2 { get; set; }

        /// <summary>
        /// String path to a 3-column plain-text file containing an ensemble of sampled 
        /// Cartesian values of the younger Euler vector, expressed in deg/Myr (degrees 
        /// per million year). Alternatively, MYRIAM can sample its own ensemble from a 
        /// singular Euler vector stage, by supplying a 10-column single line containing: 
        /// [1-2] Euler pole longitude and latitude in degrees, [3] angular velocity 
        /// magnitude in deg/Myr, [4-9] elements of the covariance matrix associated 
        /// with the Euler vector ensemble, in rad²/Myr², and [10] size of the ensemble.
        /// </summary>
        public string EVy_PATH { get; set; }

        /// <summary>
        /// Same as <see cref="EVy_PATH"/>, but for the older Euler vector. This input is 
        /// optional, i.e., if omitted MYRIAM will assume that <see cref="EVy_PATH"/> 
        /// already contains the difference between two Euler vectors at two particular stages.
        /// </summary>
        public string EVo_PATH { get; set; }

        /// <summary>
        /// String path to a plain-text file containing the plate contour coordinates as two 
        /// columns: [1] longitude and [2] latitude values, both expressed in degrees.
        /// </summary>
        public string CTR_PATH { get; set; }

        /// <summary>
        /// Array of coordinates for the plate's contour, in degrees.
        /// </summary>
        public List<Coordinate[]> CTR_COORDS { get; set; }

        /// <summary>
        /// Value for the grid resolution of the plate's base, expressed in degrees. That is, 
        /// the longitudinal/latitudinal spacing of the grid.
        /// </summary>
        public double GRID_RES { get; set; }

        /// <summary>
        /// Value of the lithosphere thickness (i.e., depth of the lithosphere-asthenosphere 
        /// boundary), expressed in kilometers.
        /// </summary>
        public double HL_km { get; set; }

        /// <summary>
        /// Average value of the asthenosphere viscosity, expressed in Pa · s.
        /// </summary>
        public double muA { get; set; }

        /// <summary>
        /// Average viscosity value of the lower part of the upper mantle, 
        /// expressed in Pa · s.
        /// </summary>
        public double muM { get; set; }

        /// <summary>
        /// Rectangular geographic region for calculating lateral variations of the 
        /// asthenosphere. If omitted, MYRIAM used the whole globe as region. If empty,
        /// a uniform asthenosphere viscosity is used as opposed to a laterally-varying one.
        /// </summary>
        public CartoLimits REGION_muA_LV { get; set; }

        /// <summary>
        /// Value between 0 and 1 that sets the vertical fraction of the region utilized 
        /// to define the depth-average viscosity of the asthenosphere using the model 
        /// PM_v2_2012 by Priestley and McKenzie, 2013.
        /// </summary>
        public double FRACTION_HA { get; set; }

        /// <summary>
        /// Value used to set a deformation buffer width (in kilometers) across the plate 
        /// boundary. The boundary region has a linearly-decreased Euler vector magnitude,
        /// acting as a damped rigidity, as yielding an overall smaller torque-variation 
        /// estimate.
        /// </summary>
        public double DEF_DISTANCE_km { get; set; }

        /// <summary>
        /// Number of bins used to obtain the histogram showing the distribution of the 
        /// torque-variation magnitude.
        /// </summary>
        public int? DM_MAGHIST_BINS { get; set; }

        /// <summary>
        /// Array of values used to constrain the spatial statistical distribution of the
        /// torque-variation poles, and thus modify the resolution of the output pole 
        /// confidence-contours: [1] resolution of the 2-D histogram grid in degrees, 
        /// [2-3] minimum and maximum longitude of the grid, and [4-5] minimum and maximum 
        /// latitude of the grid. 
        /// </summary>
        public double[] DM_CNTR_BINS { get; set; }

        /// <summary>
        /// Array of confidence levels (expressed as percentages) used by MYRIAM to calculate
        /// confidence contours of the torque-variation pole.
        /// </summary>
        public double[] DM_CNTR_PERCENT { get; set; }

        /// <summary>
        /// Array of three rotation angles (in degrees) used to improve the generation of the 
        /// torque-variation pole confidence contours, since these can be inaccurate when the 
        /// bulk of the torque-variation ensemble is close to any of the polar regions. The 
        /// angles instruct MYRIAM to perform the three elemental rotations that seek to center 
        /// and horizontally flatten the torque-variation ensemble on 0°N--0°E, where the 
        /// contouring is more accurate.
        /// </summary>
        public double[] ANG_ROT { get; set; }

        /// <summary>
        /// Boolean that instructs MYRIAM whether to save or not the output torque-variation 
        /// ensemble in a plain-text file.
        /// </summary>
        public bool SAVE_ENS { get; set; }
        

        /// <summary>
        /// String path to a Python3 executable in the running machine, used by MYRIAM to generate
        /// PNG figures.
        /// </summary>
        public string PYTHON_PATH { get; set; }

        /// <summary>
        /// Boolean that instructs MYRIAM whether to overwrite files. If false, MYRIAM will ask 
        /// whether overwriting is desired, when encountering folders with the same name as the
        /// current intended output.
        /// </summary>
        public bool OVERWRT_OUTPUT { get; set; }

        public string DIR_MTX_w2M { get; set; }
        public string DIR_dM_PPD { get; set; }
        public string DIR_TMP { get; set; }
        public string MODEL_LABEL { get; set; }
        public string RUN_LABEL { get; set; }
        public string MATRIX_PATH { get; set; }

        public InputParameters()
        {
        }


        /// <summary>
        /// Add a value to a property, using the property name as string.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">Value to be set.</param>
        public void Add(string propertyName, object value)
        {
            PropertyInfo nameProperty = typeof(InputParameters).GetProperty(propertyName);
            nameProperty.SetValue(this, value);
        }

        /// <summary>
        /// Retrieve the value of a property, using the property name as string
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Value of the property.</returns>
        public object GetValue(string propertyName)
        {
            PropertyInfo nameProperty = typeof(InputParameters).GetProperty(propertyName);
            return nameProperty.GetValue(this);
        }


        /// <summary>
        /// Checks whether a given <see cref="InputParameters"/> instance, contains
        /// a non-null value for a given property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>False is the property value is null, True if otherwise.</returns>
        public bool ContainsKey(string propertyName)
        {
            PropertyInfo nameProperty = typeof(InputParameters).GetProperty(propertyName);
            var value = nameProperty.GetValue(this);
            return value != null;
        }
    }
}
