﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Athena.Core.Repositories;
using Athena.Core.Models;
using Athena.Exceptions;
using System.Net;
using Athena.Extensions;

namespace Athena.Controllers.api
{
    [Route("api/v1/[Controller]")]
    public class MeetingController : AthenaApiController
    {
        private readonly IMeetingRepository _meetings;

        public MeetingController (IMeetingRepository meetingsRepository) =>
            _meetings = meetingsRepository ?? throw new ArgumentNullException(nameof(meetingsRepository));

        [HttpPost]
        public async Task AddMeeting([FromBody] Meeting meeting) => await _meetings.AddAsync(meeting);

        [HttpGet("{id}")]
        public async Task<Meeting> GetMeeting(Guid id) =>
            (await _meetings.GetAsync(id)).NotFoundIfNull();

        [HttpPut("{id}")]
        public async Task EditMeeting(Guid id, [FromBody] Meeting meeting)
        {
            if (id != meeting.Id)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Tried to edit {id} but got a model for {meeting.Id}");
            }
            await _meetings.EditAsync(meeting);
        }

        [HttpDelete("{id}")]
        public async Task DeleteMeeting(Guid id)
        {
            var meeting = (await _meetings.GetAsync(id)).NotFoundIfNull();
            
            await _meetings.DeleteAsync(meeting);
        }
    }
}