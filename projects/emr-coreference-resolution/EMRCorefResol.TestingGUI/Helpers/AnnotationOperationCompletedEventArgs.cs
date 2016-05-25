using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRCorefResol.TestingGUI
{
    class AnnotationOperationCompletedEventArgs : EventArgs
    {
        public AnnotationOperationResult Result { get; }
        public Exception Exception { get; }
        public string Message { get; }

        public AnnotationOperationCompletedEventArgs(
            AnnotationOperationResult result)
        {
            Result = result;
        }

        public AnnotationOperationCompletedEventArgs(
            AnnotationOperationResult result,
            string message)
        {
            Result = result;
            Message = message;
        }

        public AnnotationOperationCompletedEventArgs(
            AnnotationOperationResult result,
            string message,
            Exception exception)
        {
            Result = result;
            Exception = exception;
        }
    }
}
