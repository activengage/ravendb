// -----------------------------------------------------------------------
//  <copyright file="AdminJsConsoleController.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Raven35.Database.JsConsole;
using Raven35.Database.Server.WebApi.Attributes;
using Raven35.Imports.Newtonsoft.Json.Linq;
using Raven35.Json.Linq;

namespace Raven35.Database.Server.Controllers.Admin
{
    [RoutePrefix("")]
    public class AdminJsConsoleController : BaseAdminDatabaseApiController
    {
        [HttpPost]
        [RavenRoute("admin/console/{*id}")]
        public async Task<HttpResponseMessage> Console(string id)
        {
            var database = await DatabasesLandlord.GetResourceInternal(id).ConfigureAwait(false);

            var script = await ReadJsonObjectAsync<AdminJsScript>().ConfigureAwait(false);

            var console = new AdminJsConsole(database);

            var result = console.ApplyScript(script);

            if (result.Type == JTokenType.Null)
            {
                result = new RavenJValue("OK");
            }
            return GetMessageWithObject(result);
        }
    }
}
