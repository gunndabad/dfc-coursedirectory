﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.Services.Tests.Unit.Helpers
{
    public class SampleJsons
    {
        #region Successful Json Files

        public static string SuccessfulStandardFile()
        {
            return
                "[{\r\n  \"id\": \"00000000-0000-0000-0000-000000000000\",\r\n  \"standardCode\": 157,\r\n  \"version\": 1,\r\n  \"standardName\": \"Standard Name\",\r\n  \"standardSectorCode\": \"42\",\r\n  \"urlLink\": \"https://www.gov.uk/government/\",\r\n  \"notionalEndLevel\": \"4\",\r\n  \"otherBodyApprovalRequired\": null,\r\n  \"apprenticeshipType\": 0,\r\n  \"effectiveFrom\": \"2019-08-01T00:00:00\",\r\n  \"createdDateTimeUtc\": \"2019-08-22T14:32:53.983\",\r\n  \"modifiedDateTimeUtc\": \"2019-03-08T21:01:29.993\",\r\n  \"recordStatusId\": 1,\r\n  \"alreadyCreated\": false,\r\n  \"frameworkCode\": null,\r\n  \"progType\": null,\r\n  \"pathwayCode\": null,\r\n  \"pathwayName\": null,\r\n  \"nasTitle\": null,\r\n  \"effectiveTo\": \"0001-01-01T00:00:00\",\r\n  \"sectorSubjectAreaTier1\": null,\r\n  \"sectorSubjectAreaTier2\": null,\r\n  \"progTypeDesc\": null,\r\n  \"progTypeDesc2\": null\r\n}]";
        }
        public static string SuccessfulFrameworkFile()
        {
            return
                "[{\r\n  \"id\": \"00000000-0000-0000-0000-000000000000\",\r\n  \"standardCode\": null,\r\n  \"version\": null,\r\n  \"standardName\": \"Standard Name\",\r\n  \"standardSectorCode\": \"42\",\r\n  \"urlLink\": \"https://www.gov.uk/government/\",\r\n  \"notionalEndLevel\": \"4\",\r\n  \"otherBodyApprovalRequired\": null,\r\n  \"apprenticeshipType\": 0,\r\n  \"effectiveFrom\": \"2019-08-01T00:00:00\",\r\n  \"createdDateTimeUtc\": \"2019-08-22T14:32:53.983\",\r\n  \"modifiedDateTimeUtc\": \"2019-03-08T21:01:29.993\",\r\n  \"recordStatusId\": 1,\r\n  \"alreadyCreated\": false,\r\n  \"frameworkCode\": 3,\r\n  \"progType\": 4,\r\n  \"pathwayCode\": 5,\r\n  \"pathwayName\": null,\r\n  \"nasTitle\": null,\r\n  \"effectiveTo\": \"0001-01-01T00:00:00\",\r\n  \"sectorSubjectAreaTier1\": null,\r\n  \"sectorSubjectAreaTier2\": null,\r\n  \"progTypeDesc\": null,\r\n  \"progTypeDesc2\": null\r\n}]";
        }
        public static string SuccessfulVenueFile()
        {
           return "[{\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  \"VENUE_NAME\": \"Venue 1\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 123435,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}, {\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  \"VENUE_NAME\": \"Venue 2\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 123456,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}, {\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  \"VENUE_NAME\": \"Venue 2\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 112345,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}]";

        }
        public static string SuccessfulVenueFile_Individual_Venues()
        {
           return "[{\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  " +
                  "\"VENUE_NAME\": \"Venue 1\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 123435,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}, {\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  " +
                  "\"VENUE_NAME\": \"Venue 2\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 123456,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}, {\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  " +
                  "\"VENUE_NAME\": \"Venue 3\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 112345,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}]";

        }

        #endregion

        #region Unsuccessful files

        public static string EmptyFile()
        {
            return "[]";
        }
        public static string MultipleVenueFile()
        {
            return "[{\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  " +
                   "\"VENUE_NAME\": \"Venue 1\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 123435,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}, {\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  " +
                   "\"VENUE_NAME\": \"Venue 3\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 123456,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}, {\r\n  \"id\": \"ff60d867-76a5-429e-9285-65af0789192d\",\r\n  \"UKPRN\": 12345678,\r\n  \"PROVIDER_ID\": 123456,\r\n  \"VENUE_ID\": 1234567,\r\n  " +
                   "\"VENUE_NAME\": \"Venue 3\",\r\n  \"PROV_VENUE_ID\": \"ABC\",\r\n  \"PHONE\": \"\",\r\n  \"ADDRESS_1\": \"Address 1\",\r\n  \"ADDRESS_2\": \"Address 2\",\r\n  \"TOWN\": \"Town Name\",\r\n  \"COUNTY\": \"\",\r\n  \"POSTCODE\": \"ABC 123\",\r\n  \"EMAIL\": \"\",\r\n  \"WEBSITE\": \"\",\r\n  \"Status\": 1,\r\n  \"DateUpdated\": \"2019-07-15T14:36:50.4983406+00:00\",\r\n  \"UpdatedBy\": \"TestUser\",\r\n  \"Latitude\": \"1\",\r\n  \"Longitude\": 1,\r\n  \"LocationId\": 112345,\r\n  \"TribalLocationId\": null,\r\n  \"Telephone\": \"\",\r\n  \"Email\": \"\",\r\n  \"Website\": \"\"\r\n}]";

        }

        #endregion
    }
}
