﻿using Athena.Controllers.api;
using System;
using Athena.Core.Models;
using Athena.Exceptions;
using AutoFixture.Xunit2;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using Athena.Tests.Extensions;

namespace Athena.Tests.Controllers.Api
{
    public class OfferingCourseControllerTests : ControllerTest
    {

        private readonly OfferingCourseController _controller;

        public OfferingCourseControllerTests() => _controller = new OfferingCourseController(Offerings.Object, Courses.Object);


        [Theory, AutoData]
        public async Task GetOfferingsForCourse(Guid courseId , Course course)
        {
            Courses.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsAsync(course);

            await _controller.GetOfferingsForCourseAsync(courseId);

            Offerings.Verify(o => o.GetOfferingsForCourseAsync(course));
        }

        [Theory, AutoData]
        public async Task GetOfferingsForCourse_ThrowsforNullCourse(Guid id)
        {
            Courses.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsNullAsync();

            var ex = await Assert.ThrowsAsync<ApiException>(async () => await _controller.GetOfferingsForCourseAsync(id));

            Assert.Equal(HttpStatusCode.NotFound, ex.ResponseCode);
        }

        [Theory, AutoData]
        public async Task AddOffering_Valid(Guid courseId, Guid offeringId, Course course, Offering offering)
        {
            Courses.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsAsync(course);
            Offerings.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsAsync(offering);

            await _controller.AddOfferingAsync(courseId, offeringId);

            Courses.Verify(o => o.AddOfferingAsync(course, offering));
        }

        [Theory, AutoData]
        public async Task AddOffering_ThrowsforNullOffeirng(Guid courseId, Guid offeringId)
        {
            Courses.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsNullAsync();
            Offerings.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsNullAsync();

            var ex = await Assert.ThrowsAsync<ApiException>(async () => await _controller.AddOfferingAsync(courseId, offeringId));

            Assert.Equal(HttpStatusCode.NotFound, ex.ResponseCode);
        }

        [Theory, AutoData]
        public async Task RemoveOffering_Valid(Guid courseId, Guid offeringId , Course course, Offering offering)
        {
            Courses.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsAsync(course);
            Offerings.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsAsync(offering);

            await _controller.RemoveOfferingAsync(courseId, offeringId);

            Courses.Verify(o => o.RemoveOfferingAsync(course, offering));
        }

        [Theory, AutoData]
        public async Task RemoveOffering_ThrowsforNullMeeting(Guid courseId, Guid offeringId)
        {
            Courses.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsNullAsync();
            Offerings.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsNullAsync();

            var ex = await Assert.ThrowsAsync<ApiException>(async () => await _controller.RemoveOfferingAsync(courseId, offeringId));

            Assert.Equal(HttpStatusCode.NotFound, ex.ResponseCode);
        }
    }
}
