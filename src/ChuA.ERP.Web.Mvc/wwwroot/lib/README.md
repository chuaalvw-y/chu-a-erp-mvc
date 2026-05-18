# Front-end libraries

This folder hosts the static client libraries referenced by `_Layout.cshtml`. The build
does **not** fail without them, but the UI will be unstyled and client-side validation
will not work until they are present.

Expected layout (drop-in or restore via libman):

```
lib/
├── bootstrap/
│   ├── css/bootstrap.min.css
│   └── js/bootstrap.bundle.min.js
├── jquery/
│   └── jquery.min.js
├── jquery-validation/
│   └── jquery.validate.min.js
└── jquery-validation-unobtrusive/
    └── jquery.validate.unobtrusive.min.js
```

## Quickest setup with libman

Create `libman.json` in the project root with:

```json
{
  "version": "1.0",
  "defaultProvider": "cdnjs",
  "libraries": [
    { "library": "bootstrap@5.3.3",                            "destination": "wwwroot/lib/bootstrap" },
    { "library": "jquery@3.7.1",                               "destination": "wwwroot/lib/jquery" },
    { "library": "jquery-validate@1.21.0",                     "destination": "wwwroot/lib/jquery-validation" },
    { "library": "jquery-validation-unobtrusive@4.0.0",        "destination": "wwwroot/lib/jquery-validation-unobtrusive" }
  ]
}
```

Then `libman restore`.

## Or pull from CDNJS manually

```powershell
$libRoot = "wwwroot/lib"
Invoke-WebRequest https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.3/css/bootstrap.min.css -OutFile "$libRoot/bootstrap/css/bootstrap.min.css"
Invoke-WebRequest https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.3/js/bootstrap.bundle.min.js -OutFile "$libRoot/bootstrap/js/bootstrap.bundle.min.js"
Invoke-WebRequest https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js -OutFile "$libRoot/jquery/jquery.min.js"
Invoke-WebRequest https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.21.0/jquery.validate.min.js -OutFile "$libRoot/jquery-validation/jquery.validate.min.js"
Invoke-WebRequest https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/4.0.0/jquery.validate.unobtrusive.min.js -OutFile "$libRoot/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"
```
