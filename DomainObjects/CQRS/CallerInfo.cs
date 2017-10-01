using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.CQRS
{
    public class CallerInfo
    {
        public string Caller { get; set; }
        public string MethodName { get; set; }
        public string SourceFile { get; set; }
        public int LineNumber { get; set; }
        public CallerInfo()
        {
            Caller = ""; MethodName = ""; SourceFile = "";
        }
        public CallerInfo(string caller = null, string methodName = null, string sourceFile = null, int lineNumber = 0)
        {
            this.Caller = caller;
            this.MethodName = methodName;
            this.SourceFile = sourceFile;
            this.LineNumber = lineNumber;
        }

        public static CallerInfo Create(string caller = null,
            [CallerMemberName] string methodName = null,
            [CallerFilePath] string sourceFile = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            return new CallerInfo(caller, methodName, sourceFile, lineNumber);
        }
    }
}
