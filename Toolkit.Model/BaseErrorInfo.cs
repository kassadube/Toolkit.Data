using System;

namespace Toolkit.Model
{
    
    public class BaseErrorInfo
    {
        public string Name { get; set; }

        public long Code { get; set; }

        public int Severity { get; set; }

        public int ReturnValue { get; set; }

        private Exception EX { get; set; }

        public string Message { get; set; }

        public void BuildException(Exception ex)
        {
            EX = ex;
            string expMessage = "Message: {0} Source: {1} StackTrace: {2}; Description : {3} ErrorCode: {4} Source: Custom StackTrace: None;";
            Message = string.Format(expMessage, ex.Message, ex.Source, ex.StackTrace, Name, Code.ToString());
           
        }
    }
}
