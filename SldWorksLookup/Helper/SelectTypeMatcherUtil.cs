using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;

namespace SldWorksLookup
{
    public static class SelectTypeMatcherUtil
    {
        /// <summary>
        /// 根据枚举值匹配类型
        /// </summary>
        /// <param name="selectType"><see cref="swSelectType_e"/></param>
        /// <returns><see cref="SolidWorks.Interop.sldworks"/> 下的 接口类型 I[Type]</returns>
        public static Type Match(swSelectType_e selectType)
        {
            var type = default(Type);
            switch (selectType)
            {
                case swSelectType_e.swSelNOTHING:
                    type = null;
                    break;
                case swSelectType_e.swSelEDGES:
                    type = typeof(IEdge);
                    break;
                case swSelectType_e.swSelFACES:
                    type = typeof(IFace2);
                    break;
                case swSelectType_e.swSelVERTICES:
                    type = typeof(IVertex);
                    break;
                case swSelectType_e.swSelDATUMPLANES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelDATUMAXES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelDATUMPOINTS:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelOLEITEMS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelATTRIBUTES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelSKETCHES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelSKETCHSEGS:
                    type = typeof(ISketchSegment);
                    break;
                case swSelectType_e.swSelSKETCHPOINTS:
                    type = typeof(ISketchPoint);
                    break;
                case swSelectType_e.swSelDRAWINGVIEWS:
                    type = typeof(IView);
                    break;
                case swSelectType_e.swSelGTOLS:
                    type = typeof(IGtol);
                    break;
                case swSelectType_e.swSelDIMENSIONS:
                    type = typeof(DisplayDimension);
                    break;
                case swSelectType_e.swSelNOTES:
                    type = typeof(INote);
                    break;
                case swSelectType_e.swSelSECTIONLINES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelDETAILCIRCLES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelSECTIONTEXT:
                    throw new InvalidOperationException("Not Supported");                    
                case swSelectType_e.swSelSHEETS:
                    type = typeof(ISheet);
                    break;
                case swSelectType_e.swSelCOMPONENTS:
                    type = typeof(IComponent2);
                    break;
                case swSelectType_e.swSelMATES:
                    type = typeof(IMate);
                    break;
                case swSelectType_e.swSelBODYFEATURES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelREFCURVES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelEXTSKETCHSEGS:
                    type = typeof(ISketchSegment);
                    break;
                case swSelectType_e.swSelEXTSKETCHPOINTS:
                    type = typeof(ISketchPoint);//Or Orign Point
                    break;
                case swSelectType_e.swSelHELIX:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelREFSURFACES:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelCENTERMARKS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelINCONTEXTFEAT:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelMATEGROUP:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelBREAKLINES:
                    type = typeof(IBreakLine);
                    break;
                case swSelectType_e.swSelINCONTEXTFEATS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelMATEGROUPS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelSKETCHTEXT:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelSFSYMBOLS:
                    type = typeof(ISFSymbol);
                    break;
                case swSelectType_e.swSelDATUMTAGS:
                    type = typeof(IDatumTag);
                    break;
                case swSelectType_e.swSelCOMPPATTERN:
                    throw new InvalidOperationException("Not Supported"); 
                case swSelectType_e.swSelWELDS:
                    type = typeof(IWeldSymbol);
                    break;
                case swSelectType_e.swSelCTHREADS:
                    type = typeof(IFeature);//Or ICThread
                    break;
                case swSelectType_e.swSelDTMTARGS:
                    type = typeof(IDatumTargetSym);
                    break;
                case swSelectType_e.swSelPOINTREFS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelDCABINETS:
                    throw new InvalidOperationException("Not Supported"); ;
                case swSelectType_e.swSelEXPLVIEWS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelEXPLSTEPS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelEXPLLINES:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelSILHOUETTES:
                    type = typeof(ISilhouetteEdge);
                    break;
                case swSelectType_e.swSelCONFIGURATIONS:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelOBJHANDLES:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelARROWS:
                    type = typeof(IProjectionArrow);
                    break;
                case swSelectType_e.swSelZONES:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelREFEDGES:
                    type = typeof(IEdge);
                    break;
                case swSelectType_e.swSelREFFACES:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelREFSILHOUETTE:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelBOMS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelEQNFOLDER:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelSKETCHHATCH:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelIMPORTFOLDER:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelVIEWERHYPERLINK:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelMIDPOINTS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelCUSTOMSYMBOLS:
                    type = typeof(ICustomSymbol);
                    break;
                case swSelectType_e.swSelCOORDSYS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelDATUMLINES:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelROUTECURVES:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelBOMTEMPS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelROUTEPOINTS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelCONNECTIONPOINTS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelROUTESWEEPS:
                    throw new InvalidOperationException("Not Supported");
                case swSelectType_e.swSelPOSGROUP:
                    break;
                case swSelectType_e.swSelBROWSERITEM:
                    break;
                case swSelectType_e.swSelFABRICATEDROUTE:
                    break;
                case swSelectType_e.swSelSKETCHPOINTFEAT:
                    break;
                case swSelectType_e.swSelEMPTYSPACE:
                    break;
                case swSelectType_e.swSelLIGHTS:
                    break;
                case swSelectType_e.swSelWIREBODIES:
                    break;
                case swSelectType_e.swSelSURFACEBODIES:
                    break;
                case swSelectType_e.swSelSOLIDBODIES:
                    break;
                case swSelectType_e.swSelFRAMEPOINT:
                    break;
                case swSelectType_e.swSelSURFBODIESFIRST:
                    break;
                case swSelectType_e.swSelMANIPULATORS:
                    break;
                case swSelectType_e.swSelPICTUREBODIES:
                    break;
                case swSelectType_e.swSelSOLIDBODIESFIRST:
                    break;
                case swSelectType_e.swSelHOLESERIES:
                    break;
                case swSelectType_e.swSelLEADERS:
                    break;
                case swSelectType_e.swSelSKETCHBITMAP:
                    break;
                case swSelectType_e.swSelDOWELSYMS:
                    type = typeof(IDowelSymbol);
                    break;
                case swSelectType_e.swSelEXTSKETCHTEXT:
                    break;
                case swSelectType_e.swSelBLOCKINST:
                    type = typeof(IBlockInstance);
                    break;
                case swSelectType_e.swSelFTRFOLDER:
                    break;
                case swSelectType_e.swSelSKETCHREGION:
                    break;
                case swSelectType_e.swSelSKETCHCONTOUR:
                    break;
                case swSelectType_e.swSelBOMFEATURES:
                    break;
                case swSelectType_e.swSelANNOTATIONTABLES:
                    type = typeof(ITableAnnotation);//Or ITitleBlockTableAnnotation
                    break;
                case swSelectType_e.swSelBLOCKDEF:
                    break;
                case swSelectType_e.swSelCENTERMARKSYMS:
                    break;
                case swSelectType_e.swSelSIMULATION:
                    break;
                case swSelectType_e.swSelSIMELEMENT:
                    break;
                case swSelectType_e.swSelCENTERLINES:
                    break;
                case swSelectType_e.swSelHOLETABLEFEATS:
                    break;
                case swSelectType_e.swSelHOLETABLEAXES:
                    break;
                case swSelectType_e.swSelWELDMENT:
                    break;
                case swSelectType_e.swSelSUBWELDFOLDER:
                    break;
                case swSelectType_e.swSelEXCLUDEMANIPULATORS:
                    break;
                case swSelectType_e.swSelREVISIONTABLE:
                    break;
                case swSelectType_e.swSelSUBSKETCHINST:
                    type = typeof(ISketchBlockInstance);
                    break;
                case swSelectType_e.swSelWELDMENTTABLEFEATS:
                    type = typeof(IWeldmentCutListFeature);
                    break;
                case swSelectType_e.swSelBODYFOLDER:
                    break;
                case swSelectType_e.swSelREVISIONTABLEFEAT:
                    break;
                case swSelectType_e.swSelSUBATOMFOLDER:
                    break;
                case swSelectType_e.swSelWELDBEADS:
                    break;
                case swSelectType_e.swSelEMBEDLINKDOC:
                    break;
                case swSelectType_e.swSelJOURNAL:
                    break;
                case swSelectType_e.swSelDOCSFOLDER:
                    break;
                case swSelectType_e.swSelCOMMENTSFOLDER:
                    break;
                case swSelectType_e.swSelCOMMENT:
                    type = typeof(IComment);
                    break;
                case swSelectType_e.swSelSWIFTANNOTATIONS:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelSWIFTFEATURES:
                    type = typeof(IMateLoadReference);
                    break;
                case swSelectType_e.swSelCAMERAS:
                    type = typeof(IFeature);
                    break;
                case swSelectType_e.swSelMATESUPPLEMENT:
                    break;
                case swSelectType_e.swSelANNOTATIONVIEW:
                    break;
                case swSelectType_e.swSelGENERALTABLEFEAT:
                    break;
                case swSelectType_e.swSelDISPLAYSTATE:
                    break;
                case swSelectType_e.swSelSUBSKETCHDEF:
                    break;
                case swSelectType_e.swSelSWIFTSCHEMA:
                    type = typeof(IDimXpertManager);
                    break;
                case swSelectType_e.swSelTITLEBLOCK:
                    type = typeof(ITitleBlock);
                    break;
                case swSelectType_e.swSelTITLEBLOCKTABLEFEAT:
                    type = typeof(TitleBlockTableFeature);
                    break;
                case swSelectType_e.swSelOBJGROUP:
                    break;
                case swSelectType_e.swSelPLANESECTIONS:
                    break;
                case swSelectType_e.swSelCOSMETICWELDS:
                    break;
                case swSelectType_e.SwSelMAGNETICLINES:
                    break;
                case swSelectType_e.swSelPUNCHTABLEFEATS:
                    break;
                case swSelectType_e.swSelREVISIONCLOUDS:
                    break;
                case swSelectType_e.swSelBorder:
                    break;
                case swSelectType_e.swSelSELECTIONSETFOLDER:
                    break;
                case swSelectType_e.swSelSELECTIONSETNODE:
                    break;
                case swSelectType_e.swSelEVERYTHING:
                    break;
                case swSelectType_e.swSelLOCATIONS:
                    break;
                case swSelectType_e.swSelUNSUPPORTED:
                    break;
                default:
                    break;
            }
            return type;
        }
    }
}
