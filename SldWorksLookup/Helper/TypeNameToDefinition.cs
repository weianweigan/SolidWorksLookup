using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace SldWorksLookup
{
    public static class TypeNameToDefinition
    {
        /// <summary>
        /// 获取特征对应的接口
        /// </summary>
        /// <param name="name">使用<see cref="IFeature.GetTypeName2()"/> 获取的值</param>
        /// <returns>对应的类型，可能会返回两种类型</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static List<Type> Match(string name)
        {
            List<Type> featDataTypes = new List<Type>();
            switch (name)
            {
                //-----------Assembly-------------
                case "AsmExploder":ThrowMatchNoneException(name, "Assembly exploded view in ConfigurationManager");break;
                case "CompExplodeStep": ThrowMatchNoneException(name, "Explode step for assembly exploded view"); break;
                case "ExplodeLineProfileFeature": featDataTypes.Add(typeof(ISketch)); break;
                case "InContextFeatHolder": featDataTypes.Add(typeof(IFeature)); break;
                case "MagneticGroundPlane": featDataTypes.Add(typeof(IFeature)); break;
                case "MateCamTangent": featDataTypes.Add(typeof(ICamFollowerMateFeatureData)); break;
                case "MateCoincident": featDataTypes.Add(typeof(ICoincidentMateFeatureData)); break;
                case "MateConcentric": featDataTypes.Add(typeof(IConcentricMateFeatureData)); break;
                case "MateDistanceDim": featDataTypes.Add(typeof(IDistanceMateFeatureData)); break;
                case "MateGearDim": featDataTypes.Add(typeof(IGearMateFeatureData)); break;
                case "MateHinge": featDataTypes.Add(typeof(IHingeMateFeatureData)); break;
                case "MateInPlace":featDataTypes.Add(typeof(IMate2)); break; 
                case "MateLinearCoupler": featDataTypes.Add(typeof(ILinearCouplerMateFeatureData)); break;
                case "MateLock": featDataTypes.Add(typeof(ILockMateFeatureData)); break;
                case "MateParallel": featDataTypes.Add(typeof(IParallelMateFeatureData)); break;
                case "MatePerpendicular": featDataTypes.Add(typeof(IPerpendicularMateFeatureData)); break;
                case "MatePlanarAngleDim": featDataTypes.Add(typeof(IAngleMateFeatureData)); break;
                case "MateProfileCenter": featDataTypes.Add(typeof(IProfileCenterMateFeatureData)); break;
                case "MateRackPinionDim": featDataTypes.Add(typeof(IRackPinionMateFeatureData)); break;
                case "MateScrew": featDataTypes.Add(typeof(IScrewMateFeatureData)); break;
                case "MateSlot": featDataTypes.Add(typeof(ISlotMateFeatureData)); break;
                case "MateSymmetric": featDataTypes.Add(typeof(ISymmetricMateFeatureData)); break;
                case "MateTangent": featDataTypes.Add(typeof(ITangentMateFeatureData)); break;
                case "MateUniversalJoint": featDataTypes.Add(typeof(IUniversalJointMateFeatureData)); break;
                case "MateWidth": featDataTypes.Add(typeof(IWidthMateFeatureData)); break;
                case "PosGroupFolder": featDataTypes.Add(typeof(IMateReference)); break;

                //---------------Body-----------------
                case "SmartComponentFeature": featDataTypes.Add(typeof(ISmartComponentFeatureData)); break;
                case "AdvHoleWzd": featDataTypes.Add(typeof(IAdvancedHoleFeatureData)); break;
                case "APattern": featDataTypes.Add(typeof(IFillPatternFeatureData)); break;
                case "BaseBody": featDataTypes.Add(typeof(IExtrudeFeatureData2)); break; 
                case "Bending": ThrowMatchNoneException(name, "Flex feature"); break;
                case "Blend": featDataTypes.Add(typeof(ILoftFeatureData)); break;
                case "BlendCut": featDataTypes.Add(typeof(ILoftFeatureData)); break;
                case "BodyExplodeStep": ThrowMatchNoneException(name); break;
                case "Boss":featDataTypes.Add(typeof(IExtrudeFeatureData2)); break; 
                case "BossThin":featDataTypes.Add(typeof(IExtrudeFeatureData2)); break; 
                case "Chamfer":featDataTypes.Add(typeof(IChamferFeatureData2)); break; 
                case "CirPattern": featDataTypes.Add(typeof(ICircularPatternFeatureData)); break;
                case "CombineBodies": featDataTypes.Add(typeof(ICombineBodiesFeatureData)); break;
                case "CosmeticThread": featDataTypes.Add(typeof(ICosmeticThreadFeatureData)); break;
                case "CosmeticWeldBead": featDataTypes.Add(typeof(ICosmeticWeldBeadFeatureData)); break;
                case "CreateAssemFeat": featDataTypes.Add(typeof(ISaveBodyFeatureData)); break;
                case "CurvePattern": featDataTypes.Add(typeof(ICurveDrivenPatternFeatureData)); break;
                case "Cut":featDataTypes.Add(typeof(IExtrudeFeatureData2)); break; 
                case "CutThin":featDataTypes.Add(typeof(IExtrudeFeatureData2)); break; 
                case "Deform": ThrowMatchNoneException(name); break;
                case "DeleteBody": featDataTypes.Add(typeof(IDeleteBodyFeatureData)); break;
                case "DelFace": featDataTypes.Add(typeof(IDeleteFaceFeatureData)); break;
                case "DerivedCirPattern": featDataTypes.Add(typeof(IDerivedPatternFeatureData)); break;
                case "DerivedLPattern": featDataTypes.Add(typeof(IDerivedPatternFeatureData)); break;
                case "DimPattern": featDataTypes.Add(typeof(IDimPatternFeatureData)); break;
                case "Dome":featDataTypes.Add(typeof(IDomeFeatureData2)); break; 
                case "Draft":featDataTypes.Add(typeof(IDraftFeatureData2)); break; 
                case "EdgeMerge": featDataTypes.Add(typeof(IHealEdgesFeatureData)); break;
                case "Emboss": featDataTypes.Add(typeof(IWrapSketchFeatureData)); break;
                case "Extrusion":featDataTypes.Add(typeof(IExtrudeFeatureData2)); break; 
                case "Fillet":featDataTypes.Add(typeof(ISimpleFilletFeatureData2)); break; 
                case "Helix": featDataTypes.Add(typeof(IHelixFeatureData)); break;
                case "HoleSeries":featDataTypes.Add(typeof(IHoleSeriesFeatureData2)); break; 
                case "HoleWzd":featDataTypes.Add(typeof(IWizardHoleFeatureData2)); break; 
                case "Imported": ThrowMatchNoneException(name); ; break;
                case "LocalChainPattern": featDataTypes.Add(typeof(IChainPatternFeatureData)); break;
                case "LocalCirPattern": featDataTypes.Add(typeof(ILocalCircularPatternFeatureData)); break;
                case "LocalCurvePattern": featDataTypes.Add(typeof(ILocalCurvePatternFeatureData)); break;
                case "LocalLPattern": featDataTypes.Add(typeof(ILocalLinearPatternFeatureData)); break;
                case "LocalSketchPattern": featDataTypes.Add(typeof(ILocalSketchPatternFeatureData)); break;
                case "LPattern": featDataTypes.Add(typeof(ILinearPatternFeatureData)); break;
                case "MacroFeature": featDataTypes.Add(typeof(IMacroFeatureData)); break;
                case "MirrorCompFeat": throw new InvalidOperationException("IMirrorComponentFeatureData Can not Get in SolidWorks.Interop.SldWorks")/*featDataTypes.Add(typeof(IMirrorComponentFeatureData))*/; break;
                case "MirrorPattern": featDataTypes.Add(typeof(IMirrorPatternFeatureData)); break;
                case "MirrorSolid": featDataTypes.Add(typeof(IMirrorSolidFeatureData)); break;
                case "MirrorStock": featDataTypes.Add(typeof(IMirrorPartFeatureData)); break;
                case "MoveCopyBody": featDataTypes.Add(typeof(IMoveCopyBodyFeatureData)); break;
                case "NetBlend": featDataTypes.Add(typeof(IBoundaryBossFeatureData)); break;
                case "PrtExploder": ThrowMatchNoneException(name); break;
                case "Punch": featDataTypes.Add(typeof(IIndentFeatureData)); break;
                case "ReplaceFace": featDataTypes.Add(typeof(IReplaceFaceFeatureData)); break;
                case "RevCut":featDataTypes.Add(typeof(IRevolveFeatureData2)); break;
                case "Round fillet corner": featDataTypes.Add(typeof(ISimpleFilletFeatureData2)); break;
                case "Revolution":featDataTypes.Add(typeof(IRevolveFeatureData2)); break; 
                case "RevolutionThin":featDataTypes.Add(typeof(IRevolveFeatureData2)); break; 
                case "Rib":featDataTypes.Add(typeof(IRibFeatureData)); break; 
                case "Rip": featDataTypes.Add(typeof(IRipFeatureData)); break;
                case "Sculpt": featDataTypes.Add(typeof(IIntersectFeatureData)); break;
                case "Shape": ThrowMatchNoneException(name); break;
                case "Shell": featDataTypes.Add(typeof(IShellFeatureData)); break;
                case "SketchHole":featDataTypes.Add(typeof(ISimpleHoleFeatureData2)); break; 
                case "SketchPattern": featDataTypes.Add(typeof(ISketchPatternFeatureData)); break;
                case "Split": featDataTypes.Add(typeof(ISplitBodyFeatureData)); break;
                case "SplitBody": ThrowMatchNoneException(name); break;
                case "Stock": featDataTypes.Add(typeof(IDerivedPartFeatureData)); break;
                case "Sweep": featDataTypes.Add(typeof(ISweepFeatureData)); break;
                case "SweepCut": featDataTypes.Add(typeof(ISweepFeatureData)); break;
                case "SweepThread": featDataTypes.Add(typeof(IThreadFeatureData)); break;
                case "TablePattern": featDataTypes.Add(typeof(ITablePatternFeatureData)); break;
                case "Thicken": featDataTypes.Add(typeof(IThickenFeatureData)); break;
                case "ThickenCut": featDataTypes.Add(typeof(IThickenFeatureData)); break;
                case "VarFillet": featDataTypes.Add(typeof(IVariableFilletFeatureData2)); break; 

                //---------------Drawing-----------------
                case "BendTableAchor": featDataTypes.Add(typeof(ITableAnchor)); break;
                case "BomFeat": featDataTypes.Add(typeof(IBomFeature)); break;
                case "BomTemplate": featDataTypes.Add(typeof(ITableAnchor)); break;
                case "DetailCircle": featDataTypes.Add(typeof(IDetailCircle)); break;
                case "DrBreakoutSectionLine":featDataTypes.Add(typeof(IDrSection)); featDataTypes.Add(typeof(IBrokenOutSectionFeatureData)); break;
                case "DrSectionLine": featDataTypes.Add(typeof(IDrSection)); break;
                case "GeneralTableAnchor": featDataTypes.Add(typeof(ITableAnchor)); break;
                case "HoleTableAnchor": featDataTypes.Add(typeof(ITableAnchor)); break;
                case "LiveSection": featDataTypes.Add(typeof(IRefPlane)); break;
                case "PunchTableAnchor": featDataTypes.Add(typeof(ITableAnchor)); break;
                case "RevisionTableAnchor": featDataTypes.Add(typeof(ITableAnchor)); break;
                case "WeldmentTableAnchor": featDataTypes.Add(typeof(ITableAnchor)); break;
                case "WeldTableAnchor": featDataTypes.Add(typeof(ITableAnchor)); break;

                //-------------Folder---------------
                case "BlockFolder": ThrowMatchNoneException(name); break;
                case "CommentsFolder": featDataTypes.Add(typeof(ICommentFolder)); break;
                case "CosmeticWeldSubFolder": featDataTypes.Add(typeof(ICosmeticWeldBeadFolder)); break;
                case "CutListFolder": featDataTypes.Add(typeof(IBodyFolder)); break;
                case "FeatSolidBodyFolder": featDataTypes.Add(typeof(IBodyFolder)); break;
                case "FeatSurfaceBodyFolder": featDataTypes.Add(typeof(IBodyFolder)); break;
                case "FtrFolder": featDataTypes.Add(typeof(IFeatureFolder)); break;
                case "InsertedFeatureFolder": featDataTypes.Add(typeof(IFeatureFolder)); break;
                case "MateReferenceGroupFolder": featDataTypes.Add(typeof(IFeatureFolder)); break;
                case "ProfileFtrFolder": featDataTypes.Add(typeof(IFeatureFolder)); break;
                case "RefAxisFtrFolder": featDataTypes.Add(typeof(IFeatureFolder)); break;
                case "RefPlaneFtrFolder": featDataTypes.Add(typeof(IFeatureFolder)); break;
                case "SketchSliceFolder": featDataTypes.Add(typeof(IFeatureFolder)); break;
                case "SolidBodyFolder": featDataTypes.Add(typeof(IBodyFolder)); break;
                case "SubAtomFolder": featDataTypes.Add(typeof(IBodyFolder)); break;//if a body
                case "SubWeldFolder": featDataTypes.Add(typeof(IBodyFolder)); break;
                case "SurfaceBodyFolder": featDataTypes.Add(typeof(IBodyFolder)); break;
                case "TemplateFlatPattern": featDataTypes.Add(typeof(IFlatPatternFolder)); break;
                
                //-------------Imported File-------------
                case "MBimport": throw new InvalidOperationException($"{name}-IImport3DInterconnectData Can not get {nameof(SolidWorks.Interop.sldworks)}")/*featDataTypes.Add(typeof(IImport3DInterconnectData))*/;

                //------------Miscellaneous-----------
                case "Attribute": featDataTypes.Add(typeof(IAttribute)); break;
                case "BlockDef": ThrowMatchNoneException(name); break;
                case "CurveInFile": featDataTypes.Add(typeof(IFreePointCurveFeatureData)); break;
                case "GridFeature": ThrowMatchNoneException(name); break;
                case "LibraryFeature": featDataTypes.Add(typeof(ILibraryFeatureData)); break;
                case "Scale": featDataTypes.Add(typeof(IScaleFeatureData)); break;
                case "Sensor": featDataTypes.Add(typeof(ISensor)); break;
                case "ViewBodyFeature": ThrowMatchNoneException(name); break;

                //-----------Mold--------------
                case "Cavity": featDataTypes.Add(typeof(ICavityFeatureData)); break;
                case "MoldCoreCavitySolids": featDataTypes.Add(typeof(IToolingSplitFeatureData)); break;
                case "MoldPartingGeom": featDataTypes.Add(typeof(IPartingSurfaceFeatureData)); break;
                case "MoldPartLine": featDataTypes.Add(typeof(IPartingLineFeatureData)); break;
                case "MoldShutOffSrf": featDataTypes.Add(typeof(IShutOffSurfaceFeatureData)); break;
                case "SideCore": featDataTypes.Add(typeof(ICoreFeatureData)); break;
                case "XformStock":featDataTypes.Add(typeof(DerivedPartFeatureData)); break;

                //-----------Motion and Simulation--------
                case "AEM3DContact":featDataTypes.Add(typeof(ISimulation3DContactFeatureData));break;
                case "AEMGravity": featDataTypes.Add(typeof(ISimulationGravityFeatureData)); break;
                case "AEMLinearDamper": featDataTypes.Add(typeof(ISimulationDamperFeatureData)); break;
                case "AEMLinearMotor": featDataTypes.Add(typeof(ISimulationMotorFeatureData)); break;
                case "AEMLinearSpring": featDataTypes.Add(typeof(ISimulationLinearSpringFeatureData)); break;
                case "AEMRotationalMotor": featDataTypes.Add(typeof(ISimulationMotorFeatureData)); break;
                case "AEMTorque": featDataTypes.Add(typeof(ISimulationForceFeatureData)); break;
                case "AEMTorsionalDamper": featDataTypes.Add(typeof(ISimulationDamperFeatureData)); break;
                case "AEMTorsionalSpring": ThrowMatchNoneException(name); break;
                case "SimPlotFeature": featDataTypes.Add(typeof(IMotionPlotFeatureData)); break;
                case "SimPlotXAxisFeature": featDataTypes.Add(typeof(IMotionPlotAxisFeatureData)); break;
                case "SimPlotYAxisFeature": featDataTypes.Add(typeof(IMotionPlotAxisFeatureData)); break;
                case "SimResultFolder":ThrowMatchNoneException(name); /*featDataTypes.Add(typeof(IMotionStudyResults));*/ break;

                //--------------Reference Geometry--------------
                case "BoundingBox": featDataTypes.Add(typeof(IBoundingBoxFeatureData)); break;
                case "CoordSys": featDataTypes.Add(typeof(ICoordinateSystemFeatureData)); break;
                case "GroundPlane": featDataTypes.Add(typeof(IGroundPlaneFeatureData)); break;
                case "RefAxis":featDataTypes.Add(typeof(IRefAxis)); featDataTypes.Add(typeof(IRefAxisFeatureData)); break; 
                case "RefPlane":featDataTypes.Add(typeof(IRefPlane)); featDataTypes.Add(typeof(IRefPlaneFeatureData)); break;

                //------------------Scenes, Lights, and Cameras------------
                case "AmbientLight": featDataTypes.Add(typeof(ILight)); break;
                case "CameraFeature": featDataTypes.Add(typeof(ICamera)); break;
                case "DirectionLight": featDataTypes.Add(typeof(ILight)); break;
                case "PointLight": featDataTypes.Add(typeof(ILight)); break;
                case "SpotLight": featDataTypes.Add(typeof(ILight)); break;

                //----------------Sheet Metal---------------
                case "SMBaseFlange": featDataTypes.Add(typeof(IBaseFlangeFeatureData)); break;
                case "BreakCorner": featDataTypes.Add(typeof(IBreakCornerFeatureData)); break;
                case "CornerTrim": featDataTypes.Add(typeof(IBreakCornerFeatureData)); break;
                case "CrossBreak": featDataTypes.Add(typeof(ICrossBreakFeatureData)); break;
                case "EdgeFlange": featDataTypes.Add(typeof(IEdgeFlangeFeatureData)); break;
                case "FlatPattern": featDataTypes.Add(typeof(IFlatPatternFeatureData)); break;
                case "FlattenBends": featDataTypes.Add(typeof(IBendsFeatureData)); break;
                case "Fold": featDataTypes.Add(typeof(IFoldsFeatureData)); break;
                case "FormToolInstance": ThrowMatchNoneException(name); break;
                case "Hem": featDataTypes.Add(typeof(IHemFeatureData)); break;
                case "Jog": featDataTypes.Add(typeof(IJogFeatureData)); break;
                case "LoftedBend": featDataTypes.Add(typeof(ILoftedBendsFeatureData)); break;
                case "NormalCut":featDataTypes.Add(typeof(ISMNormalCutFeatureData2)); break;
                case "OneBend": featDataTypes.Add(typeof(IOneBendFeatureData)); break;
                case "ProcessBends": featDataTypes.Add(typeof(IBendsFeatureData)); break;
                case "SheetMetal": featDataTypes.Add(typeof(ISheetMetalFeatureData)); break;
                case "SketchBend":featDataTypes.Add(typeof(IOneBendFeatureData)); break;
                case "SM3dBend": featDataTypes.Add(typeof(ISketchedBendFeatureData));break;
                //case "SMBaseFlange": featDataTypes.Add(typeof(IBaseFlangeFeatureData)); break;
                case "SMGusset": featDataTypes.Add(typeof(ISMGussetFeatureData)); break;
                case "SMMiteredFlange": featDataTypes.Add(typeof(IMiterFlangeFeatureData)); break;
                case "TemplateSheetMetal": featDataTypes.Add(typeof(ISheetMetalFolder)); break;
                case "ToroidalBend": featDataTypes.Add(typeof(IOneBendFeatureData)); break;
                case "UnFold":featDataTypes.Add(typeof(IFoldsFeatureData)); break;

                //-------------Sketch
                case "3DProfileFeature":featDataTypes.Add(typeof(ISketch));break;
                case "3DSplineCurve":featDataTypes.Add(typeof(IReferenceCurve));featDataTypes.Add(typeof(IReferencePointCurveFeatureData));break;
                case "CompositeCurve":featDataTypes.Add(typeof(IReferenceCurve)); featDataTypes.Add(typeof(ICompositeCurveFeatureData)); break;
                case "ImportedCurve":featDataTypes.Add(typeof(IReferenceCurve)); featDataTypes.Add(typeof(IImportedCurveFeatureData)); break; 
                case "PLine": featDataTypes.Add(typeof(ISplitLineFeatureData)); break;
                case "ProfileFeature": featDataTypes.Add(typeof(ISketch)); break;
                case "RefCurve":featDataTypes.Add(typeof(IReferenceCurve));featDataTypes.Add(typeof(IProjectionCurveFeatureData)); break;
                case "RefPoint":featDataTypes.Add(typeof(IRefPoint));featDataTypes.Add(typeof(IRefPointFeatureData)); break;
                case "SketchBlockDef": featDataTypes.Add(typeof(ISketchBlockDefinition)); break;
                case "SketchBlockInst": featDataTypes.Add(typeof(ISketchBlockInstance)); break;
                case "SketchBitmap": featDataTypes.Add(typeof(ISketchPicture)); break;
                case "BlendRefSurface": ThrowMatchNoneException(name); break;
                case "ExtendRefSurface": featDataTypes.Add(typeof(ISurfaceExtendFeatureData)); break;
                case "ExtruRefSurface": featDataTypes.Add(typeof(ISurfExtrudeFeatureData)); break;
                case "FillRefSurface": featDataTypes.Add(typeof(IFillSurfaceFeatureData)); break;
                case "FlattenSurface": featDataTypes.Add(typeof(ISurfaceFlattenFeatureData)); break;
                case "MidRefSurface":featDataTypes.Add(typeof(IMidSurface3)); break; 
                case "OffsetRefSuface": featDataTypes.Add(typeof(ISurfaceOffsetFeatureData)); break;
                case "PlanarSurface": featDataTypes.Add(typeof(ISurfacePlanarFeatureData)); break;
                case "RadiateRefSurface": featDataTypes.Add(typeof(ISurfaceRadiateFeatureData)); break;
                case "RefSurface": ThrowMatchNoneException(name); break;
                case "RevolvRefSurf": featDataTypes.Add(typeof(ISurfRevolveFeatureData)); break;
                case "RuledSrfFromEdge": featDataTypes.Add(typeof(IRuledSurfaceFeatureData)); break;
                case "SewRefSurface": featDataTypes.Add(typeof(ISurfaceKnitFeatureData)); break;
                case "SurfCut": featDataTypes.Add(typeof(ISurfaceCutFeatureData)); break;
                case "SweepRefSurface": featDataTypes.Add(typeof(ISweepFeatureData)); break;
                case "TrimRefSurface": featDataTypes.Add(typeof(ISurfaceTrimFeatureData)); break;
                case "UnTrimRefSurf": ThrowMatchNoneException(name); break;
                case "Weldment":featDataTypes.Add(typeof(IEndCapFeatureData)); break; 
                case "Gusset": featDataTypes.Add(typeof(IGussetFeatureData)); break;
                case "WeldBeadFeat": featDataTypes.Add(typeof(IWeldmentBeadFeatureData)); break;
                case "WeldCornerFeat": featDataTypes.Add(typeof(IWeldmentTrimExtendFeatureData)); break;
                case "WeldMemberFeat": featDataTypes.Add(typeof(IStructuralMemberFeatureData)); break;
                case "WeldmentFeature": featDataTypes.Add(typeof(IStructuralMemberFeatureData)); break;
                case "WeldmentTableFeat": featDataTypes.Add(typeof(IWeldmentCutListFeature)); break;
                default:
                    break;
            }

            return featDataTypes;
        }

        private static void ThrowMatchNoneException(string name,string msg = "")
        {
            throw new InvalidOperationException($"{name} Match None -- {msg}");
        }
    }
}
