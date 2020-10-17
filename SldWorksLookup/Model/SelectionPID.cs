using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SolidWorks.Interop.swconst;
using System;
using System.Text;
using System.Linq;

namespace SldWorksLookup.Model
{
    public enum PIDType
    {
        Base64,
        ByteArray,
    }

    public class SelectionPID : ObservableObject
    {
        private PIDType _pIDType;

        protected SelectionPID(swSelectType_e selectType, object selectedObject, int mark, byte[] pid)
        {
            SelectType = selectType;
            SelectedObject = selectedObject;
            Mark = mark;
            BytePID = pid;
        }

        public static SelectionPID Create(swSelectType_e selectType, object selectedObject, int mark, object pid)
        {
            var byteArray = pid as byte[];

            return new SelectionPID(selectType, selectedObject, mark, byteArray);
        }

        public swSelectType_e SelectType { get; }

        public object SelectedObject { get; }

        public int Mark { get; }

        public PIDType PIDType { get => _pIDType; set => Set(ref _pIDType ,value); }

        public string PID
        {
            get
            {
                return ByteArrayToString(BytePID);
            }
        }

        public byte[] BytePID { get; set; }

        public void Open()
        {

        }

        public string ByteArrayToString(byte[] byteArray)
        {
            string value = string.Empty ;

            if (byteArray == null || byteArray.Length < 2)
            {
                return $"Byte Error {byteArray?.Length}";
            }

            switch (PIDType)
            {
                case PIDType.ByteArray:
                    value = byteArray.Select(p => p.ToString()).Aggregate((p1, p2) => $"{p1} {p2}");
                    break;
                case PIDType.Base64:
                    value = Convert.ToBase64String(byteArray);
                    break;
                default:
                    break;
            }

            return value;
        }
    }
}
