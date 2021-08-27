using Dangl.Identity.TestHost.SetupData;
using System;
using System.Collections.Generic;

namespace Dangl.OpenCDE.TestUtilities.TestData
{
    public static class Users
    {
        public static List<UserSetupDto> Values => new List<UserSetupDto>
        {
            User,
            AdminUser,
            NotKnownBeforeUser
        };

        /// <summary>
        /// These values should be present when the database is seeded
        /// </summary>
        public static List<UserSetupDto> SeedValues => new List<UserSetupDto>
        {
            User,
            AdminUser
        };

        public static UserSetupDto User => new UserSetupDto
        {
            Email = "cdeuser@dangl-it.com",
            Password = "P4ssw0rd!2020",
            Username = "CDE User",
            UserId = new Guid("a724f4df-5286-4dbc-b3d9-d22568bb38e6")
        };

        public static UserSetupDto AdminUser => new UserSetupDto
        {
            Email = "admin@dangl-it.com",
            Password = "P4ssw0rd!2020",
            Username = "Test Admin",
            UserId = new Guid("bba2b3a2-5751-4d40-b156-a661ca8c99e0"),
            Roles = new List<string> { Identity.Shared.DanglIdentityConstants.ADMIN_ROLE_NAME }
        };

        /// <summary>
        /// This user has access but is not yet known to the system, so he's
        /// excluded from the seed data but can still login via the Dangl.Identity account
        /// </summary>
        public static UserSetupDto NotKnownBeforeUser => new UserSetupDto
        {
            Email = "newuser@dangl-it.de",
            Password = "P4ssw0rd!2020",
            Username = "New CDE User",
            UserId = new Guid("74c5eb90-8d3f-4296-a88f-53a0d31bf57e")
        };
    }
}
