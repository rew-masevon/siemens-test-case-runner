using Siemens.Engineering;
using Siemens.Engineering.TestSuite;
using Siemens.Engineering.TestSuite.ApplicationTest;
using System;
using System.IO;
using System.Linq;

string projectFile = Path.GetFullPath(@"..\..\..\..\..\..\MyTestProject\MyTestProject\MyTestProject.ap18");

var tiaPortal = GetOrCreateTiaPortal(projectFile);
var project = GetProject(tiaPortal, projectFile);

var testSuiteService = project.GetService<TestSuiteService>() ?? throw new Exception("Cannot find TestSuiteService.");
var testCaseExecutor = testSuiteService.ApplicationTestGroup.GetService<TestCaseExecutor>();

// Call to .Run below will always result into a failing unit test,
// whereas running the same test in the IDE will succeed:
var result = testCaseExecutor.Run(testSuiteService.ApplicationTestGroup.TestCases);

PrintResultReport(result);

Console.ReadKey(); // Keep window alive...
static TiaPortal GetOrCreateTiaPortal(string projectFile)
    => TiaPortal
           .GetProcesses()
           .FirstOrDefault(t => t.ProjectPath is not null && t.ProjectPath.FullName.Equals(projectFile, StringComparison.OrdinalIgnoreCase))?
           .Attach()
       ?? new TiaPortal(TiaPortalMode.WithUserInterface);

static Project GetProject(TiaPortal portal, string projectFile)
    => portal.Projects.FirstOrDefault(t => t.Path.FullName.Equals(projectFile, StringComparison.OrdinalIgnoreCase))
       ?? portal.Projects.Open(new System.IO.FileInfo(projectFile));

static void PrintResultReport(TestResults results)
{
    Console.WriteLine($"Results: {results.ErrorCount} errors, {results.WarningCount} warnings, state: {results.State}.");
    Console.WriteLine("=============");

    foreach (var message in results.Messages)
    {
        Console.WriteLine($"{message.DateTime:O}: {message.State} {message.Description}");
    }

    Console.WriteLine("=============");
}