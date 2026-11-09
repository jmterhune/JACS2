/*
    Court Registry - editable attorney emails for the withdrawal process.

    RUN THIS AGAINST: the jud12.flcourts.org database (the one holding the other
    tjc_car_* tables, which the intranet module reaches through its "Jud12"
    connection). The module control is registered by a separate script against the
    intranet DNN database.

    Safe to re-run: the table is created only if missing, and each seed row is
    inserted only if its key is absent, so wording Court Administration has edited
    is never overwritten.

    The seeded bodies reproduce the wording the code sent before these became
    editable. If a row is missing, disabled or empty, the module falls back to that
    same built-in wording, so running this script is not required for the emails to
    work - it is what makes them editable.
*/

SET NOCOUNT ON
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tjc_car_email_templates]') AND type = N'U')
BEGIN
    CREATE TABLE [dbo].[tjc_car_email_templates](
        [TemplateId]        [int] IDENTITY(1,1) NOT NULL,
        [TemplateKey]       [nvarchar](100) NOT NULL,
        [TemplateName]      [nvarchar](200) NOT NULL,
        [Description]       [nvarchar](500) NULL,
        [Subject]           [nvarchar](500) NOT NULL,
        [Body]              [nvarchar](max) NOT NULL,
        [IsEnabled]         [bit] NOT NULL,
        [DateModified]      [datetime] NULL,
        [ModifiedByUserId]  [int] NULL,
     CONSTRAINT [PK_tjc_car_email_templates] PRIMARY KEY CLUSTERED
    (
        [TemplateId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    ALTER TABLE [dbo].[tjc_car_email_templates] ADD CONSTRAINT [DF_tjc_car_email_templates_IsEnabled] DEFAULT ((1)) FOR [IsEnabled]

    PRINT 'Created table tjc_car_email_templates.'
END
ELSE
    PRINT 'Table tjc_car_email_templates already exists - left alone.'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_tjc_car_email_templates_Key' AND object_id = OBJECT_ID(N'[dbo].[tjc_car_email_templates]'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [UX_tjc_car_email_templates_Key]
        ON [dbo].[tjc_car_email_templates] ([TemplateKey] ASC)
    PRINT 'Created index UX_tjc_car_email_templates_Key.'
END
ELSE
    PRINT 'Index UX_tjc_car_email_templates_Key already exists.'
GO

/* ----------------------------------------------------------------------
   Seed rows. Each is inserted only when its key is absent.
   ---------------------------------------------------------------------- */

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_car_email_templates WHERE TemplateKey = N'withdrawal.submitted')
BEGIN
    INSERT INTO dbo.tjc_car_email_templates (TemplateKey, TemplateName, [Description], [Subject], Body, IsEnabled)
    VALUES (N'withdrawal.submitted',
            N'Removal request received',
            N'Sent to the attorney as soon as they submit a request to be removed from the registry.',
            N'Your Court Registry Removal Request',
            N'<p>Dear [FullName],</p>
<p>Court Administration has received your request to be removed from the 12<sup>th</sup> Circuit court appointed registry. The request is pending review. You will receive another email when it has been acted on, and you remain on the registry until then.</p>
<p><strong>Removal applies to future appointments only</strong><br />Removal from the registry applies to future appointments only. You are required to continue representation on every case you are currently appointed to until either the case ends or you file a motion to withdraw in that case, set the motion for hearing, and the motion is granted by the court. Your representation on a case you are already appointed to is not over by this request to be removed from the registry.</p>
[if:Reason]<p><strong>Reason you gave:</strong><br />[Reason]</p>[/if]
<p>Requested on [DateRequested] and signed [Signature].</p>',
            1)
    PRINT 'Seeded withdrawal.submitted.'
END
ELSE
    PRINT 'withdrawal.submitted already present - left alone.'
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_car_email_templates WHERE TemplateKey = N'withdrawal.approved')
BEGIN
    INSERT INTO dbo.tjc_car_email_templates (TemplateKey, TemplateName, [Description], [Subject], Body, IsEnabled)
    VALUES (N'withdrawal.approved',
            N'Removal request approved',
            N'Sent to the attorney when Court Administration approves the request and takes them off the registry.',
            N'Your Court Registry Removal Request Has Been Approved',
            N'<p>Dear [FullName],</p>
<p>Court Administration has approved your request to be removed from the 12<sup>th</sup> Circuit court appointed registry, effective [EffectiveDate]. You will not be considered for new appointments.</p>
<p><strong>Removal from the registry applies to future appointments only. You are required to continue representation on every case you are currently appointed to until either the case ends or you file a motion to withdraw in that case, set the motion for hearing, and the motion is granted by the court. Your representation on a case you are already appointed to is not over by this request to be removed from the registry.</strong></p>
[NoteBlock]',
            1)
    PRINT 'Seeded withdrawal.approved.'
END
ELSE
    PRINT 'withdrawal.approved already present - left alone.'
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tjc_car_email_templates WHERE TemplateKey = N'withdrawal.denied')
BEGIN
    INSERT INTO dbo.tjc_car_email_templates (TemplateKey, TemplateName, [Description], [Subject], Body, IsEnabled)
    VALUES (N'withdrawal.denied',
            N'Removal request denied',
            N'Sent to the attorney when Court Administration denies the request. They stay on the registry.',
            N'Your Court Registry Removal Request Has Been Denied',
            N'<p>Dear [FullName],</p>
<p>Court Administration has denied your request to be removed from the 12<sup>th</sup> Circuit court appointed registry. You remain on the registry and will continue to receive appointments.</p>
[NoteBlock]',
            1)
    PRINT 'Seeded withdrawal.denied.'
END
ELSE
    PRINT 'withdrawal.denied already present - left alone.'
GO

SELECT TemplateId, TemplateKey, TemplateName, IsEnabled, DateModified
FROM dbo.tjc_car_email_templates ORDER BY TemplateKey
/* expect 3 rows */
GO
