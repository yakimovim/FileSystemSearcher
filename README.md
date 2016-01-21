#Extended file system object searcher

This project contains class that asynchronously searches for file system objects using extended syntax of templates.

##Usage

###1\. Basic search:

```csharp
using EdlinSoftware.FileSystemSearcher;

...

var txtFiles = await new Searcher().FindAsync(@"c:\documents\*.txt"); 
```

###2\. Templates in path:

You can use '*' and '?' wildcards not only in the last part of template path, but in any place.

```csharp
var txtFiles = await new Searcher().FindAsync(@"c:\user\doc*\abs?.txt"); 
```

###3\. Search in all subdirectories:

You can use '**' wildcard to execute search inside all subdirectories of some directory.

```csharp
var txtFiles = await new Searcher().FindAsync(@"c:\user\**\doc\*.txt"); 
```

###4\. Types of paths:

With the searcher you can use absolute, network or relative paths. In case of relative paths they are considered relatively value of **BaseDirectory** property. Default value of this property is **Environment.CurrentDirectory**.

```csharp
var filesByAbsolutePath = await new Searcher().FindAsync(@"c:\user\**\doc\*.txt"); 
```
```csharp
var filesByNetworkPath = await new Searcher().FindAsync(@"\\computer\storage\images\**\*.jpg"); 
```
```csharp
var filesByRelativePath = await new Searcher().FindAsync(@".\bin\*.dll"); 
```
```csharp
var filesByRelativePath = await new Searcher{ BaseDirectory = @"c:\user\" }.FindAsync(@".\documents\*.txt"); 
```
