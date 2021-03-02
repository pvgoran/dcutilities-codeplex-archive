# dcutilities

Archive of DigitallyCreated Utilities (dcutilities) from https://archive.codeplex.com/?p=dcutilities.

I do not own this project, I only published it to keep it publicly available after upcoming shutdown of CodePlex Archive. The author/owner of the project is (apparently) Daniel Chambers, http://www.digitallycreated.net/Contact. See http://www.digitallycreated.net/Programs/DCUtilities for more information.

`dcutilities.zip` is the archive that I downloaded from archive.codeplex.com. It has a weird format with file names which have literal backslashes `\` in them, instead of being placed into directories. I "unpacked" these filenames to create a regular directory structure. There is the `.hg` directory, so the project must have used Mercurial as its source control system. Maybe I'll import the revision history from it some day.

_Original project readme/description:_

---

DigitallyCreated Utilities are some utilities and extensions that make working in .NET easier. It has classes to help work with LINQ, Entity Framework, ASP.NET MVC, Unity, concurrent programming and error reporting. We value documentation, so everything has a tutorial and XMLdoc.

# Project Description

DigitallyCreated Utilities are some utilities and extensions that make working in .NET easier. It has classes to help work with LINQ, Entity Framework, ASP.NET MVC, Unity, concurrent programming and error reporting. We value documentation, so everything has a tutorial and XMLdoc.

Currently, the most recent release version of DigitallyCreated Utilities has the following features (categorised by technology):

* **ASP.NET MVC and LINQ**
  * Sorting and paging of data in a table made easy by HtmlHelpers and LINQ extensions (see tutorial)
* **ASP.NET MVC**
  * HtmlHelpers
    * TempInfoBox - A temporary "action performed" box that displays to the user for 5 seconds then fades out (see tutorial)
    * CollapsibleFieldset - A fieldset that collapses and expands when you click the legend (see tutorial)
    * Gravatar - Renders an img tag for a Gravatar (see tutorial)
    * CheckboxStandard & BoolBinder - Renders a normal checkbox without MVC's normal hidden field (see tutorial)
    * EncodeAndInsertBrsAndLinks - Great for the display of user input, this will insert `<br/>`s for newlines and `<a>` tags for URLs and escape all HTML (see tutorial)
  * IncomingRequestRouteConstraint - Great for supporting old permalink URLs using ASP.NET routing (see tutorial)
  * Improved JsonResult - Replaces ASP.NET MVC's JsonResult with one that lets you specify JavaScriptConverters (see tutorial)
  * Permanently Redirect ActionResults - Redirect users with 301 (Moved Permanently) HTTP status codes (see tutorial)
  * Miscellaneous Route Helpers - For example, RouteToCurrentPage (see tutorial)
* **LINQ**
  * MatchUp & Federator LINQ methods - Great for doing diffs on sequences (see tutorial)
* **Entity Framework**
  * CompiledQueryReplicator - Manage your compiled queries such that you don't accidentally bake in the wrong MergeOption and create a difficult to discover bug (see tutorial)
  * Miscellaneous Entity Framework Utilities - For example, ClearNonScalarProperties and Setting Entity Properties to Modified State (see tutorial)
* **Error Reporting**
  * Easily wire up some simple classes and have your application email you detailed exception and error object dumps (see tutorial)
* **Concurrent Programming**
  * Semaphore/FifoSemaphore & Mutex/FifoMutex (see tutorial)
  * ReaderWriterLock (see tutorial)
  * LinkedListChannel - A thread-safe queue (see tutorial)
  * ActiveObject - Easily inherit from ActiveObject to separately thread your class (see tutorial)
* **Unity & WCF**
  * WCF Client Injection Extension - Easily use dependency injection to transparently inject WCF clients using Unity (see tutorial)
* **Miscellaneous Base Class Library Utilities**
  * SafeUsingBlock and DisposableWrapper - Work with IDisposables in an easier fashion and avoid the bug where using blocks can silently swallow exceptions (see tutorial)
  * Time Utilities - For example, TimeSpan To Ago String, TzId -> Windows TimeZoneInfo (see tutorial)
  * Miscellaneous Utilities - Collection Add/RemoveAll, Base64StreamReader, AggregateException (see tutorial)

# Project Technologies

DC Utilities is written with and is for the following technologies:

* .NET 3.5 SP1, C# 3.0, VS2008
* Entity Framework v1 (.NET 3.5 SP1)
* ASP.NET MVC v1
* Unity Application Block 1.2
* jQuery 1.3.2

# Project Values

* To provide useful utilities and extensions to basic .NET functionality that saves developers' time and makes their code more elegant
* To provide fully XML-documented source code (nothing is more annoying than undocumented code)
* To back up the source code documentation with useful tutorial articles that help developers use this project
