﻿using FreeSql.DataAnnotations;
using System;

namespace AgileConfig.Server.Data.Entity
{
    [Table(Name = "agc_user_role")]
    public class UserRole
    {
        [Column(Name = "id", StringLength = 36)]
        public string Id { get; set; }

        [Column(Name = "app_id", StringLength = 36)]
        public string AppId { get; set; }

        [Column(Name = "user_id", StringLength = 50)]
        public string UserId { get; set; }

        [Column(Name = "role")]
        public Role Role { get; set; }

        [Column(Name = "create_time")]
        public DateTime CreateTime { get; set; }

    }

    public enum Role
    {
        SystemAdmin = 0,//系统管理员
        AppAdmin, //app管理员
        Editor, //
        Publisher //
    }

}