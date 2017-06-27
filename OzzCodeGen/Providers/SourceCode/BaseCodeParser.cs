using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.Providers.SourceCode
{
    public abstract class BaseCodeParser : IDisposable
    {
        protected CodeFile CodeFile { get; set; }

        public string ClassPrefix { get; set; }
        public bool StripClassPrefixes { get; set; }

        public abstract void ParseInnerCodeBlocks(SourceCodeBlock parent);
        public abstract string ParseValue(SourceCodeBlock codeBlock);

        public virtual string GetFolderPath()
        {
            return CodeFile.Directory.FullName;
        }

        public virtual IEnumerable<string> GetSubFolders()
        {
            return GetSubFolders(GetFolderPath());
        }

        protected virtual IEnumerable<string> GetSubFolders(string folderPath)
        {
            var dirs = Directory
                    .EnumerateDirectories(GetFolderPath(), "*", SearchOption.AllDirectories);
            return dirs;
        }

        public virtual void ParseFile(CodeFile codeFile)
        {
            this.CodeFile = codeFile;
            codeFile.FileContent = new SourceCodeBlock(this);
            codeFile.FileContent.InnerText = codeFile.ReadAll();
        }

        public virtual string GetValueByKey(SourceCodeBlock codeBlock, string key)
        {
            var child = GetChildByKey(codeBlock, key);
            var innerChild = child != null ?
                GetChildByKey(child, key) :
                GetChildByKey(codeBlock.InnerCodeBlocks.FirstOrDefault(), key);

            if (innerChild != null)
            {
                return ParseValue(innerChild);
            }
            else if (child != null)
            {
                return ParseValue(child);
            }
            else
            {
                return string.Empty;
            }
        }

        public virtual SourceCodeBlock GetChildByKey(SourceCodeBlock codeBlock, string key)
        {
            return codeBlock
                        .InnerCodeBlocks
                        .FirstOrDefault(r => r.InnerText.StartsWith(key));
        }

        protected virtual void ParseSemicolons(SourceCodeBlock parent)
        {
            var innerChars = parent.InnerText.ToCharArray();
            int openBrackets = 0;
            int totalSemicolons = 0;
            bool brackectBeforeSemicolon = false;
            char prevChar = ' ';
            bool inComment = false;
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool inInlineComment = false;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            StringBuilder sb = new StringBuilder();
            foreach (char item in innerChars)
            {
                if (prevChar == '/' && item == '*' && openBrackets == 0)
                {
                    inComment = true;
                    sb.Remove(sb.Length - 1, 1);
                }

                //Inline comments
                if (prevChar == '/' && item == '/' && openBrackets == 0)
                {
                    inInlineComment = true;
                    //TODO: Uncomment below to strip inline comments.
                    //sb.Remove(sb.Length - 1, 1);
                }
                if (item == '\r') inInlineComment = false;

                //TODO: Strip inline comments too.
                if (!inComment) sb.Append(item);

                if (prevChar == '*' && item == '/') inComment = false;

                if (item == ';') totalSemicolons++;
                if (item == '}') openBrackets--;
                if (item == '{')
                {
                    openBrackets++;
                    if (totalSemicolons == 0) brackectBeforeSemicolon = true;
                }
                if (item == ';' && openBrackets == 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                    var child = parent.AddChildBlock(sb.ToString().Trim());
                    if (child != null && brackectBeforeSemicolon)
                    {
                        ParseBetweenBrackets(child);
                    }
                    sb.Clear();
                    totalSemicolons = 0;
                    brackectBeforeSemicolon = false;
                }
                prevChar = item;
            }
        }

        /// <summary>
        /// Parses SourceCodeBlock from object's InnerCodeBlock
        /// </summary>
        /// <param name="parent"></param>
        protected virtual void ParseBetweenBrackets(SourceCodeBlock parent)
        {
            var innerChars = parent.InnerText.ToCharArray();
            int openBrackets = 0;
            bool anyOpen = false;
            StringBuilder sb = new StringBuilder();

            foreach (char item in innerChars)
            {
                if (item == '}') openBrackets--;

                if (openBrackets > 0) sb.Append(item);
                if (anyOpen && openBrackets == 0)
                {
                    parent.AddChildBlock(sb.ToString().Trim());
                    sb.Clear();
                    anyOpen = false;
                }

                if (item == '{')
                {
                    openBrackets++;
                    anyOpen = true;
                }
            }
        }

        public virtual void Dispose()
        {
            CodeFile = null;
        }
    }
}
