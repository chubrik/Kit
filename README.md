# [![Kit project](https://raw.githubusercontent.com/chubrik/Kit/master/icon.png)](#) Kit
[![NuGet](https://img.shields.io/nuget/v/Kit.svg)](https://www.nuget.org/packages/Kit/)
[![Build Status](https://travis-ci.org/chubrik/Kit.svg?branch=master)](https://travis-ci.org/chubrik/Kit)
[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/chubrik/Kit/blob/master/LICENSE)

The powerful engine for various C# projects.<br>Has advanced diagnostics and the most useful features.

## How to use
```c#
void Main(string[] args) {
  Kit.Setup(...); // optional
  Kit.Execute(MyApp);
}
async Task MyApp(CancellationToken ct) {
  // sync or async method
}
```
