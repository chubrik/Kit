# [![Kit project](https://raw.githubusercontent.com/chubrik/Kit/main/icon.png)](#) Kit
[![NuGet package](https://img.shields.io/nuget/v/Chubrik.Kit)](https://www.nuget.org/packages/Chubrik.Kit/)
[![MIT license](https://img.shields.io/github/license/chubrik/Kit)](https://github.com/chubrik/Kit/blob/main/LICENSE)

Small engine for various projects. Has advanced diagnostics and the most useful features.
<br><br>

## Usage
```csharp
using Chubrik.Kit;

void Main(string[] args) {
  Kit.Setup(...); // optional
  Kit.Execute(MyApp);
}
async Task MyApp(CancellationToken ct) {
  // sync or async method
}
```
<br>

## <a name="license"></a>License
The Kit is licensed under the [MIT license](https://github.com/chubrik/Kit/blob/main/LICENSE).
<br><br>
