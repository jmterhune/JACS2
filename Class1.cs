if (!t.def_attorney_id.HasValue || !t.opp_attorney_id.HasValue)
{
    string sql = "INSERT INTO [courts] ([description],[case_num_format],[county_id],[plaintiff]," +
        "[defendant],[scheduling],[web_policy],[public_timeslot],[public_docket],[public_docket_days]," +
        "[email_confirmations],[lagtime],[custom_email_body],[twitter_notification],[calendar_weeks]," +
        "[auto_extension],[plaintiff_required],[defendant_required],[defendant_attorney_required]," +
        "[plaintiff_attorney_required],[category_print],[max_lagtime],[custom_header],[timeslot_header]," +
        "[created_at],[updated_at],[case_format_type]) " +
        "OUTPUT INSERTED.[id] VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18,@19,@20," +
        "@21,@22,@23,@24,@25,@26)";
    if (t.def_attorney_id.HasValue)
    {
        sql = "INSERT INTO [courts] ([description],[case_num_format],[county_id],[plaintiff],def_attorney_id," +
        "[defendant],[scheduling],[web_policy],[public_timeslot],[public_docket],[public_docket_days]," +
        "[email_confirmations],[lagtime],[custom_email_body],[twitter_notification],[calendar_weeks]," +
        "[auto_extension],[plaintiff_required],[defendant_required],[defendant_attorney_required]," +
        "[plaintiff_attorney_required],[category_print],[max_lagtime],[custom_header],[timeslot_header]," +
        "[created_at],[updated_at],[case_format_type]) " +
        "OUTPUT INSERTED.[id] VALUES (@0,@1,@2,@3," + t.def_attorney_id.Value + ",@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18,@19,@20," +
        "@21,@22,@23,@24,@25,@26)";
    }
    if (t.opp_attorney_id.HasValue)
    {
        sql = "INSERT INTO [courts] ([description],[case_num_format],[county_id],[plaintiff],opp_attorney_id," +
        "[defendant],[scheduling],[web_policy],[public_timeslot],[public_docket],[public_docket_days]," +
        "[email_confirmations],[lagtime],[custom_email_body],[twitter_notification],[calendar_weeks]," +
        "[auto_extension],[plaintiff_required],[defendant_required],[defendant_attorney_required]," +
        "[plaintiff_attorney_required],[category_print],[max_lagtime],[custom_header],[timeslot_header]," +
        "[created_at],[updated_at],[case_format_type]) " +
        "OUTPUT INSERTED.[id] VALUES (@0,@1,@2,@3," + t.opp_attorney_id.Value + ",@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18,@19,@20," +
        "@21,@22,@23,@24,@25,@26)";
    }
    courtId = ctx.ExecuteScalar<long>(CommandType.Text, sql, t.description, t.case_num_format, t.county_id, t.plaintiff, t.defendant, t.scheduling,
        t.web_policy, t.public_timeslot, t.public_docket, t.public_docket_days, t.email_confirmations, t.lagtime, t.custom_email_body,
        t.twitter_notification, t.calendar_weeks, t.auto_extension, t.plaintiff_required, t.defendant_required, t.defendant_attorney_required,
        t.plaintiff_attorney_required, t.category_print, t.max_lagtime, t.custom_header, t.timeslot_header, DateTime.Now, DateTime.Now, t.case_format_type);
    string cacheKey = "Courts"; // Example; match your retrieval code
    DataCache.RemoveCache(cacheKey);
    Court court = new Court
    {
        county_id = t.county_id,
        auto_extension = t.auto_extension,
        calendar_weeks = t.calendar_weeks,
        case_format_type = t.case_format_type,
        case_num_format = t.case_num_format,
        category_print = t.category_print,
        custom_email_body = t.custom_email_body,
        custom_header = t.custom_header,
        defendant = t.defendant,
        def_attorney_id = t.def_attorney_id.HasValue ? t.def_attorney_id.Value : (long?)null,
        opp_attorney_id = t.opp_attorney_id.HasValue ? t.opp_attorney_id.Value : (long?)null,
        defendant_attorney_required = t.defendant_attorney_required,
        defendant_required = t.defendant_required,
        description = t.description,
        email_confirmations = t.email_confirmations,
        lagtime = t.lagtime,
        max_lagtime = t.max_lagtime,
        plaintiff = t.plaintiff,
        plaintiff_attorney_required = t.plaintiff_attorney_required,
        plaintiff_required = t.plaintiff_required,
        public_docket = t.public_docket,
        public_docket_days = t.public_docket_days,
        public_timeslot = t.public_timeslot,
        scheduling = t.scheduling,
        timeslot_header = t.timeslot_header,
        web_policy = t.web_policy,
        twitter_notification = t.twitter_notification,
        created_at = DateTime.Now,
        updated_at = DateTime.Now,
    };