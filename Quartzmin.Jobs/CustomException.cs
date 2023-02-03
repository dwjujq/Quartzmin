using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Quartzmin.Jobs
{
    public class CustomException : Exception
    {

        public CustomException(string message, int code = 0) : base(message)
        {
            Code = code;
        }

        public CustomException(string message, Exception innerException, int code = 0) : base(message, innerException)
        {
            Code = code;
        }

        public CustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize data for our base classes.  base will verify info != null.
            base.GetObjectData(info, context);

        }

        public int Code { get; private set; }
    }
}
