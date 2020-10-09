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

        [Title(nameof(TestFramework))]
        [Icon(typeof(Resource), nameof(Resource.test_tube_4))]
        TestFramework
    }
}
