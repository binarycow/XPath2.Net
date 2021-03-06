﻿// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System.Xml.XPath;

namespace Wmhelp.XPath2.Iterator
{
    internal sealed class PrecedingNodeIterator : AxisNodeIterator
    {
        private readonly XPathNodeType kind;

        public PrecedingNodeIterator(XPath2Context context, object nodeTest, XPath2NodeIterator iter)
            : base(context, nodeTest, false, iter)
        {
            if (typeTest == null)
            {
                if (nameTest == null)
                    kind = XPathNodeType.All;
                else
                    kind = XPathNodeType.Element;
            }
            else
                kind = typeTest.GetNodeKind();
        }

        private PrecedingNodeIterator(PrecedingNodeIterator src)
        {
            AssignFrom(src);
            kind = src.kind;
        }

        public override XPath2NodeIterator Clone()
        {
            return new PrecedingNodeIterator(this);
        }

        private XPathNavigator nav = null;

        protected override XPathItem NextItem()
        {
            while (true)
            {
                if (!accept)
                {
                    if (!MoveNextIter())
                        return null;
                    nav = curr.Clone();
                    curr.MoveToRoot();
                }
                accept = curr.MoveToFollowing(kind, nav);
                if (accept)
                {
                    if (TestItem())
                    {
                        sequentialPosition++;
                        return curr;
                    }
                }
            }
        }
    }
}