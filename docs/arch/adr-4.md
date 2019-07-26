## ADR 4: Choosing a Framework
July 26, 2019

### Context
While being a novice to both C# and WPF, I thought it might be helpful to adopt a popular framework to make the development process easier and remove some of the unnecessary "gotchas" of writing in a new language. Using a framework might also quicken the pace of development and remove the need to write many helper classes.

### Decision
The Galaxy Zoo touch table app will not use a framework. Adopting a framework seems like a bit of a shortcut to learning the basics of C#, and I wanted to focus more on _how_ the language worked than simply getting something working. Also, a framework might cause a lot of overhead if only a few aspects of the framework are being used. Learning a new framework might be an overload when the main focus should be becoming comfortable with C# and .NET.

### Status
Declined

### Consequences
While some frameworks are well documented (MVVM Light), others are quickly becoming deprecated (Silverlight), so there is a worry that any framework selected could soon lose favor in the .NET community. By declining to use a framework, there is the potential of adding more dev time creating a messenger, view model base, and other components often found in a WPF framework.

_In Retrospect:_ After becoming more familiar with WPF, I wish I had chosen a framework (perhaps MVVM Light) to achieve some of my needs on this app. For example, although I created my own messenger in the app based on a Stack Overflow post, I often ran into issues with the messenger and I often found advice online for those using the MVVM Light messenger, which was of no use to me. I don't think MVVM Light would've been a huge load on the code, and it likely could've cleaned up the code base a bit. I can see the merits of declining a framework for those creating their first WPF app, but I don't think much is gained from going without.
