using System.ComponentModel;
using Xarial.XCad.Base.Attributes;
using SldWorksLookup.Properties;

namespace SldWorksLookup
{
    [Title("Lookup")]
    [Description("Object Lookup for SldWorks")]
    public enum Command_e
    {
        [Title("Snoop ISldWorks")]
        [Icon(typeof(Resource), nameof(Resource.BrowseData_16x))]
        Lookup,

        [Title("Snoop ActiveDoc")]
        [Icon(typeof(Resource),nameof(Resource.tasklistpart_40))]
        ActiveDoc,

        [Title("Snoop Current Selection")]
        [Icon(typeof(Resource), nameof(Resource.SketchRegion))]
        CurrentSelection,

        [Title("Snoop PID")]
        [Description("Snoop Persistent Reference IDs")]
        [Icon(typeof(Resource),nameof(Resource.id))]
        SnoopPID,

        [Title("GetObjectByPID")]
        [Description("Get Object By PID")]
        [Icon(typeof(Resource), nameof(Resource.Class_32x))]
        GetObjectByPID,

        [Title("CaptureCmd")]
        [Description("Capture SolidWorks Commands")]
        [Icon(typeof(Resource),nameof(Resource.Buttons))]
        CaptureCmd,

        [Title("ColorToInt")]
        [Description("Select a color to get a int value")]
        [Icon(typeof(Resource),nameof(Resource.color_palette))]
        ColorToInt,

        [Title(nameof(TestFramework))]
        [Icon(typeof(Resource), nameof(Resource.test_tube_4))]
        TestFramework
    }
}
