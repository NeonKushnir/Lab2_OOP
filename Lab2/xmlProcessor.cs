using System;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Linq;
using models;  // Припускаючи, що це ваш простір імен для моделей

namespace Lab2
{
    public interface IXmlProcessingStrategy
    {
        string ProcessXml(XDocument document, string keywords);
    }

    public class LinqToXmlStrategy : IXmlProcessingStrategy
    {
        public string ProcessXml(XDocument document, string keywords)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body>");

            // Фільтруємо 'department'
            var departments = document.Descendants("department")
                .Where(dept => dept.Attributes().Any(attr => attr.Value.Contains(keywords))
                               || dept.Descendants().Any(el => el.Value.Contains(keywords)));
            foreach (var dept in departments)
            {
                sb.Append(dept);
            }

            // Фільтруємо 'course'
            var courses = document.Descendants("course")
                .Where(course => course.Attributes().Any(attr => attr.Value.Contains(keywords))
                                 || course.Descendants().Any(el => el.Value.Contains(keywords)));
            foreach (var course in courses)
            {
                sb.Append(course);
            }

            // Фільтруємо 'class'
            var classes = document.Descendants("class")
                .Where(cl => cl.Attributes().Any(attr => attr.Value.Contains(keywords))
                             || cl.Descendants().Any(el => el.Value.Contains(keywords)));
            foreach (var cl in classes)
            {
                sb.Append(cl);
            }

            // Фільтруємо 'leader'
            var leaders = document.Descendants("leader")
                .Where(leader => leader.Attributes().Any(attr => attr.Value.Contains(keywords)));
            foreach (var leader in leaders)
            {
                sb.Append(leader);
            }

            sb.Append("</body></html>");
            return sb.ToString();
        }
    }

    public class DomXmlStrategy : IXmlProcessingStrategy
    {
        public string ProcessXml(XDocument document, string keywords)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = document.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            var filteredNodes = xmlDocument.SelectNodes("//department[contains(.,'" + keywords + "')]");

            var sb = new StringBuilder();
            sb.Append("<html><body>");

            foreach (XmlNode node in filteredNodes)
            {
                sb.Append(node.OuterXml); // Ви можете тут змінити форматування під HTML
            }

            sb.Append("</body></html>");
            return sb.ToString();
        }
    }

    public class SaxXmlStrategy : IXmlProcessingStrategy
    {
        public string ProcessXml(XDocument document, string keywords)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body>");

            // Використовуємо LINQ to XML для імітації SAX-подібного читання.
            foreach (var element in document.Descendants("department"))
            {
                if (element.ToString().Contains(keywords))
                {
                    sb.Append(element); // Ви можете тут змінити форматування під HTML
                }
            }

            sb.Append("</body></html>");
            return sb.ToString();
        }
    }
}