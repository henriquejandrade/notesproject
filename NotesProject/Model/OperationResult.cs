using System;
using System.Collections.Generic;
using System.Text;

namespace NotesProject.Model
{
    class OperationResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public OperationResult(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        public OperationResult(int code, string message,object data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }
    }
}
