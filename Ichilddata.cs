using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using vacrem.Models;

namespace vacrem
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "Ichilddata" in both code and config file together.
    [ServiceContract]
    public interface Ichilddata
    { 
        [OperationContract]
        void DoWork();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/VR/Child/{uid}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<child> childlist(string uid);

    }
}
