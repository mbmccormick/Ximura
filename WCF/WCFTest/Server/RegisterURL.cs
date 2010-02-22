using System;
using System.Xml;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace Ximura.WCF
{

    /// <summary>
    /// This class reserves the namespace for the application.
    /// </summary>
    public class ReserveHttpNamespace
    {
        //static void Main(string[] args)
        //{
        //    if (args.Length != 2)
        //    {
        //        Console.WriteLine(
        //        "Usage: reserveHttpNamespace " +
        //        "prefix account");
        //        return;
        //    }
        //    try
        //    {
        //        ModifyReservation(args[0], args[1], false);
        //        Console.WriteLine("Success!");
        //    }
        //    catch (Exception x)
        //    {
        //        Console.WriteLine(x.Message);
        //    }
        //}

        static void ModifyReservation(string urlPrefix,
                                      string accountName,
                                      bool removeReservation)
        {
            string sddl = createSddl(accountName);

            HTTP_SERVICE_CONFIG_URLACL_SET configInfo;

            configInfo.Key.UrlPrefix = urlPrefix;

            configInfo.Param.Sddl = sddl;
            
            HTTPAPI_VERSION httpApiVersion =
                new HTTPAPI_VERSION(1, 0);

            int errorCode = HttpInitialize(httpApiVersion,
                HTTP_INITIALIZE_CONFIG, IntPtr.Zero);

            if (0 != errorCode) throw getException(
                "HttpInitialize",
                errorCode);

            try
            {
                // do our best to delete any existing ACL
                errorCode = HttpDeleteServiceConfigurationAcl(
                    IntPtr.Zero,
                    HttpServiceConfigUrlAclInfo,
                    ref configInfo,
                    Marshal.SizeOf(
                        typeof(HTTP_SERVICE_CONFIG_URLACL_SET)),
                    IntPtr.Zero);
                if (removeReservation)
                {
                    if (0 != errorCode) throw getException(
                        "HttpDeleteServiceConfigurationAcl",
                        errorCode);
                    return;
                }
                errorCode = HttpSetServiceConfigurationAcl(
                    IntPtr.Zero,
                    HttpServiceConfigUrlAclInfo,
                    ref configInfo,
                    Marshal.SizeOf(
                        typeof(HTTP_SERVICE_CONFIG_URLACL_SET)),
                    IntPtr.Zero);
                if (0 != errorCode) throw getException(
                    "HttpSetServiceConfigurationAcl",
                    errorCode);
            }
            finally
            {
                errorCode = HttpTerminate(
                    HTTP_INITIALIZE_CONFIG,
                    IntPtr.Zero);
                if (0 != errorCode) throw getException(
                    "HttpTerminate",
                    errorCode);
            }
        }

        private static Exception getException(string fcn, int errorCode)
        {
            Exception x = new Exception(
                string.Format("{0} failed: {1}",
                fcn, getWin32ErrorMessage(errorCode)));
            return x;
        }

        static string createSddl(string account)
        {
            string sid = new NTAccount(account).Translate(
                typeof(SecurityIdentifier)).ToString();
            // DACL that Allows Generic eXecute for the user
            // specified by account
            // see help for HTTP_SERVICE_CONFIG_URLACL_PARAM
            // for details on what this means
            return string.Format("D:(A;;GX;;;{0})", sid);
        }

        static string getWin32ErrorMessage(int errorCode)
        {
            int hr = HRESULT_FROM_WIN32(errorCode);
            Exception x = Marshal.GetExceptionForHR(hr);
            return x.Message;
        }

        static int HRESULT_FROM_WIN32(int errorCode)
        {
            if (errorCode <= 0) return errorCode;
            return (int)((0x0000FFFFU &
                ((uint)errorCode)) | (7U << 16) |
                0x80000000U);
        }

        // P/Invoke stubs from http.h
        const int HttpServiceConfigUrlAclInfo = 2;

        const int HTTP_INITIALIZE_CONFIG = 2;

        [StructLayout(LayoutKind.Sequential)]
        struct HTTPAPI_VERSION
        {
            public HTTPAPI_VERSION(short maj, short min)
            {
                Major = maj; Minor = min;
            }
            short Major;
            short Minor;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HTTP_SERVICE_CONFIG_URLACL_KEY
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string UrlPrefix;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HTTP_SERVICE_CONFIG_URLACL_PARAM
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string Sddl;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HTTP_SERVICE_CONFIG_URLACL_SET
        {
            public HTTP_SERVICE_CONFIG_URLACL_KEY Key;
            public HTTP_SERVICE_CONFIG_URLACL_PARAM Param;
        }

        [DllImport("httpapi.dll", ExactSpelling = true,
                EntryPoint = "HttpSetServiceConfiguration")]
        static extern int HttpSetServiceConfigurationAcl(
            IntPtr mustBeZero, int configID,
            [In] ref HTTP_SERVICE_CONFIG_URLACL_SET configInfo,
            int configInfoLength, IntPtr mustBeZero2);

        [DllImport("httpapi.dll", ExactSpelling = true,
                EntryPoint = "HttpDeleteServiceConfiguration")]
        static extern int HttpDeleteServiceConfigurationAcl(
            IntPtr mustBeZero, int configID,
            [In] ref HTTP_SERVICE_CONFIG_URLACL_SET configInfo,
            int configInfoLength, IntPtr mustBeZero2);

        [DllImport("httpapi.dll")]
        static extern int HttpInitialize(
            HTTPAPI_VERSION version,
            int flags, IntPtr mustBeZero);

        [DllImport("httpapi.dll")]
        static extern int HttpTerminate(int flags,
            IntPtr mustBeZero);
    }

}
