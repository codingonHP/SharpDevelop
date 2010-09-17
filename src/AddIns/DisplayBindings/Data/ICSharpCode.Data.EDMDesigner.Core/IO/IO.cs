﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.IO
{
    public class IO
    {
        #region Namespace declarations

        protected static XNamespace edmxNamespace = "http://schemas.microsoft.com/ado/2008/10/edmx";
        protected static XNamespace ssdlNamespace = "http://schemas.microsoft.com/ado/2009/02/edm/ssdl";
        protected static XNamespace storeNamespace = "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator";

        protected static XNamespace csdlNamespace = "http://schemas.microsoft.com/ado/2008/09/edm";
        protected static XNamespace csdlCodeGenerationNamespace = "http://schemas.microsoft.com/ado/2006/04/codegeneration";
        protected static XNamespace csdlAnnotationNamespace = "http://schemas.microsoft.com/ado/2009/02/edm/annotation";

        protected static XNamespace mslNamespace = "http://schemas.microsoft.com/ado/2008/09/mapping/cs";
        
        #endregion

        #region Helper functions

        protected static string GetName(string fullName)
        {
            return fullName.Substring(fullName.LastIndexOf(".") + 1);
        }

        protected static void SetStringValueFromAttribute(XElement element, string attributeName, Action<string> setAction)
        {
            SetStringValueFromAttribute(element, attributeName, string.Empty, setAction);
        }

        protected static void SetStringValueFromAttribute(XElement element, string attributeName, string ns, Action<string> setAction)
        {
            var attribute = element.Attribute(XName.Get(attributeName, ns));
            if (attribute != null)
                setAction(attribute.Value);
        }

        protected static void SetBoolValueFromAttribute(XElement element, string attributeName, Action<bool> setAction)
        {
            SetStringValueFromAttribute(element, attributeName,
                value =>
                {
                    switch (value)
                    {
                        case "0":
                        case "false":
                        case "False":
                            setAction(false);
                            break;
                        case "1":
                        case "true":
                        case "True":
                            setAction(true);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                });
        }

        protected static void SetIntValueFromAttribute(XElement element, string attributeName, Action<int> setAction)
        {
            SetStringValueFromAttribute(element, attributeName,
                value =>
                {
                    if (value != "Max")
                        setAction(int.Parse(value));
                });
        }

        protected static void SetEnumValueFromAttribute<T>(XElement element, string attribute, Action<T> setAction)
        {
            SetEnumValueFromAttribute<T>(element, attribute, string.Empty, setAction);
        }

        protected static void SetEnumValueFromAttribute<T>(XElement element, string attribute, string ns, Action<T> setAction)
        {
            SetStringValueFromAttribute(element, attribute, ns,
                enumName =>
                {
                    T[] values = Enum.GetValues(typeof(T)).Cast<object>().Where(v => v.ToString() == enumName).Select(v => (T)v).Take(1).ToArray();
                    if (values.Length == 0)
                        throw new NotImplementedException();
                    setAction(values[0]);
                });
        }

        protected static void SetCardinalityValueFromAttribute(XElement element, Action<Cardinality> setAction)
        {
            SetStringValueFromAttribute(element, "Multiplicity", multiplicity => setAction(CardinalityStringConverter.CardinalityFromString(multiplicity)));
        }

        protected static void SetStringValueFromElement(XElement element, string elementName, string ns, Action<string> setAction)
        {
            var subElement = element.Element(XName.Get(elementName, ns));
            if (subElement != null)
                setAction(subElement.Value);
        }

        #endregion
    }
}
