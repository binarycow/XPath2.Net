﻿// Microsoft Public License (Ms-PL)
// See the file License.rtf or License.txt for the license details.

// Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
// All rights reserved.

using System.Xml.XPath;

namespace Wmhelp.XPath2.Iterator
{
    internal sealed class PrecedingSiblingNodeIterator : SequentialAxisNodeIterator
    {
        public PrecedingSiblingNodeIterator(XPath2Context context, object nodeTest, XPath2NodeIterator iter)
            : base(context, nodeTest, false, iter)
        {
        }

        private PrecedingSiblingNodeIterator(AxisNodeIterator src)
        {
            AssignFrom(src);
        }

        public override XPath2NodeIterator Clone()
        {
            return new PrecedingSiblingNodeIterator(this);
        }

        protected override bool MoveToFirst(XPathNavigator nav)
        {
            return nav.MoveToPrevious();
        }

        protected override bool MoveToNext(XPathNavigator nav)
        {
            return nav.MoveToPrevious();
        }
    }
}