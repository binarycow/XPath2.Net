﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Schema;

namespace XQTSRunConsole
{
    public class XQTSRunner
    {
        public const String XQTSNamespace = "http://www.w3.org/2005/02/query-test-XQTSCatalog";

        internal string _basePath;
        internal string _queryOffsetPath;
        internal string _sourceOffsetPath;
        internal string _resultOffsetPath;
        internal string _queryFileExtension;

        internal NameTable _nameTable;
        internal XmlNamespaceManager _nsmgr;
        internal XmlDocument _catalog;
        internal DataTable _testTab;
        internal Dictionary<string, string> _sources;
        internal Dictionary<string, string> _module;
        internal Dictionary<string, string[]> _collection;
        internal Dictionary<string, string[]> _schema;
        internal string _lastFindString = "";
        internal HashSet<String> _ignoredTest;

        internal int _total;
        internal int _passed;
        internal int _repeatCount;

        internal static string[] s_ignoredTest =
        {
            "nametest-1", "nametest-2", "nametest-5", "nametest-6",
            "nametest-7", "nametest-8", "nametest-9", "nametest-10",
            "nametest-11", "nametest-12", "nametest-13", "nametest-14",
            "nametest-15", "nametest-16", "nametest-17", "nametest-18",
            "CastAs660", "CastAs661", "CastAs662", "CastAs663",
            "CastAs664", "CastAs665", "CastAs666", "CastAs667",
            "CastAs668", "CastAs669", "CastAs671", "CastableAs648",
            "fn-trace-2", "fn-trace-9",
            "NodeTesthc-1", "NodeTesthc-2", "NodeTesthc-3", "NodeTesthc-4",
            "NodeTesthc-5", "NodeTesthc-6", "NodeTesthc-7", "NodeTesthc-8",
            "fn-max-3", "fn-min-3",
            "defaultnamespacedeclerr-1", "defaultnamespacedeclerr-2",
            "fn-document-uri-12", "fn-document-uri-15", "fn-document-uri-16",
            "fn-document-uri-17", "fn-document-uri-18", "fn-document-uri-19",
            "fn-prefix-from-qname-8", "boundaryspacedeclerr-1",
            "fn-resolve-uri-2",
            "ancestor-21", "ancestorself-21", "following-21",
            "followingsibling-21", "preceding-21", "preceding-sibling-21"
        };

        private void OpenCatalog(string fileName)
        {
            _catalog = new XmlDocument(_nameTable);

            var schemaSet = new XmlSchemaSet();
            var settings = new XmlReaderSettings
            {
                Schemas = schemaSet,
                DtdProcessing = DtdProcessing.Ignore
            };
            var resolver = new XmlUrlResolver
            {
                Credentials = CredentialCache.DefaultCredentials
            };

            settings.XmlResolver = resolver;
            settings.NameTable = _nameTable;
            settings.ValidationFlags = XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationType = ValidationType.Schema;
            using (XmlReader reader = XmlReader.Create(fileName, settings))
            {
                _catalog.Load(reader);
                reader.Close();
            }

            if (!(_catalog.DocumentElement.NamespaceURI == XQTSNamespace && _catalog.DocumentElement.LocalName == "test-suite"))
            {
                throw new ArgumentException("Input file is not XQTS catalog.");
            }

            if (_catalog.DocumentElement.GetAttribute("version") != "1.0.2")
            {
                throw new NotSupportedException("Only version 1.0.2 is XQTS supported.");
            }

            _basePath = Path.GetDirectoryName(fileName);
            _sourceOffsetPath = _catalog.DocumentElement.GetAttribute("SourceOffsetPath");
            _queryOffsetPath = _catalog.DocumentElement.GetAttribute("XQueryQueryOffsetPath");
            _resultOffsetPath = _catalog.DocumentElement.GetAttribute("ResultOffsetPath");
            _queryFileExtension = _catalog.DocumentElement.GetAttribute("XQueryFileExtension");

            _sources = new Dictionary<string, string>();
            _module = new Dictionary<string, string>();
            _collection = new Dictionary<string, string[]>();
            _schema = new Dictionary<string, string[]>();

            foreach (XmlElement node in _catalog.SelectNodes("/ts:test-suite/ts:sources/ts:schema", _nsmgr))
            {
                string id = node.GetAttribute("ID");
                string targetNs = node.GetAttribute("uri");
                string schemaFileName = Path.Combine(_basePath, node.GetAttribute("FileName").Replace('/', '\\'));
                _schema.Add(id, new[] { targetNs, schemaFileName });
            }

            foreach (XmlElement node in _catalog.SelectNodes("/ts:test-suite/ts:sources/ts:source", _nsmgr))
            {
                string id = node.GetAttribute("ID");
                string sourceFileName = Path.Combine(_basePath, node.GetAttribute("FileName").Replace('/', '\\'));
                _sources.Add(id, sourceFileName);
            }

            foreach (XmlElement node in _catalog.SelectNodes("/ts:test-suite/ts:sources/ts:collection", _nsmgr))
            {
                string id = node.GetAttribute("ID");
                XmlNodeList nodes = node.SelectNodes("ts:input-document", _nsmgr);
                String[] items = new String[nodes.Count];
                int k = 0;
                foreach (XmlElement curr in nodes)
                {
                    items[k++] = curr.InnerText;
                }
                _collection.Add(id, items);
            }

            foreach (XmlElement node in _catalog.SelectNodes("/ts:test-suite/ts:sources/ts:module", _nsmgr))
            {
                string id = node.GetAttribute("ID");
                string moduleFileName = Path.Combine(_basePath, node.GetAttribute("FileName").Replace('/', '\\') + _queryFileExtension);
                _module.Add(id, moduleFileName);
            }

            //treeView1.Nodes.Clear();
            //treeView1.BeginUpdate();
            //TreeNode rootNode = new TreeNode("Test-suite", 0, 0);
            //treeView1.Nodes.Add(rootNode);
            //ReadTestTree(_catalog.DocumentElement, rootNode);
            //treeView1.EndUpdate();
            //rootNode.Expand();
        }
    }
}