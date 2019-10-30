using EfCoreIntroductionExercise.Data.Result;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(DeleteProjectById(new SoftUniContext()));
        }

        //problem 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var result = context.Employees
                .Select(e => new
                {
                    Id = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    MiddleName = e.MiddleName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .OrderBy(i => i.Id)
                .ToList();

            StringBuilder resultString = new StringBuilder();

            foreach (var employee in result)
            {
                resultString.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return resultString.ToString();

        }

        //problem 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var result = context.Employees
                .Select(e => new
                {
                    Name = e.FirstName,
                    Salary = e.Salary
                })
                .Where(s => s.Salary > 50000)
                .OrderBy(n => n.Name)
                .ToList();


            StringBuilder resultString = new StringBuilder();

            foreach (var employee in result)
            {
                resultString.AppendLine($"{employee.Name} - {employee.Salary:f2}");
            }

            return resultString.ToString();

        }

        //problem 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var result = context
                .Employees
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Salary = e.Salary,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department.Name
                })
                .Where(d => d.DepartmentName == "Research and Development")
                .OrderBy(e => e.Salary).ThenByDescending(e => e.FirstName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in result)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:f2}");
            }

            return sb.ToString();
        }

        //problem 6
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var adress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(adress);

            context.SaveChanges();

            var employee = context.Employees
                .Where(e => e.LastName == "Nakov")
                .FirstOrDefault();


            employee.AddressId = adress.AddressId;

            context.SaveChanges();

            var result = context.Employees
                .Select(e => new
                {
                    AdressId = e.AddressId,
                    text = e.Address.AddressText
                })
                .OrderByDescending(e => e.AdressId)
                .Take(10)
                .ToList();


            StringBuilder sb = new StringBuilder();

            foreach (var item in result)
            {
                sb.AppendLine(item.text);
            }

            return sb.ToString();
        }

        //problem 7
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.EmployeesProjects
                .Any(p => p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    EmployeeName = e.FirstName + " " + e.LastName,
                    ManagerName = e.Manager.FirstName + " " + e.Manager.LastName,
                    Projects = e.EmployeesProjects
                    .Select(p => new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate,
                        EndDate = p.Project.EndDate
                    })
                    .ToList()
                })
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.EmployeeName} - Manager: {employee.ManagerName}");

                foreach (var project in employee.Projects)
                {
                    var startDate = project.StartDate
                        .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    var endDate = project.EndDate == null ?
                        "not finished" :
                        project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                    sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //problem 8
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var result = context
                .Addresses
                .Select(a => new
                {
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count
                })
                .OrderByDescending(e => e.EmployeesCount)
                .ThenBy(e => e.TownName)
                .ThenBy(e => e.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var adress in result)
            {
                sb.AppendLine($"{adress.AddressText}, {adress.TownName} - {adress.EmployeesCount} employees");
            }

            return sb.ToString();
        }

        //problem 9
        public static string GetEmployee147(SoftUniContext context)
        {
            var result = context
                .Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    JobTitle = e.JobTitle,
                    Project = e.EmployeesProjects
                    .Where(p => p.EmployeeId == e.EmployeeId)
                    .Select(p => new
                    {
                        Name = p.Project.Name
                    })
                    .OrderBy(p => p.Name)
                    .ToList()
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in result)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

                foreach (var p in e.Project)
                {
                    sb.AppendLine(p.Name);
                }
            }

            return sb.ToString();
        }

        //problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var result = context.Departments
                .Where(d => d.Employees.Count > 5)
                .Select(d => new
                {
                    Id = d.DepartmentId,
                    DepartmentName = d.Name,
                    ManagerName = d.Manager.FirstName + " " + d.Manager.LastName,
                    Employees = d.Employees
                    .Select(e => new
                    {
                        DepartmentId = e.DepartmentId,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        JobTitle = e.JobTitle
                    })
                    .Where(emp => emp.DepartmentId == d.DepartmentId)
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList()
                })
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.DepartmentName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var d in result)
            {
                sb.AppendLine($"{d.DepartmentName} – {d.ManagerName}");

                foreach (var e in d.Employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString();
        }

        //problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            int projectsCount = context
                .Projects.Count();

            var result = context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .OrderBy(p => p.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var project in result)
            {
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Description);
                var dateResult = project.StartDate.ToString("M/d/yyyy h:mm:ss tt",CultureInfo.InvariantCulture);
                sb.AppendLine(dateResult);
            }

            return sb.ToString();
        }

        //problem 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var allEmployees = context
                .Employees
                .Where(e => e.Department.Name == "Engineering" ||
                e.Department.Name == "Tool Design" ||
                e.Department.Name == "Marketing" ||
                e.Department.Name == "Information Services");

            foreach (var employee in allEmployees)
            {
                employee.Salary *= 1.12M;
            }

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            var employeesToPring = allEmployees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName);

            foreach (var employe in employeesToPring)
            {
                sb.AppendLine($"{employe.FirstName} {employe.LastName} (${employe.Salary:f2})");
            }

            return sb.ToString();
        }

        //problem 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var result = context
                .Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();


            StringBuilder sb = new StringBuilder();

            foreach (var e in result)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString();
        }

        //problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            var workingOnProject = context
                .EmployeesProjects
                .Where(e => e.ProjectId == 2)
                .ToList();

            foreach (var ep in workingOnProject)
            {
                context.EmployeesProjects.Remove(ep);
            }

            var currentProject = context
                .Projects
                .First(p => p.ProjectId == 2);

            context.Projects.Remove(currentProject);

            context.SaveChanges();

            var result = context
                .Projects
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var p in result)
            {
                sb.AppendLine(p.Name);
            }

            return sb.ToString().TrimEnd();
        }

        //problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            var seattle = context
                .Towns
                .First(t => t.Name == "Seattle");

            var addressesInTown = context
                .Addresses
                .Where(a => a.Town == seattle);

            var employeesToRemoveAdress = context
                .Employees
                .Where(e => addressesInTown.Contains(e.Address));

            foreach (var e in employeesToRemoveAdress)
            {
                e.AddressId = null;
            }

            foreach (var a in addressesInTown)
            {
                context.Addresses
                    .Remove(a);
            }

            int addressesCount = addressesInTown.Count();

            context.Towns.Remove(seattle);
            context.SaveChanges();

            return $"{addressesCount} addresses in Seattle were deleted";
        }

    }
}
