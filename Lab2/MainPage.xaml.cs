using System;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using models;

namespace Lab2
{
    public partial class MainPage : ContentPage
    {
        private List<Course> _courses;
        private List<Department> _departments;

        public MainPage()
        {
            InitializeComponent();
            LoadXmlData();
            InitializeCoursePicker();
            ClassPicker.IsEnabled = false;
        }

        private void LoadXmlData()
        {
            var xmlReader = new XmlReader();
            _departments = xmlReader.LoadXmlData(GlobalSettings.XmlFilePath);
            _courses = _departments.SelectMany(d => d.Courses).Distinct().ToList();
        }

        private void InitializeCoursePicker()
        {
            CoursePicker.ItemsSource = _courses.Select(c => c.Name).Distinct().ToList();
        }

        private void OnCourseSelected(object sender, EventArgs e)
        {
            if (CoursePicker.SelectedIndex == -1)
            {
                DepartmentPicker.ItemsSource = null;
                ClassPicker.ItemsSource = null;
                ClassPicker.IsEnabled = false;
                return;
            }

            var selectedCourseName = CoursePicker.SelectedItem.ToString();
            var relevantDepartments = _departments
                .Where(d => d.Courses.Any(c => c.Name == selectedCourseName))
                .ToList();

            DepartmentPicker.ItemsSource = relevantDepartments.Select(d => d.Name).ToList();
            DepartmentPicker.SelectedIndex = -1;
            ClassPicker.ItemsSource = null;
            ClassPicker.IsEnabled = false;

        }

        private void OnDepartmentSelected(object sender, EventArgs e)
        {
            if (DepartmentPicker.SelectedIndex == -1)
            {
                ClassPicker.ItemsSource = null;
                ClassPicker.IsEnabled = false;
                return;
            }

            var selectedDepartmentName = DepartmentPicker.SelectedItem.ToString();
            var selectedCourseName = CoursePicker.SelectedItem.ToString();

            var selectedDepartment = _departments.FirstOrDefault(d => d.Name == selectedDepartmentName);
            var classes = selectedDepartment?.Courses.FirstOrDefault(c => c.Name == selectedCourseName)?.Classes;

            if (classes != null && classes.Any())
            {
                ClassPicker.ItemsSource = classes.Select(cl => cl.Type).ToList();
                ClassPicker.IsEnabled = true;
                ClassPicker.SelectedIndex = -1;
            }
            else
            {
                ClassPicker.ItemsSource = null;
                ClassPicker.IsEnabled = false;
            }
        }

        private void OnClassSelected(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex == -1)
            {
                HideClassDetails();
                return;
            }

            var selectedDepartmentName = DepartmentPicker.SelectedItem.ToString();
            var selectedCourseName = CoursePicker.SelectedItem.ToString();
            var selectedClassName = ClassPicker.SelectedItem.ToString();

            var selectedDepartment = _departments.FirstOrDefault(d => d.Name == selectedDepartmentName);
            var selectedClass = selectedDepartment?.Courses.FirstOrDefault(c => c.Name == selectedCourseName)
                                                       ?.Classes.FirstOrDefault(cl => cl.Type == selectedClassName);

            if (selectedClass != null)
            {
                DisplayClassInfo(selectedClass);
                ShowClassDetails();
            }
            else
            {
                HideClassDetails();
            }
        }
        private void DisplayClassInfo(Class selectedClass)
        {
            TypeLabel.Text = $"Тип: {selectedClass.Type}";
            TimeLabel.Text = $"Час: {selectedClass.Time}";
            LeaderNameLabel.Text = $"Лідер: {selectedClass.Leader.Name}";
            LeaderPhoneLabel.Text = $"Телефон лідера: {selectedClass.Leader.Phone}";
            InstructorLabel.Text = $"Інструктор: {selectedClass.Instructor}";
        }

        private void ShowClassDetails()
        {
            DetailsLabel.IsVisible = true;
            TypeLabel.IsVisible = true;
            TimeLabel.IsVisible = true;
            LeaderNameLabel.IsVisible = true;
            LeaderPhoneLabel.IsVisible = true;
            InstructorLabel.IsVisible = true;
            ContactInfoLabel.IsVisible = true;
        }

        private void HideClassDetails()
        {
            DetailsLabel.IsVisible = false;
            TypeLabel.IsVisible = false;
            TimeLabel.IsVisible = false;
            LeaderNameLabel.IsVisible = false;
            LeaderPhoneLabel.IsVisible = false;
            InstructorLabel.IsVisible = false;
            ContactInfoLabel.IsVisible = false;
        }

        private void OnSaveHtmlButtonClicked(object sender, EventArgs e)
        {
            switch (StrategyPicker.SelectedItem)
            {
                case "SAX":
                    SetStrategy(new SaxXmlStrategy());
                    break;
                case "DOM":
                    SetStrategy(new DomXmlStrategy());
                    break;
                case "LINQ":
                    SetStrategy(new LinqToXmlStrategy());
                    break;
            }

            ProcessXmlAndSaveReport();
        }

        private IXmlProcessingStrategy _currentStrategy;

