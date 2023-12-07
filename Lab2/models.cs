namespace models;

public class Department
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string Id { get; set; }
    public List<Course> Courses { get; set; }
}
public class Course
{
    public string Name { get; set; }
    public List<Class> Classes { get; set; }
}

public class Leader
{
    public string Name { get; set; }
    public string Phone { get; set; }
}

public class Class
{
    public string Type { get; set; }
    public string Time { get; set; }
    public Leader Leader { get; set; }
    public string Instructor { get; set; }
    public string Phone { get; set; }

}