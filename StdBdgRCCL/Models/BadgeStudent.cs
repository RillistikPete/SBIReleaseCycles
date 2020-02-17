using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class BadgeStudent
    {
        [JsonProperty("bmsid")]
        public string StudentId { get; set; }

        private string _lastName = "";

        [JsonProperty("lastname")]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value ?? ""; }
        }

        private string _firstName = "";

        [JsonProperty("firstname")]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value ?? ""; }
        }

        private string _middleName = "";

        [JsonProperty("middlename")]
        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value ?? ""; }
        }

        [JsonProperty("school_code")]
        public string SchoolNumber { get; set; }

        [JsonProperty("school_name")]
        public string SchoolName { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        [JsonProperty("issuedate")]
        public DateTime? IssueDate { get; set; }

        [JsonProperty("numofbadges")]
        public int? NumberOfBadges { get; set; }

        private string _personId = "";

        [JsonProperty("personid")]
        public string PersonId
        {
            get { return _personId; }
            set { _personId = value ?? ""; }
        }

        [JsonProperty("DOB")]
        public DateTime? BirthDate { get; set; }

        //New additions ==========================

        [JsonProperty("badgetype")]
        public string BadgeType { get; set; }

        private string _lastUpdatedBy;

        [JsonProperty("lastupdatedby")]
        public string LastUpdatedBy
        {
            get { return _lastUpdatedBy; }
            set { _lastUpdatedBy = value ?? ""; }
        }

        [JsonProperty("lastupdated")]
        public DateTime? LastUpdated { get; set; }

        private string _schoolCode2;

        [JsonProperty("school_code_2")]
        public string SchoolCode2
        {
            get { return _schoolCode2; }
            set { _schoolCode2 = value ?? ""; }
        }

        [JsonProperty("printdate")]
        public Nullable<DateTime> PrintDate { get; set; }

        private string _mifareCardNumber = "";

        [JsonProperty("mifare_card_num")]
        public string MifareCardNumber
        {
            get { return _mifareCardNumber; }
            set { _mifareCardNumber = value ?? ""; }
        }

        private string _mifareBussPassNum;

        [JsonProperty("mifare_bus_pass_num")]
        public string MifareBusPassNum
        {
            get { return _mifareBussPassNum; }
            set { _mifareBussPassNum = value ?? ""; }
        }

        private string _mifare_eligible;

        [JsonProperty("mifare_eligible")]
        public string MifareEligible
        {
            get { return _mifare_eligible; }
            set { _mifare_eligible = value ?? ""; }
        }

        private string _mifareApproved;

        [JsonProperty("mifare_approved")]
        public string MifareApproved
        {
            get { return _mifareApproved; }
            set { _mifareApproved = value ?? ""; }
        }

        [JsonProperty("hashkey")]
        public long? Hashkey { get; set; }

        [JsonProperty("expiredate")]
        public DateTime? Expiredate { get; set; }

        [JsonProperty("expiration")]
        public DateTime? Expiration { get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        [JsonProperty("homeroom")]
        public string Homeroom { get; set; }

        [JsonProperty("ServType")]
        public string ServType { get; set; }

        [JsonProperty("calendar")]
        public string Calendar { get; set; }

        [JsonProperty("prev_mifare_card_num")]
        public string PrevMifareCardNum { get; set; }

        //New field for data retention policy
        //[JsonProperty("active")]
        //public bool Active { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("checksum")]
        public string Checksum { get; set; }
        [JsonProperty("combined")]
        public List<string> Combined { get; set; }
    }
}
