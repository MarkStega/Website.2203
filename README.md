# Dioptra Website

[![GitHub issues](https://img.shields.io/github/issues/Dioptra/Website.2203?logo=github&style=flat-square)](https://github.com/Dioptra/Website.2203/issues)
[![GitHub forks](https://img.shields.io/github/forks/Dioptra/Website.2203?logo=github&style=flat-square)](https://github.com/Dioptra/Website.2203/network/members)
[![GitHub stars](https://img.shields.io/github/stars/Dioptra/Website.2203?logo=github&style=flat-square)](https://github.com/Dioptra/Website.2203/stargazers)
[![GitHub stars](https://img.shields.io/github/watchers/Dioptra/Website.2203?logo=github&style=flat-square)](https://github.com/Dioptra/Website.2203/watchers)

[![GithubActionsMainPublish](https://img.shields.io/github/workflow/status/Dioptra/Website.2203/GithubActionsRelease?label=actions%20release&logo=github&style=flat-square)](https://github.com/Dioptra/Website.2203/actions?query=workflow%3AGithubActionsRelease)
[![GithubActionsDevelop](https://img.shields.io/github/workflow/status/Dioptra/Website.2203/GithubActionsWIP?label=actions%20wip&logo=github&style=flat-square)](https://github.com/Dioptra/Website.2203/actions?query=workflow%3AGithubActionsWIP)

Supplied with a [modified MIT license](https://github.com/Dioptra/Website.2203/blob/main/LICENSE.md). You are restricted from copying website written and visual content
as defined in the license file, however all other software is on an MIT equivalent basis.

Many thanks to [@MarkStega](https://github.com/MarkStega) of [Optimium Health](https://www.optimiumhealth.com/) for contributions to this project and for collaborating with
[@SimonZiegler](https://github.com/simonziegler) of [Dioptra](https://dioptra.tech) on [Material.Blazor](https://github.com/Material-Blazor) since 2020.

<br />

## About The Project
This is the Blazor project (both Blazor Server and Blazor WebAssembly) for the [public website of Dioptra Limited](https://dioptra.tech). This project is open sourced to
provide the developer community with an opinionated Blazor project reference.

The project makes use of all of the [Material.Blazor nuget packages](https://github.com/Material-Blazor) that are jointly copyright to Dioptra Limited and Optimium Health:

- [Material.Blazor](https://github.com/Material-Blazor/Material.Blazor) is a Razor Class Library integrating Google's Material Design 2 to Blazor.
- [CompressedStaticFiles.AspNetCore](https://github.com/Material-Blazor/CompressedStaticFiles.AspNetCore) serves pre-compressed GZip and Brotli files for ASP.NET projects.
- [GoogleAnalytics.Blazor](https://github.com/Material-Blazor/GoogleAnalytics.Blazor) gives access to Google Analytics for Blazor projects.
- [HttpSecurity.AspNet](https://github.com/Material-Blazor/HttpSecurity.AspNet) manages HTTP security headers include Content Security Policies for ASP.NET projects.

The project does not have detailed documentation, and you are welcome to pick through the repo for inspriation. We're open to answering questions or receving comments about
the technology employed here, however don't expect a swift response.

Enjoy!

<br />

## Notes

### Blazor Dual Mode

The `Website.Server` project can be deployed as either a Server Hosted Blazor WebAssembly app or a Blazor Server app. This dual mode approach is switched by using either the 
`DEBUG_WEBASSEMBLY` or the `DEBUG_SERVER` configurations, or likewise for release the `RELEASE_WEBASSEMBLY` or the `RELEASE_SERVER` configurations.

### Security

We aim to deploy this website with reasonably strong security, making use of HTTP headers, Content Security Policies and either integrity hashes or nonces for all linked
styles and scripts. This stance is informed by recent penetration testing performed on Dioptra's commercial web app, Vectis.

### Proprietary Information

This is Dioptra's public facing website and expresses how Dioptra chooses to represent itself. All written and visual content is proprietary, subject to copyright and may
not be reproduced. Images used by this website are subject to third party licensing and have been paid for by Dioptra.
