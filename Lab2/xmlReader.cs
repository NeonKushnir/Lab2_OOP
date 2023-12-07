using System.Xml.Linq;

using models;

public class XmlReader

{
    public List<Department> LoadXmlData(string filePath)
    {
        var departments = new List<Department>();

        var xDocument = XDocument.Load(filePath);
        foreach (var deptElement in xDocument.Descendants("department"))
        {
            var department = new Department
            {
                Name = deptElement.Attribute("name")?.Value,
                Location = deptElement.Attribute("location")?.Value,
                Id = deptElement.Attribute("id")?.Value,
                Courses = new List<Course>()
            };

            foreach (var courseElement in deptElement.Elements("course"))
            {
                var course = new Course
                {
                    Name = courseElement.Attribute("name")?.Value,
                    Classes = new List<Class>()
                };

                foreach (var classElement in courseElement.Elements("class"))
                {
                    var classItem = new Class
                    {
                        Type = classElement.Attribute("type")?.Value,
                        Time = classElement.Attribute("time")?.Value,
                        Leader = new Leader
                        {
                            Name = classElement.Element("leader")?.Attribute("name")?.Value,
                            Phone = classElement.Element("leader")?.Attribute("phone")?.Value
                        },
                        Instructor = classElement.Element("instructor")?.Attribute("name")?.Value
                    };

                    course.Classes.Add(classItem);
                }

                department.Courses.Add(course);
            }

            departments.Add(department);
        }

        return departments;
    }

}