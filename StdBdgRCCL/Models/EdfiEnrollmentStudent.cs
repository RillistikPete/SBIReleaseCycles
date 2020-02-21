using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public partial class EdfiEnrollmentStudent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("studentUniqueId")]
        public string StudentUniqueId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("middleName")]
        public string MiddleName { get; set; } = "";

        [JsonProperty("lastSurname")]
        public string LastSurname { get; set; }

        [JsonProperty("birthDate")]
        public DateTime? BirthDate { get; set; }

        [JsonProperty("schools")]
        public List<School> Schools { get; set; }

        [JsonProperty("educationOrganizations")]
        public List<EducationOrganization> EducationOrganizations { get; set; }
    }

    public partial class School
    {
        [JsonProperty("entryGradeLevelDescriptor")]
        public string EntryGradeLevelDescriptor { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("schoolId")]
        public long SchoolId { get; set; }
    }

    public partial class EducationOrganization
    {
        [JsonProperty("educationOrganizationId")]
        public long EducationOrganizationId { get; set; }

        [JsonProperty("sexDescriptor")]
        public string SexDescriptor { get; set; }

        [JsonProperty("hispanicLatinoEthnicity")]
        public bool HispanicLatinoEthnicity { get; set; }

        [JsonProperty("addresses")]
        public List<Address> Addresses { get; set; }

        [JsonProperty("electronicMails")]
        public List<ElectronicMail> ElectronicMails { get; set; }

        [JsonProperty("identificationCodes")]
        public List<IdentificationCode> IdentificationCodes { get; set; }

        [JsonProperty("languages")]
        public List<Language> Languages { get; set; }

        [JsonProperty("races")]
        public List<Race> Races { get; set; }

        [JsonProperty("telephones")]
        public List<Telephone> Telephones { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("addressType")]
        public string AddressType { get; set; }

        [JsonProperty("addressTypeDescriptor")]
        public string AddressTypeDescriptor { get; set; }

        [JsonProperty("streetNumberName")]
        public string StreetNumberName { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("stateAbbreviationType")]
        public string StateAbbreviationType { get; set; }

        [JsonProperty("stateAbbreviationDescriptor")]
        public string StateAbbreviationDescriptor { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("nameOfCountry")]
        public string NameOfCounty { get; set; }
    }

    public partial class ElectronicMail
    {
        [JsonProperty("electronicMailTypeDescriptor")]
        public string ElectronicMailTypeDescriptor { get; set; }

        [JsonProperty("electronicMailType")]
        public string ElectronicMailType { get; set; }

        [JsonProperty("electronicMailAddress")]
        public string ElectronicMailAddress { get; set; }

        [JsonProperty("primaryEmailAddressIndicator")]
        public bool PrimaryEmailAddressIndicator { get; set; }
    }

    public partial class Language
    {
        [JsonProperty("languageType")]
        public string LanguageType { get; set; }
    }

    public partial class Race
    {
        [JsonProperty("raceDescriptor")]
        public string RaceDescriptor { get; set; }

        [JsonProperty("raceType")]
        public string RaceType { get; set; }
    }

    public partial class Telephone
    {
        [JsonProperty("telephoneNumberTypeDescriptor")]
        public string TelephoneNumberTypeDescriptor { get; set; }

        [JsonProperty("telephoneNumber")]
        public string TelephoneNumber { get; set; }

        [JsonProperty("orderOfPriority")]
        public long OrderOfPriority { get; set; }

        [JsonProperty("textMessageCapabilityIndicator")]
        public bool TextMessageCapabilityIndicator { get; set; }
    }

    public partial class IdentificationCode
    {
        [JsonProperty("studentIdentificationSystemDescriptor")]
        public string StudentIdentificationSystemDescriptor { get; set; }
        [JsonProperty("asigningOrganizationIdentificationCode")]
        public string AssigningOrganizationIdentificationCode { get; set; }
        [JsonProperty("identificationCode")]
        public string IDCode { get; set; }
    }
}
