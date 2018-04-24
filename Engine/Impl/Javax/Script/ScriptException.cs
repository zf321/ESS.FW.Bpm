using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Javax.Script
{
    public class ScriptException:System.Exception
    {
        private String fileName;
        private int lineNumber;
        private int columnNumber;
        public ScriptException(String s):base(s)
        {
            fileName = null;
            lineNumber = -1;
            columnNumber = -1;
        }

        /**
         * Creates a <code>ScriptException</code> wrapping an <code>Exception</code> thrown by an underlying
         * interpreter.  Line and column numbers and filename are unspecified.
         *
         * @param e The wrapped <code>Exception</code>.
         */
        public ScriptException(System.Exception e):base(e.Message,e)
        {
            fileName = null;
            lineNumber = -1;
            columnNumber = -1;
        }

        /**
         * Creates a <code>ScriptException</code> with message, filename and linenumber to
         * be used in error messages.
         *
         * @param message The string to use in the message
         *
         * @param fileName The file or resource name describing the location of a script error
         * causing the <code>ScriptException</code> to be thrown.
         *
         * @param lineNumber A line number describing the location of a script error causing
         * the <code>ScriptException</code> to be thrown.
         */
        public ScriptException(String message, String fileName, int lineNumber):base(message)
        {
            this.fileName = fileName;
            this.lineNumber = lineNumber;
            this.columnNumber = -1;
        }

        /**
         * <code>ScriptException</code> constructor specifying message, filename, line number
         * and column number.
         * @param message The message.
         * @param fileName The filename
         * @param lineNumber the line number.
         * @param columnNumber the column number.
         */
        public ScriptException(String message,
                String fileName,
                int lineNumber,
                int columnNumber):base(message)
        {
            this.fileName = fileName;
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
        }

        /**
         * Returns a message containing the String passed to a constructor as well as
         * line and column numbers and filename if any of these are known.
         * @return The error message.
         */
        public String getMessage()
        {
            String ret =Message;
            if (fileName != null)
            {
                ret += (" in " + fileName);
                if (lineNumber != -1)
                {
                    ret += " at line number " + lineNumber;
                }

                if (columnNumber != -1)
                {
                    ret += " at column number " + columnNumber;
                }
            }

            return ret;
        }

        /**
         * Get the line number on which an error occurred.
         * @return The line number.  Returns -1 if a line number is unavailable.
         */
        public int getLineNumber()
        {
            return lineNumber;
        }

        /**
         * Get the column number on which an error occurred.
         * @return The column number.  Returns -1 if a column number is unavailable.
         */
        public int getColumnNumber()
        {
            return columnNumber;
        }

        /**
         * Get the source of the script causing the error.
         * @return The file name of the script or some other string describing the script
         * source.  May return some implementation-defined string such as <i>&lt;unknown&gt;</i>
         * if a description of the source is unavailable.
         */
        public String getFileName()
        {
            return fileName;
        }
    }
}
