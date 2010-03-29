//#region using
//using System;
//using Ximura;
//#endregion 
//namespace Ximura.Data
//{
//    public interface ICDSHelper
//    {
//        string Execute(Type contentType, CDSData rq, Content inData, out Content outData);

//        string Execute(Type contentType, CDSData rq, out Guid? cid, out Guid? vid);

//        string Execute(Type contentType, CDSData rq);

//        string Execute(Type contentType, CDSData rq, out Content outData);

//        string Execute(Type contentType, CDSData rq, Content inData);

//        string Execute<T>(CDSData rq, out Guid? cid, out Guid? vid) where T : Content;

//        string Execute<T>(CDSData rq, T inData, out Guid? newVersionID) where T : Content;

//        string Execute<T>(CDSData rq) where T : Content;

//        string Execute<T>(CDSData rq, T inData) where T : Content;

//        string Execute<T>(CDSData rq, T inData, out T outData) where T : Content;

//        string Execute<T>(CDSData rq, out T outData) where T : Content;
//    }
//}
