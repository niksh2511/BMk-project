﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RxWeb.Core.AspNetCore.Binder;
using RxWeb.Core.AspNetCore.Extensions;

namespace RxWeb.Core.AspNetCore
{
    public abstract class BaseCoreDomainController<T,FromQuery> : ControllerBase where T : class where FromQuery : class
    {
        protected ICoreDomain<T,FromQuery> Domain { get; set; }

        public BaseCoreDomainController(ICoreDomain<T, FromQuery> domain)
        {
            this.Domain = domain;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> Get([ModelBinder(typeof(QueryParamsBinder))]JObject jObject) => Ok(await this.Domain.GetAsync(jObject.ToObject<FromQuery>()));


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> GetBy([ModelBinder(typeof(QueryParamsBinder))]JObject jObject) => Ok(await this.Domain.GetBy(jObject.ToObject<FromQuery>()));

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Post([FromBody]T entity)
        {
            var validations = this.Domain.AddValidation(entity);
            if (validations.Count() == 0)
            {
                await this.Domain.AddAsync(entity);
                return Ok("Success");
            }
            return BadRequest(validations);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Put(int id, [FromBody]T entity)
        {
            var validations = this.Domain.UpdateValidation(entity);
            if (validations.Count() == 0)
            {
                await this.Domain.UpdateAsync(entity);
                return NoContent();
            }
            return BadRequest(validations);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Delete([ModelBinder(typeof(QueryParamsBinder))]JObject jObject)
        {
            var model = jObject.ToObject<FromQuery>();
            var validations = this.Domain.DeleteValidation(model);
            if (validations.Count() == 0)
            {
                await this.Domain.DeleteAsync(model);
                return NoContent();
            }
            return BadRequest(validations);
        }
    }
}
