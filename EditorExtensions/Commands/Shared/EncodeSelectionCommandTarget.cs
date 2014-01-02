﻿using System;
using System.Web;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace MadsKristensen.EditorExtensions
{
    internal class EncodeSelection : CommandTargetBase
    {
        private DTE2 _dte;
        private static CommandId[] _commandIds = {
            CommandId.HtmlEncode,
            CommandId.HtmlDecode,
            CommandId.AttrEncode,
            CommandId.UrlEncode,
            CommandId.UrlDecode,
            CommandId.JsEncode,
        };

        private delegate string Replacement(string original);

        public EncodeSelection(IVsTextView adapter, IWpfTextView textView)
            : base(adapter, textView, CommandGuids.guidEditorExtensionsCmdSet, _commandIds)
        {
            _dte = EditorExtensionsPackage.DTE;
        }

        protected override bool Execute(uint commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            switch ((CommandId)commandId)
            {
                case CommandId.HtmlEncode:
                    return Replace(HttpUtility.HtmlEncode);
                case CommandId.HtmlDecode:
                    return Replace(HttpUtility.HtmlDecode);
                case CommandId.AttrEncode:
                    return Replace(HttpUtility.HtmlAttributeEncode);
                case CommandId.UrlEncode:
                    return Replace(HttpUtility.UrlEncode);
                case CommandId.UrlDecode:
                    return Replace(HttpUtility.UrlDecode);
                case CommandId.JsEncode:
                    return Replace(HttpUtility.JavaScriptStringEncode);
            }

            return false;
        }

        private bool Replace(Replacement callback)
        {
            TextDocument document = GetTextDocument();
            string replacement = callback(document.Selection.Text);

            using (EditorExtensionsPackage.UndoContext((callback.Method.Name)))
                document.Selection.Insert(replacement, 0);

            return true;
        }

        private TextDocument GetTextDocument()
        {
            return _dte.ActiveDocument.Object("TextDocument") as TextDocument;
        }

        protected override bool IsEnabled()
        {
            return !TextView.Selection.IsEmpty;
        }
    }
}