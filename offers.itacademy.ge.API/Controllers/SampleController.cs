// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace offers.itacademy.ge.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion(1)]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Simulate some async work
            await Task.Delay(1000).ConfigureAwait(false);
            return Ok("Hello from SampleController! this method is deprecated and will be removed soon.");
        }
    }
}
