// ReSharper disable RedundantUsingDirective.Global
#pragma warning disable IDE0065

global using Catalog.Auth;
global using Catalog.Auth.Extensions;
global using Catalog.Auth.Model;
global using Catalog.Auth.ViewModel;
global using Catalog.Auth.Infrastructure;
global using Catalog.Auth.Services;

global using FluentValidation;
global using Ardalis.ApiEndpoints;
global using ErrorOr;
global using System.Net;
global using Asp.Versioning;
global using Swashbuckle.AspNetCore.Annotations;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Serilog;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.Extensions.Options;

global using System.Text;
global using System.Security.Cryptography;
global using System.Security.Claims;
global using System.Reflection;
#pragma warning restore IDE0065
// ReSharper restore RedundantUsingDirective.Global