# Front-end libraries

This folder hosts the static client libraries referenced by `_Layout.cshtml`.
The restored library files are committed with the app so publish output does not
depend on a manual restore step.

Expected layout after running `libman restore` from `src/ChuA.ERP.Web.Mvc`:

```
lib/
|-- bootstrap/
|   |-- css/bootstrap.min.css
|   `-- js/bootstrap.bundle.min.js
|-- jquery/
|   `-- jquery.min.js
|-- jquery-validation/
|   `-- jquery.validate.min.js
`-- jquery-validation-unobtrusive/
    `-- jquery.validate.unobtrusive.min.js
```

## Setup with LibMan

The project includes `libman.json`. Install the LibMan CLI if needed, then run
`libman restore` from the MVC project root to refresh the committed files.
