## ADR 3: Choosing the MVVM Pattern
July 25, 2019

### Context
Files should be structured in a way that makes the code base easy to navigate and intuitive when looking for certain components. Organization should be suitable to the language used (C#) and should be an approach accepted and well-documented by the wider community.

### Decision
The MVVM (Model-View-View Model) approach is widely used by the WPF (Windows Presentation Foundation) community. It's difficult to search the web for insight on building WPF applications without running into information about MVVM architecture. This seems to be the standard for WPF applications.

Application components should be divided into a Model, View, and View Model folder, with each folder containing the necessary items for displaying the UI and interpreting data on the app.

### Status
Accepted

### Consequences
It will be easier to find solutions to coding problems by accepting a widely-used design pattern. However, this doesn't necessarily solve the problem of how other items should be organized (lib, images, fonts, etc.).

_In Retrospect:_ The MVVM pattern was overall beneficial, but I was often confused how strictly I should adhere to the pattern. MVVM says each view should have an accompanying view model and model. However, with so many design elements to the app, it often felt unnecessary to have a data model tied to each view. What would the model be for a modal and how would that be different from the view model?  
