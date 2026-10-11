# Lab 3 – Workshop RSVP (ASP.NET Core MVC)

A small MVC feature for RSVPing to workshops, built on the ASP.NET Core Web App (Model-View-Controller) template.

## Pages
| URL | Action | What it does |
|---|---|---|
| `/Workshops` | `Index` (GET) | Landing page describing the RSVP feature, with navigation buttons |
| `/Workshops/RsvpForm` | `RsvpForm` (GET) | Form to collect RSVP data |
| `/Workshops/Confirm` | `Confirm` (POST) | Saves the RSVP, sets `ViewData["Message"]`, returns `View(model)` |
| `/Workshops/Registrations` | `Registrations` (GET) | Table of all RSVPs (in-memory static list) |

## Structure
```
WorkshopRsvp/
├─ Controllers/
│  ├─ HomeController.cs          (template boilerplate)
│  └─ WorkshopsController.cs     (Lab 3)
├─ Models/
│  ├─ ErrorViewModel.cs
│  └─ Rsvp.cs                    (Lab 3)
├─ Views/
│  ├─ Home/                      (template boilerplate)
│  ├─ Shared/_Layout.cshtml      ("Workshops" menu link via Tag Helpers)
│  └─ Workshops/                 Index, RsvpForm, Confirm, Registrations
└─ wwwroot/
```

## Run locally
```
cd WorkshopRsvp
dotnet run
```
Or open `WorkshopRsvp.sln` in Visual Studio and press F5.

> Registrations are stored in a static list, so they reset whenever the app restarts.
