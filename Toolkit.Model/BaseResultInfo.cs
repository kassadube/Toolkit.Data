using System;
using System.Xml.Linq;
using System.Collections.Specialized;
using Toolkit.Serialization;

namespace Toolkit.Model
{
    public class BaseResultInfo
    {
        public BaseErrorInfo Error { get; set; }

        public long TokenId { get; set; }

         public int ReturnValue { get; set; }

        public bool ResultValue { get { return ReturnValue == 0 ? false : true; } set { ReturnValue = value ? 1 : 0; } }

        public bool IsSucceded { get { return Error == null; } }

        public virtual object ResultObject { get; set; }
       
        public T GetResult<T>()  where T : class
        {
            return ResultObject as T;
        }

        public bool TryGetResult<T>(out T ToItem) where T : class
        {
            try
            {
                ToItem = ResultObject as T;
            }
            catch (Exception)
            {
                ToItem = null;
                return false;
            }

            return true;
        }

        public NameValueCollection Params { get; set; }

        public virtual XElement ToXElement()
        {
            XElement res = new XElement("Result");
            res.Add(new XElement("IsSucceded", this.IsSucceded.ToString()));
            if (Error != null)
            {
                XElement x = new XElement("ResultObject");
                x.Add(Serializer.Serialize(Error));
                res.Add(x);
            }
            return res;
        }
    }

    public class DataResultInfo<T> : BaseResultInfo where T : class
    {
        public new T ResultObject { get { return base.ResultObject as T; } set { base.ResultObject = value; } }

        
    }
}
