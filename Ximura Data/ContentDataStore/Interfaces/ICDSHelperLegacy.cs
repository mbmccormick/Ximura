//#region using
//using System;
//using Ximura;
//#endregion
//namespace Ximura.Data
//{
//    public interface ICDSHelperLegacy : ICDSHelper
//    {
//        CDSResponse Create(Content inData);

//        CDSResponse Read<T>(Guid? CID, Guid? VID, out T data) where T : Content;
//        CDSResponse Read<T>(string refType, string refValue, out T data) where T : Content;

//        CDSResponse Update(Content inData);
//        CDSResponse Update(Content inData, out Content outData);
//        CDSResponse Update<T>(T inData) where T : Content;
//        CDSResponse Update<T>(T inData, out T outData) where T : Content;

//        CDSResponse Delete(Type objectType, Guid? CID, Guid? VID);
//        CDSResponse Delete(Type objectType, string refType, string refValue);
//        CDSResponse Delete<T>(string refType, string refValue) where T : Content;
//        CDSResponse Delete<T>(Guid? CID, Guid? VID) where T : Content;

//        CDSResponse VersionCheck(Type objectType, Guid? CID, Guid? VID, out Guid? cid, out Guid? vid);
//        CDSResponse VersionCheck(Type objectType, string refType, string refValue, out Guid? cid, out Guid? vid);
//        CDSResponse VersionCheck<T>(Guid? CID, Guid? VID, out Guid? cid, out Guid? vid) where T : Content;
//        CDSResponse VersionCheck<T>(string refType, string refValue, out Guid? cid, out Guid? vid) where T : Content;

//        CDSHelper.BrowseContext<T> Browse<T>() where T : Content;
//        CDSHelper.BrowseContext<T> Browse<T>(CDSHelper.CDSBrowseConstraints constraints) where T : Content;
//    }
//}
