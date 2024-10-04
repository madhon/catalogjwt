// ReSharper disable RedundantUsingDirective.Global
#pragma warning disable IDE0065

global using Catalog.Auth;
global using Catalog.Auth.Model;
global using Catalog.Auth.Infrastructure;
global using Catalog.Auth.Extensions;
global using Catalog.Auth.Services;
global using Catalog.ServiceDefaults;

global using FluentValidation;
global using ErrorOr;

global using Asp.Versioning;
global using Asp.Versioning.Builder;
global using Swashbuckle.AspNetCore.SwaggerGen;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Logging;

global using System.Text;
global using System.Security.Claims;
global using System.Reflection;
global using System.Text.Json;
global using System.Text.Json.Serialization;

#pragma warning restore IDE0065
// ReSharper restore RedundantUsingDirective.Global