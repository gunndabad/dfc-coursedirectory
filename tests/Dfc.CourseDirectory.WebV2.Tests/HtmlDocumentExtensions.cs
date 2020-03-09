﻿using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Xunit;
using Xunit.Sdk;

namespace Dfc.CourseDirectory.WebV2.Tests
{
    public static class HtmlDocumentExtensions
    {
        public static void AssertHasError(this IHtmlDocument doc, string fieldName, string expectedMessage)
        {
            var errorElementId = $"{fieldName}-error";
            var errorElement = doc.GetElementById(errorElementId);

            if (errorElement == null)
            {
                throw new XunitException($"No error found for field '{fieldName}'.");
            }

            var vht = errorElement.GetElementsByTagName("span")[0];
            var errorMessage = errorElement.InnerHtml.Substring(vht.OuterHtml.Length);
            Assert.Equal(expectedMessage, errorMessage);
        }

        public static IElement GetElementWithLabel(this IHtmlDocument doc, string label)
        {
            var allLabels = doc.QuerySelectorAll("label");

            foreach (var l in allLabels)
            {
                if (l.TextContent.Trim() == label)
                {
                    return doc.GetElementById(l.GetAttribute("for"));
                }
            }

            return null;
        }
    }
}