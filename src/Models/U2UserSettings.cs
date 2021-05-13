using NPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace U2.Models
{
    [TableName("U2UserSettings")]
    [PrimaryKey("UserId", AutoIncrement = false)]

    public class U2UserSettings
    {
        [Column("UserId")]
        [PrimaryKeyColumn(AutoIncrement =false)]
        public int UserId { get; set; }

        [Column("AuthenticatorSecret")]
        public string UserSecret { get; set; }

        [Column("IsAuthenticatorEnabled")]
        public bool IsAuthenticatorEnabled { get; set; }
    }
}