        private void SetStrategy(IXmlProcessingStrategy strategy)
        {
            _currentStrategy = strategy;
        }

        private void ProcessXmlAndSaveReport()
        {
            string xmlFilePath = GlobalSettings.XmlFilePath;
            if (File.Exists(xmlFilePath))
            {
                string xmlData = File.ReadAllText(xmlFilePath);
                XDocument xDoc = XDocument.Parse(xmlData);

                string htmlContent = _currentStrategy.ProcessXml(xDoc, SearchEntry.Text);
                SaveHtmlReport(htmlContent);
                OpenHtmlReport(htmlContent);
            }
            else
            {
                Console.WriteLine("XML file not found.");
            }
        }

        private void SaveHtmlReport(string htmlContent)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string htmlFilePath = Path.Combine(desktopPath, "CoursesReport.html");
            File.WriteAllText(htmlFilePath, htmlContent);
        }

        private void OpenHtmlReport(string htmlContent)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string htmlFilePath = Path.Combine(desktopPath, "CoursesReport.html");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(htmlFilePath) { UseShellExecute = true });
        }

        public string ProcessXml(XDocument document)
        {
            var html = new StringBuilder();
            html.Append("<html><body>");
            html.Append("<table border='1'>");
            html.Append("<tr><th>Course</th><th>Type</th><th>Time</th><th>Leader</th><th>Instructor</th></tr>");

            foreach (var department in document.Descendants("department"))
            {
                foreach (var course in department.Descendants("course"))
                {
                    foreach (var classElement in course.Descendants("class"))
                    {
                        html.Append("<tr>");
                        html.AppendFormat("<td>{0}</td>", course.Attribute("name")?.Value);
                        html.AppendFormat("<td>{0}</td>", classElement.Attribute("type")?.Value);
                        html.AppendFormat("<td>{0}</td>", classElement.Attribute("time")?.Value);

                        var leader = classElement.Element("leader");
                        var instructor = classElement.Element("instructor");

                        html.AppendFormat("<td>{0} ({1})</td>", leader.Attribute("name")?.Value, leader.Attribute("phone")?.Value);
                        html.AppendFormat("<td>{0}</td>", instructor.Attribute("name")?.Value);

                        html.Append("</tr>");
                    }
                }
            }

            html.Append("</table>");
            html.Append("</body></html>");

            return html.ToString();
        }
        private void OnClearButtonClicked(object sender, EventArgs e)
        {
            SearchEntry.Text = string.Empty;
            CoursePicker.SelectedIndex = -1;
            DepartmentPicker.SelectedIndex = -1;
            ClassPicker.SelectedIndex = -1;
            StrategyPicker.SelectedIndex = -1;

            TypeLabel.IsVisible = false;
            TimeLabel.IsVisible = false;
            LeaderNameLabel.IsVisible = false;
            LeaderPhoneLabel.IsVisible = false;
            InstructorLabel.IsVisible = false;
            ContactInfoLabel.IsVisible = false;
        }
        private void OnSearchButtonClicked(object sender, EventArgs e)
        {
            var keyword = SearchEntry.Text.Trim();
            var htmlContent = GenerateHtmlForKeyword(keyword.ToLowerInvariant());
            SearchResultsWebView.Source = new HtmlWebViewSource { Html = htmlContent };
        }

        private string GenerateHtmlForKeyword(string keyword)
        {
            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<html><head></head><body>");
            htmlBuilder.Append("<h1>Результати пошуку</h1>");

            // Передбачається, що _departments - це вже завантажений список відділів і курсів
            foreach (var department in _departments)
            {
                bool departmentAdded = false;

                foreach (var course in department.Courses)
                {
                    foreach (var classInfo in course.Classes)
                    {
                        if (classInfo.Type.ToLowerInvariant().Contains(keyword) ||
                            (classInfo.Leader != null && classInfo.Leader.Name.ToLowerInvariant().Contains(keyword)) ||
                            classInfo.Instructor.ToLowerInvariant().Contains(keyword))
                        {
                            if (!departmentAdded)
                            {
                                htmlBuilder.AppendFormat("<h2>{0} - {1}</h2>", department.Name, department.Location);
                                departmentAdded = true;
                            }
                            htmlBuilder.AppendFormat("<h3>{0}</h3>", course.Name);
                            htmlBuilder.Append("<p>");
                            htmlBuilder.AppendFormat("Тип: {0}<br>", classInfo.Type);
                            htmlBuilder.AppendFormat("Час: {0}<br>", classInfo.Time);
                            if (classInfo.Leader != null)
                            {
                                htmlBuilder.AppendFormat("Лідер: {0} ({1})<br>", classInfo.Leader.Name, classInfo.Leader.Phone);
                            }
                            htmlBuilder.AppendFormat("Інструктор: {0}<br>", classInfo.Instructor);
                            htmlBuilder.Append("</p>");
                        }
                    }
                }
            }

            htmlBuilder.Append("</body></html>");
            return htmlBuilder.ToString();
        }

        private async void OnExitClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Exit", "Ти хочеш втікти?", "Так", "Ні");
            if (answer)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }




    }
    }

